using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class ProcLevelMesh : MonoBehaviour {
    MeshFilter filter;
    
    public bool generateOnStart = true;
    
    [System.NonSerialized]
    public int[,,] levelArr = new int[100, 100, 100];
    [System.NonSerialized]
    public Dictionary<Vector3Int, Vector3Int> excludedWalls;
    
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<List<int>> triangles = new List<List<int>>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();
    
    void Start() {
        //Reset portal holes
        foreach (Material m in GetComponent<Renderer>().sharedMaterials) {
            for (int i = 1; i <= 2; i++) {
                m.SetVector("_PortalPos" + (i).ToString(), new Vector3(1000, 1000, 1000));
                m.SetVector("_PortalRot" + (i).ToString(), Vector4.zero);
            }
        }
        
        filter = GetComponent<MeshFilter>();
        filter.mesh = new Mesh();
        
        //Set up array
        ClearLevel();
        
        //Generate mesh
        if (generateOnStart)
            Generate();
    }
    
    public void ClearLevel() {
        excludedWalls = new Dictionary<Vector3Int, Vector3Int>();
        
        for (int x = 0; x < levelArr.GetLength(0); x++) {
            for (int y = 0; y < levelArr.GetLength(1); y++) {
                for (int z = 0; z < levelArr.GetLength(2); z++) {
                    levelArr[x,y,z] = 1;
                }
            }
        }
        
        for (int x = 45; x < 55; x++) {
            for (int y = 48; y < 52; y++) {
                for (int z = 45; z < 55; z++) {
                    levelArr[x,y,z] = 0;
                }
            }
        }
    }
    
    public void Generate() {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<List<int>>();
        triangles.Add(new List<int>());
        triangles.Add(new List<int>());
        
        uvs = new List<Vector2>();
        colors = new List<Color>();
    
        //Do generation!
        for (int x = 0; x < levelArr.GetLength(0); x++) {
            for (int y = 0; y < levelArr.GetLength(1); y++) {
                for (int z = 0; z < levelArr.GetLength(2); z++) {
                    //If levelArr[x,y,z] is empty
                    if (levelArr[x,y,z] == 0) {
                        AddEmpty(new Vector3Int(x,y,z));
                    }
                }
            }
        }

        //Clear mesh
        filter.mesh.Clear();
        filter.mesh.subMeshCount = GetComponent<MeshRenderer>().sharedMaterials.Length;
        
        //Set filter.mesh properties
        filter.mesh.SetVertices(vertices);
        filter.mesh.SetNormals(normals);
        for (int i = 0; i < GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
        {
            filter.mesh.SetTriangles(triangles[i], i);
            //filter.mesh.SetTriangles(triangles[0], 0);
        }
        filter.mesh.SetUVs(0, uvs);
        filter.mesh.SetColors(colors);
        
        //Update collider
        GetComponent<MeshCollider>().sharedMesh = filter.mesh;
    }
    
    void AddEmpty(Vector3Int pos) {
        List<Vector3Int> directions = new List<Vector3Int>() {
            Vector3Int.up,
            -Vector3Int.up,
            Vector3Int.right,
            -Vector3Int.right,
            Vector3Int.forward,
            -Vector3Int.forward,
        };
        
        foreach (Vector3Int d in directions) {
            Vector3Int offsetPos = pos + d;
            
            //Out of bounds
            if (offsetPos.x < 0 || offsetPos.y < 0 || offsetPos.z < 0 ||
                    offsetPos.x >= levelArr.GetLength(0) ||
                    offsetPos.y >= levelArr.GetLength(1) ||
                    offsetPos.z >= levelArr.GetLength(2)) {
                AddQuad(offsetPos, -d, 0);
                continue;
            }
            
            //There is a filled voxel next to this empty voxel
            if (levelArr[offsetPos.x, offsetPos.y, offsetPos.z] != 0) {
                //Lets add a quad, facing the opposite direction as the offset
                AddQuad(offsetPos, -d, levelArr[offsetPos.x, offsetPos.y, offsetPos.z] - 1);
            }
        }
    }
    
    void AddQuad(Vector3Int pos, Vector3Int normal, int material) {
        //If wall is excluded (i.e. a door will be on this wall) do nothing
        if (excludedWalls.ContainsKey(pos) && excludedWalls[pos] == normal)
            return;
        
        Vector3[] tempVertices = new Vector3[4] {
            new Vector3(-0.5f, -0.5f, 0.0f), //BOTTOM LEFT
            new Vector3(0.5f, -0.5f, 0.0f), //BOTTOM RIGHT
            new Vector3(-0.5f, 0.5f, 0.0f), //TOP LEFT
            new Vector3(0.5f, 0.5f, 0.0f) //TOP RIGHT
        };
        int[] tempTriangles = new int[6] {
            0,2,1,
            2,3,1
        };
        Vector2[] tempWallUvs = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(0.5f, 0),
            new Vector2(0, 1),
            new Vector2(0.5f, 1)
        };
        Vector2[] tempGroundUvs = new Vector2[4]
        {
            new Vector2(0.5f, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1),
            new Vector2(1, 1)
        };
        
        Vector3 center = ((Vector3)pos) + ((Vector3)normal)/2.0f;
        Quaternion rotation = Quaternion.LookRotation(-normal, Vector3.up);
        
        for (int i = 0; i < tempTriangles.Length; i++) {
            triangles[material].Add(tempTriangles[i] + vertices.Count);
        }
        for (int i = 0; i < 4; i++) {
            vertices.Add((rotation * tempVertices[i]) + center);
            normals.Add(normal);
            if (Mathf.Abs(normal.y) == 1) {
                uvs.Add(tempGroundUvs[i]);
            } else {
                uvs.Add(tempWallUvs[i]);
            }
        }
        
        //Colors
        for (int i = 0; i < 4; i++) {
            //Vector3Int side1Offset = pos + normal + (Quaternion.AngleAxis(90 * i, normal) * Vector3.Cross(normal, Vector3.up).normalized).RoundToInt();
            //if (Mathf.Abs(normal.y) == 1)
            //    side1Offset = pos + normal + (Quaternion.AngleAxis(90 * i, normal) * Vector3.Cross(normal, Vector3.right).normalized).RoundToInt();
            //
            //Vector3Int side2Offset = pos + normal + (Quaternion.AngleAxis(90 * (i+1), normal) * Vector3.Cross(normal, Vector3.up).normalized).RoundToInt();
            //if (Mathf.Abs(normal.y) == 1)
            //    side2Offset = pos + normal + (Quaternion.AngleAxis(90 * (i+1), normal) * Vector3.Cross(normal, Vector3.right).normalized).RoundToInt();
            
            Vector3Int cornerPos = (rotation * tempVertices[i]).normalized.RoundToInt() + pos + normal;
            bool corner = levelArr[cornerPos.x, cornerPos.y, cornerPos.z] != 0;
            
            Vector3Int side1Pos = (rotation * (Quaternion.AngleAxis(45, Vector3.forward) * tempVertices[i])).normalized.RoundToInt() + pos + normal;
            bool side1 = levelArr[side1Pos.x, side1Pos.y, side1Pos.z] != 0;
            
            Vector3Int side2Pos = (rotation * (Quaternion.AngleAxis(-45, Vector3.forward) * tempVertices[i])).normalized.RoundToInt() + pos + normal;
            bool side2 = levelArr[side2Pos.x, side2Pos.y, side2Pos.z] != 0;
            
            colors.Add(Mathf.Max(VertexAO(side1, side2, corner), 0)/3.0f * Color.white);
        }
    }
    
    int VertexAO(bool side1, bool side2, bool corner) {
        if(side1 && side2) {
            return 0;
        }
        return 3 - ((side1 ? 1 : 0) + (side2 ? 1 : 0) + (corner ? 1 : 0));
    }
}
