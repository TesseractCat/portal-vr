using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelObject", menuName = "ScriptableObject/LevelScriptableObject", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
    [Header("Info")]
    public string label = "";
    
    [Header("Prefabs")]
    public GameObject objectPrefab;
    public GameObject visualPrefab;
    
    [Header("Config")]
    public Vector3Int canPlaceNormals = Vector3Int.one;
    public bool maxOne = false;
    public bool excludeWall = false;
    public bool decoration = false;
    public bool connectable = false;
}
