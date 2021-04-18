using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

public class Level {
    public string name = "Untitled";
    public string author = "Unknown";
    
    public JsonVector3Int levelDimensions = Vector3Int.one;
    public int[] flatLevelArray;
    
    //See: https://stackoverflow.com/questions/24504245/not-ableto-serialize-dictionary-with-complex-key-using-json-net/56351540
    public List<KeyValuePair<JsonVector3Int, PlacedObject>> levelObjects = new List<KeyValuePair<JsonVector3Int, PlacedObject>>();
}

public class SaveLoadHandler : MonoBehaviour
{
    [Header("Object References")]
    public ProcLevelMesh levelMesh;
    public Transform levelObjectsParent;
    
    public List<LevelScriptableObject> levelObjects;
    
    [Header("Prefabs")]
    public GameObject selectablePrefab;
    
    [Header("Config")]
    public int connectionChannels = 16;
    
    string savePath;
    string saveFolderPath;
    
    [System.NonSerialized]
    public string[] levelPaths;
    
    public void Awake() {
        saveFolderPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"../Levels/"));
        Refresh();
    }
    
    public void Refresh() {
        levelPaths = Directory.GetFiles(saveFolderPath);
    }
    
    public string Save(string name, string author) {
        Level l = new Level();
        l.name = name;
        l.author = author;
        l.levelDimensions = new Vector3Int(
                levelMesh.levelArr.GetLength(0),
                levelMesh.levelArr.GetLength(1),
                levelMesh.levelArr.GetLength(2));
        l.flatLevelArray = flattenedArray(levelMesh.levelArr);
        
        return JsonConvert.SerializeObject(l);
    }
    
    public void Save(Level l) {
        savePath = Path.GetFullPath(Path.Combine(Application.dataPath, @"../Levels/", l.name + ".json"));
        saveFolderPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"../Levels/"));
        
        l.levelDimensions = new Vector3Int(
                levelMesh.levelArr.GetLength(0),
                levelMesh.levelArr.GetLength(1),
                levelMesh.levelArr.GetLength(2));
        l.flatLevelArray = flattenedArray(levelMesh.levelArr);
        
        string json = JsonConvert.SerializeObject(l);
        
        Directory.CreateDirectory(saveFolderPath);
        File.WriteAllText(savePath, json);
    }

    public void Load(Level l, bool visual) {
        //Reset everything
        ClearLevelObjects();
        levelMesh.ClearLevel();
        
        //Place objects
        foreach (KeyValuePair<JsonVector3Int, PlacedObject> kv in l.levelObjects) {
            Transform tempObj = PlaceObject(kv.Value, kv.Key, visual);
            
            //Configure channel
            if (!visual) {
                Activator activator = tempObj.GetComponentInChildren<Activator>();
                Activatable activatable = tempObj.GetComponentInChildren<Activatable>();
                if (activator != null)
                    activator.channel = kv.Value.connection;
                if (activatable != null)
                    activatable.channel = kv.Value.connection;
            }
        }
        
        //Do connections
        if (!visual) {
            Activator[] activators = FindObjectsOfType<Activator>();
            Activatable[] activatables = FindObjectsOfType<Activatable>();
            
            //Iterate through all channels
            for (int i = 0; i <= connectionChannels; i++) {
                //Find all activators on this channel
                foreach (Activator av in activators) {
                    if (av.channel == i) {
                        //Add all activatables on the same channel
                        foreach (Activatable ab in activatables) {
                            if (ab.channel == i) {
                                av.connectedList.Add(ab);
                            }
                        }
                    }
                }
            }
        }
        
        //Generate voxel mesh
        levelMesh.levelArr = inflatedArray(l.flatLevelArray, l.levelDimensions);
        levelMesh.Generate();
    }
    
    public Level Load(string path, bool visual) {
        string json = File.ReadAllText(path);
        Level l = JsonConvert.DeserializeObject<Level>(json);
        Load(l, visual);
        return l;
    }
    
    public void ClearLevelObjects() {
        foreach (Transform t in levelObjectsParent.transform) {
            GameObject.Destroy(t.gameObject);
        }
        foreach (Transform t in levelMesh.transform) {
            GameObject.Destroy(t.gameObject);
        }
    }
    
    public Transform PlaceObject(PlacedObject toPlace, Vector3Int pos, bool visual) {
        LevelScriptableObject levelObject = null;
        foreach (LevelScriptableObject lo in levelObjects) {
            if (toPlace.label == lo.label) {
                levelObject = lo;
                break;
            }
        }
        
        if (levelObject == null)
            return null;
        
        Vector3 position = levelMesh.transform.TransformPoint(toPlace.position);
        GameObject tempObject;
        if (visual) {
            Transform selectableParent =
                Instantiate(selectablePrefab, position + (Vector3)toPlace.normal * levelMesh.transform.localScale.x * 0.5f, Quaternion.identity, levelObjectsParent).transform;
            selectableParent.GetComponent<BoxCollider>().size = levelMesh.transform.localScale;
            tempObject = Instantiate(levelObject.visualPrefab, position, toPlace.rotation, selectableParent);
        } else {
            tempObject = (GameObject)Instantiate(levelObject.objectPrefab, position, toPlace.rotation);
            tempObject.name = levelObject.label;
            if (tempObject.GetComponentInChildren<PortalObject>())
                tempObject.GetComponentInChildren<PortalObject>().enabled = true;
            
            if (levelObject.decoration) {
                tempObject.transform.parent = levelMesh.transform;
            } else {
                tempObject.transform.parent = levelObjectsParent.transform;
            }
            
            if (tempObject.GetComponentInChildren<Symbol>() != null) {
                tempObject.GetComponentInChildren<Symbol>().Assign(toPlace.connection);
            }
        }
        
        if (levelObject.excludeWall) {
            Vector3Int normal = ((Vector3)toPlace.normal).RoundToInt();
            levelMesh.excludedWalls.Add(pos - normal, normal);
        }
        
        return tempObject.transform;
    }
    
    public int[] flattenedArray(int[,,] multidimensionalArray) {
        int[] outArr = new int[multidimensionalArray.GetLength(0) * multidimensionalArray.GetLength(1) * multidimensionalArray.GetLength(2)];
        
        for (int x = 0; x < multidimensionalArray.GetLength(0); x++) {
            for (int y = 0; y < multidimensionalArray.GetLength(1); y++) {
                for (int z = 0; z < multidimensionalArray.GetLength(2); z++) {
                    outArr[to1D(x,y,z, multidimensionalArray.GetLength(0), multidimensionalArray.GetLength(1))] = multidimensionalArray[x,y,z];
                }
            }
        }
        
        return outArr;
    }
    
    public int[,,] inflatedArray(int[] flattenedArray, Vector3Int dimensions) {
        int[,,] outArr = new int[dimensions.x, dimensions.y, dimensions.z];
        
        for (int i = 0; i < flattenedArray.Length; i++) {
            Vector3Int idx = to3D(i, dimensions.x, dimensions.y);
            outArr[idx.x, idx.y, idx.z] = flattenedArray[i];
        }
        
        return outArr;
    }
    
    public int to1D(int x, int y, int z, int xMax, int yMax) {
        return (z * xMax * yMax) + (y * xMax) + x;
    }

    public Vector3Int to3D(int idx, int xMax, int yMax) {
        int z = idx / (xMax * yMax);
        idx -= (z * xMax * yMax);
        int y = idx / xMax;
        int x = idx % xMax;
        return new Vector3Int(x, y, z);
    }
}
