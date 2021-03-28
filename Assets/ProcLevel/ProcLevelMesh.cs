using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#region Tuple Definition
public struct Tuple<T1, T2>
{
    public readonly T1 Item1;
    public readonly T2 Item2;
    public Tuple(T1 item1, T2 item2) { Item1 = item1; Item2 = item2; }
}
#endregion

public class ProcLevelMesh : MonoBehaviour {

    MeshFilter filter;

    //The level will be represented by a 3 dim array. A value of 1 means the area is empty, and a value of 0 means it is solid.
    //All lengths should be the same
    public int[,,] levelArr = new int[100, 100, 100];
    //The walls will be in a dict with a vector3 that shows the grid position and a directional Vector3 that shows the side
    public Dictionary<Tuple<Vector3, Vector3>, int> wallDict = new Dictionary<Tuple<Vector3, Vector3>, int>();

    int GetWall(Vector3 pos, Vector3 side)
    {
        if (wallDict.ContainsKey(new Tuple<Vector3, Vector3>(pos, side)))
        {
            return wallDict[new Tuple<Vector3, Vector3>(pos, side)];
        } else
        {
            return 0;
        }
    }

	void Start () {
        filter = GetComponent<MeshFilter>();
        filter.mesh = new Mesh();
        for (var x = 45; x < 55; x++)
        {
            for (var y = 45; y < 50; y++)
            {
                for (var z = 45; z < 55; z++)
                {
                    levelArr[x, y, z] = 1;
                }
            }
        }
        wallDict[new Tuple<Vector3, Vector3>(new Vector3(45, 45, 45), -Vector3.up)] = 0;
        Generate();
	}

    public void Generate()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<List<int>> triangles = new List<List<int>>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
        {
            triangles.Add(new List<int>());
        }

        for (var x = 0; x < levelArr.GetLength(0); x++)
        {
            for (var y = 0; y < levelArr.GetLength(0); y++)
            {
                for (var z = 0; z < levelArr.GetLength(0); z++)
                {
                    if (levelArr[x, y, z] == 1)
                    {
                        GenArrayIndex(new Vector3(x, y, z), ref vertices, ref triangles, ref uvs);
                    }
                }
            }
        }


        filter.mesh.Clear();
        filter.mesh.subMeshCount = GetComponent<MeshRenderer>().sharedMaterials.Length;

        filter.mesh.SetVertices(vertices);
        for (int i = 0; i < GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
        {
            Debug.Log(i);
            filter.mesh.SetTriangles(triangles[i], i);
        }
        filter.mesh.SetUVs(0, uvs);

        filter.mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = filter.mesh;
    }

    void GenArrayIndex(Vector3 pos, ref List<Vector3> verticesRef, ref List<List<int>> triRef, ref List<Vector2> uvsRef)
    {
        List<Vector3> OffsetList = new List<Vector3>();
        OffsetList.Add(new Vector3(1, 0, 0));
        OffsetList.Add(new Vector3(0, 1, 0));
        OffsetList.Add(new Vector3(0, 0, 1));
        OffsetList.Add(new Vector3(-1, 0, 0));
        OffsetList.Add(new Vector3(0, -1, 0));
        OffsetList.Add(new Vector3(0, 0, -1));

        foreach (Vector3 offset in OffsetList)
        {
            Vector3 realPos = pos + offset;

            //If value in array
            if ((int) realPos.x < levelArr.GetLength(0) && (int) realPos.y < levelArr.GetLength(0) && (int) realPos.z < levelArr.GetLength(0)
                && (int) realPos.x >= 0 && (int) realPos.y >= 0 && (int) realPos.z >= 0)
            {
                if (levelArr[(int) realPos.x, (int)realPos.y, (int)realPos.z] == 0)
                {
                    //Value is wall
                    AddQuad(1, realPos - (offset/2), offset, pos, ref verticesRef, ref triRef, ref uvsRef);
                }
            } else
            {
                //OFFSET THING?
                //Value is not in array, put a wall
                AddQuad(1, realPos - (offset/2), offset, pos, ref verticesRef, ref triRef, ref uvsRef);
            }
        }
    }

