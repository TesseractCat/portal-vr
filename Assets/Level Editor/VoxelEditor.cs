using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

public class PlacedObject {
    public string label;
    
    public JsonVector3 position;
    public JsonVector3 normal;
    
    public JsonQuaternion rotation;
}

public class VoxelEditor : MonoBehaviour
{
    [Header("Object References")]
    public GameObject selectionQuad;
    public GameObject selectionStartQuad;
    
    public ProcLevelMesh levelMesh;
    public SaveLoadHandler saveLoadHandler;
    public GUISkin guiSkin;
    
    Vector3Int? leftHandle;
    Vector3Int? rightHandle;
    Vector3 handleNormal;
    
    LevelScriptableObject selectedObject = null;
    GameObject selectedObjectPreview = null;
    
    Level currentLevel;
    
    void Awake() {
        currentLevel = new Level();
    }
    
    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
        Vector3Int? point = null;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Ground")))) {
            Vector3 p = levelMesh.transform.InverseTransformPoint(hit.point);
            point = (p - hit.normal/2.0f).RoundToInt();
        }
        
        //Object placement
        if (selectedObject != null) {
            if (point.HasValue) {
                if (((selectedObject.canPlaceOnWalls && hit.normal.RoundToInt().y == 0) ||
                        (selectedObject.canPlaceOnGround && hit.normal.RoundToInt().Abs().y == 1))) {// &&
                        //!currentLevel.levelObjects.Find((point.Value + hit.normal).RoundToInt())) {
                    //We can show the object :)
                    selectedObjectPreview.transform.position =
                        levelMesh.transform.TransformPoint(point.Value + hit.normal/2.0f);
                    selectedObjectPreview.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                    
                    //Placing object
                    if (mouse.leftButton.wasPressedThisFrame) {
                        PlacedObject toPlace = new PlacedObject();
                        
                        toPlace.label = selectedObject.label;
                        toPlace.position = levelMesh.transform.InverseTransformPoint(
                                selectedObjectPreview.transform.GetChild(0).position);
                        toPlace.normal = hit.normal;
                        toPlace.rotation = selectedObjectPreview.transform.GetChild(0).rotation;
                        
                        currentLevel.levelObjects.Add(
                                new KeyValuePair<JsonVector3Int, PlacedObject>(
                                    (point.Value + hit.normal).RoundToInt(), toPlace));
                        
                        saveLoadHandler.PlaceObject(toPlace, true);
                    }
                } else {
                    selectedObjectPreview.transform.position = new Vector3(1000,1000,1000);
                }
            } else {
                selectedObjectPreview.transform.position = new Vector3(1000,1000,1000);
            }
            return;
        }
        
        //Selection
        if (leftHandle != null && rightHandle != null) {
            if (mouse.leftButton.wasPressedThisFrame) {
                ResetHandles();
            }
        } else if (point.HasValue) {
            selectionQuad.GetComponent<Renderer>().enabled = true;
            
            selectionQuad.transform.position =
                levelMesh.transform.TransformPoint(point.Value + hit.normal/2.0f + hit.normal/100.0f);
            selectionQuad.transform.forward = -hit.normal;
        
            if (mouse.leftButton.wasPressedThisFrame) {
                if (leftHandle == null) {
                    leftHandle = point.Value;
                    handleNormal = hit.normal;
                    
                    selectionStartQuad.transform.position = selectionQuad.transform.position;
                    selectionStartQuad.transform.rotation = selectionQuad.transform.rotation;
                    selectionStartQuad.GetComponent<Renderer>().enabled = true;
                } else if (rightHandle == null) {
                    if (CheckInline(leftHandle.Value, point.Value, handleNormal.RoundToInt())) {
                        rightHandle = point.Value;
                        selectionStartQuad.GetComponent<Renderer>().enabled = false;
                        ResizeHandle();
                    } else {
                        ResetHandles();
                    }
                }
            }
        } else {
            selectionQuad.GetComponent<Renderer>().enabled = false;
        }
        
        //Keyboard commands
        if (GUIUtility.keyboardControl == 0) {
            if (keyboard.rKey.wasPressedThisFrame) {
                IterateThroughHandles(leftHandle.Value, rightHandle.Value, (x,y,z) => {
                        levelMesh.levelArr[x, y, z] = 0;
                });
                
                levelMesh.Generate();
                
                MoveHandlesAlongNormal(-1);
            } else if (keyboard.fKey.wasPressedThisFrame) {
                MoveHandlesAlongNormal(1);
                
                IterateThroughHandles(leftHandle.Value, rightHandle.Value, (x,y,z) => {
                        levelMesh.levelArr[x, y, z] = 1;
                });
                
                levelMesh.Generate();
            } else if (keyboard.eKey.wasPressedThisFrame) {
                IterateThroughHandles(leftHandle.Value, rightHandle.Value, (x,y,z) => {
                        //Toggle
                        if (levelMesh.levelArr[x, y, z] == 1) {
                            levelMesh.levelArr[x, y, z] = 2;
                        } else if (levelMesh.levelArr[x, y, z] == 2) {
                            levelMesh.levelArr[x, y, z] = 1;
                        }
                });
                
                levelMesh.Generate();
            }
        }
    }
    
    void IterateThroughHandles(Vector3Int left, Vector3Int right, Action<int,int,int> lambda) {
        (int, int) xRange = (Mathf.Min(left.x, right.x), Mathf.Max(left.x, right.x));
        (int, int) yRange = (Mathf.Min(left.y, right.y), Mathf.Max(left.y, right.y));
        (int, int) zRange = (Mathf.Min(left.z, right.z), Mathf.Max(left.z, right.z));
        
        for (int x = xRange.Item1; x <= xRange.Item2; x++) {
            for (int y = yRange.Item1; y <= yRange.Item2; y++) {
                for (int z = zRange.Item1; z <= zRange.Item2; z++) {
                    lambda(x,y,z);
                }
            }
        }
    }
    
    void MoveHandlesAlongNormal(int amount) {
        if (leftHandle.HasValue && rightHandle.HasValue) {
            leftHandle = leftHandle.Value + (handleNormal * amount).RoundToInt();
            rightHandle = rightHandle.Value + (handleNormal * amount).RoundToInt();
            
            selectionQuad.transform.position +=
                (handleNormal * levelMesh.transform.localScale.x * amount).RoundToInt();
        }
    }
    
    void ResizeHandle() {
        Vector3Int mask = handleNormal.RoundToInt().Abs();
        Vector2Int leftVec2 = leftHandle.Value.MaskToVector2Int(mask);
        Vector2Int rightVec2 = rightHandle.Value.MaskToVector2Int(mask);
        
        //Vector2Int size =
        //    selectionQuad.transform.InverseTransformPoint(leftHandle.Value)
        //    .RoundToInt().Abs().MaskToVector2Int(new Vector3Int(0, 0, 1))
        //    + Vector2Int.one;
        
        Vector2Int size =
            ((rightHandle.Value - leftHandle.Value).MaskToVector2Int(mask)).Abs()
            + Vector2Int.one;
        
        selectionQuad.transform.localScale =
            levelMesh.transform.localScale.x * (Vector3)size.MaskToVector3Int(new Vector3Int(0, 0, 1), 1);
        
        selectionQuad.transform.position =
            levelMesh.transform.TransformPoint(((Vector3)(leftHandle + rightHandle))/2.0f + handleNormal/2.0f + handleNormal/100.0f);
        selectionQuad.transform.rotation = Quaternion.LookRotation(-handleNormal, Vector3.up);
            
    }
    
    void ResetHandles() {
        leftHandle = null;
        rightHandle = null;
        
        //Toggle
        selectionQuad.transform.localScale = 2.0f * Vector3.one;
        selectionQuad.GetComponent<Renderer>().enabled = false;
        selectionStartQuad.GetComponent<Renderer>().enabled = false;
    }
    
    void SelectObject(LevelScriptableObject toSelect) {
        if (selectedObjectPreview != null)
            GameObject.Destroy(selectedObjectPreview);
        
        selectedObject = toSelect;
        selectedObjectPreview = new GameObject(toSelect.label + " Preview");
        
        var tempObject = (GameObject)Instantiate(toSelect.visualPrefab, Vector3.zero, Quaternion.identity);
        tempObject.transform.parent = selectedObjectPreview.transform;
        tempObject.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
        
        ResetHandles();
    }
    
    bool CheckInline(Vector3Int a, Vector3Int b, Vector3Int normal) {
        int i = normal.Abs().IndexOf(1);
        Debug.Assert(i != -1);
        return (a[i] == b[i]);
    }
    
    //GUI and level saving/loading
    void Save() {
        saveLoadHandler.Save(currentLevel);
    }
    
    void Load(string path) {
        currentLevel = saveLoadHandler.Load(path, true);
        Debug.Log("Loading level: " + currentLevel.name + ", with dimensions: " + currentLevel.levelDimensions
                + ", and with array length: " + currentLevel.flatLevelArray.Length);
    }
    
    Rect objectPaletteRect = new Rect(20, 20, 1, 1);
    Vector2 scrollPosition;
    void OnGUI() {
        GUI.skin = guiSkin;
        objectPaletteRect = GUILayout.Window(0, objectPaletteRect, OnPalette, "Palette");
    }
    
    void OnPalette(int id) {
        GUILayout.Label("");
        foreach (LevelScriptableObject component in saveLoadHandler.levelObjects) {
            if (GUILayout.Button(component.label)) {
                SelectObject(component);
            }
        }
        
        GUILayout.Space(10);
        if (GUILayout.Button("Voxels")) {
            selectedObject = null;
            GameObject.Destroy(selectedObjectPreview);
            ResetHandles();
        }
        
        GUILayout.Space(20);
        
        GUILayout.Label("Level name: ");
        currentLevel.name = GUILayout.TextField(currentLevel.name, 25);
        if (GUILayout.Button("Save"))
            Save();
        if (GUILayout.Button("Reset Level")) {
            currentLevel = new Level();
            levelMesh.ClearLevel();
            levelMesh.Generate();
        }
        if (GUILayout.Button("Clear Objects")) {
            currentLevel.levelObjects = new List<KeyValuePair<JsonVector3Int, PlacedObject>>();
            saveLoadHandler.ClearLevelObjects();
        }
        
        GUILayout.Space(20);
        
        scrollPosition = GUILayout.BeginScrollView(
                scrollPosition, GUILayout.Height(100));
        
        foreach (string path in saveLoadHandler.levelPaths) {
            if (GUILayout.Button(Path.GetFileName(path)))
                Load(path);
        }
        
        GUILayout.EndScrollView();
        
        if (GUILayout.Button("Refresh"))
            saveLoadHandler.Refresh();
    }
}