    void AddQuad(float width, Vector3 position, Vector3 direction, Vector3 gridPos, ref List<Vector3> verticesRef, ref List<List<int>> triRef, ref List<Vector2> uvsRef)
    {
        int wallMaterial = GetWall(gridPos, new Vector3(direction.x, direction.y, -direction.z));

        int[] tempQuadTris = new int[6];
        Vector3[] tempQuadNormals = new Vector3[4];
        Vector2[] tempQuadUVs = new Vector2[4];

        // Quad verts (based on direction)
        if (vec3abs(direction) == new Vector3(0, 1, 0))
        {
            //Up
            verticesRef.Add(new Vector3(-width / 2, 0, -width / 2));
            verticesRef.Add(new Vector3(width / 2, 0, -width / 2));
            verticesRef.Add(new Vector3(-width / 2, 0, width / 2));
            verticesRef.Add(new Vector3(width/2, 0, width/2));

            //Fix direction issue
            direction = -direction;
        } else if (vec3abs(direction) == new Vector3(0, 0, 1))
        {
            //Forward
            verticesRef.Add(new Vector3(-width / 2, -width / 2, 0));
            verticesRef.Add(new Vector3(width / 2, -width / 2, 0));
            verticesRef.Add(new Vector3(-width / 2, width / 2, 0));
            verticesRef.Add(new Vector3(width / 2, width / 2, 0));
        } else if (vec3abs(direction) == new Vector3(1, 0, 0))
        {
            //Side
            verticesRef.Add(new Vector3(0, -width / 2, -width / 2));
            verticesRef.Add(new Vector3(0, -width / 2, width / 2));
            verticesRef.Add(new Vector3(0, width / 2, -width / 2));
            verticesRef.Add(new Vector3(0, width / 2, width / 2));

            //Fix direction issue
            direction = -direction;
        }

        // Add position
        for (int i = 0; i < 4; i++)
        {
            verticesRef[verticesRef.Count-4+i] += position;
        }

        if ((direction.x + direction.y + direction.z) > 0)
        {
            //  Lower left triangle.
            tempQuadTris[0] = (verticesRef.Count - 4) + 0;
            tempQuadTris[1] = (verticesRef.Count - 4) + 2;
            tempQuadTris[2] = (verticesRef.Count - 4) + 1;

            //  Upper right triangle.   
            tempQuadTris[3] = (verticesRef.Count - 4) + 2;
            tempQuadTris[4] = (verticesRef.Count - 4) + 3;
            tempQuadTris[5] = (verticesRef.Count - 4) + 1;
        } else if ((direction.x + direction.y + direction.z) < 0)
        {
            //  Lower left triangle.
            tempQuadTris[0] = (verticesRef.Count - 4) + 1;
            tempQuadTris[1] = (verticesRef.Count - 4) + 2;
            tempQuadTris[2] = (verticesRef.Count - 4) + 0;

            //  Upper right triangle.   
            tempQuadTris[3] = (verticesRef.Count - 4) + 1;
            tempQuadTris[4] = (verticesRef.Count - 4) + 3;
            tempQuadTris[5] = (verticesRef.Count - 4) + 2;
        }

        // Normals
        tempQuadNormals[0] = -Vector3.forward;
        tempQuadNormals[1] = -Vector3.forward;
        tempQuadNormals[2] = -Vector3.forward;
        tempQuadNormals[3] = -Vector3.forward;

        // UVs
        if (vec3abs(direction) == new Vector3(0, 1, 0))
        {
            //Floor or ceiling
            tempQuadUVs[0] = new Vector2(0.5f, 0);
            tempQuadUVs[1] = new Vector2(1, 0);
            tempQuadUVs[2] = new Vector2(0.5f, 0.5f);
            tempQuadUVs[3] = new Vector2(1, 0.5f);
        } else
        {
            //Wall
            tempQuadUVs[0] = new Vector2(0, 0);
            tempQuadUVs[1] = new Vector2(0.5f, 0);
            tempQuadUVs[2] = new Vector2(0, 0.5f);
            tempQuadUVs[3] = new Vector2(0.5f, 0.5f);
        }
        
        triRef[wallMaterial].AddRange(tempQuadTris);
        uvsRef.AddRange(tempQuadUVs);
    }

    Vector3 vec3abs(Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
}
