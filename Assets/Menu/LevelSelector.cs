using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public SaveLoadHandler saveLoadHandler;
    
    [Header("Object References")]
    public Transform mapParent;
    public Transform levelObjectsParent;
    public Transform menuDoor;
    public GameObject exitZone;
    
    [Header("Display References")]
    public TextMeshPro numberText;
    public TextMeshPro titleText;
    
    int selectedLevel = 0;
    
    void Start()
    {
        RedrawText();
    }
    
    void Update() {
        var keyboard = Keyboard.current;
        
        if (keyboard.upArrowKey.wasPressedThisFrame) {
            Next();
        } else if (keyboard.downArrowKey.wasPressedThisFrame) {
            Prev();
        } else if (keyboard.enterKey.wasPressedThisFrame) {
            SelectLevel();
        }
    }
    
    public void SelectLevel() {
        StartCoroutine(SelectLevelCoroutine());
    }
    
    public void Next() {
        selectedLevel++;
        if (selectedLevel > saveLoadHandler.levelPaths.Length - 1)
            selectedLevel = saveLoadHandler.levelPaths.Length - 1;
        RedrawText();
    }
    
    public void Prev() {
        selectedLevel--;
        if (selectedLevel < 0)
            selectedLevel = 0;
        RedrawText();
    }
    
    void RedrawText() {
        numberText.text = (selectedLevel + 1).ToString();
        titleText.text = Path.GetFileNameWithoutExtension(saveLoadHandler.levelPaths[selectedLevel]);
    }
    
    IEnumerator SelectLevelCoroutine() {
        menuDoor.gameObject.SetActive(true);
        saveLoadHandler.Load(saveLoadHandler.levelPaths[selectedLevel], false);
        
        //Move level to align with menu door
        Transform entryDoor = GameObject.Find("Entry Door").transform;
        //Assuming the doors are always on non-sloped walls
        Quaternion offsetRot = Quaternion.AngleAxis(
                (menuDoor.eulerAngles.y - entryDoor.eulerAngles.y) + 180, Vector3.up);
        mapParent.transform.Rotate(offsetRot.eulerAngles);
        levelObjectsParent.transform.Rotate(offsetRot.eulerAngles);
        
        Vector3 offset = menuDoor.position - entryDoor.position;
        mapParent.transform.position += offset;
        levelObjectsParent.transform.position += offset;
        
        menuDoor.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        
        //menuDoor.GetComponentInChildren<Activatable>().Activate();
        entryDoor.GetComponentInChildren<Activatable>().Activate();
    }
    
    public void DoHideCoroutine(Collider c) {
        if (c.gameObject.tag == "MainCamera")
            StartCoroutine(HideCoroutine());
    }
    public IEnumerator HideCoroutine() {
        exitZone.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        Transform entryDoor = GameObject.Find("Entry Door").transform;
        
        entryDoor.GetComponentInChildren<Activatable>().Deactivate();
        yield return new WaitForSeconds(1.0f);
        Hide();
        
        //Move to align with exit door
        Transform exitDoor = GameObject.Find("Exit Door").transform;
        //Assuming the doors are always on non-sloped walls
        Quaternion offsetRot = Quaternion.AngleAxis(
                (menuDoor.eulerAngles.y - exitDoor.eulerAngles.y) + 180, Vector3.up);
        transform.Rotate(offsetRot.eulerAngles);
        
        Vector3 offset = exitDoor.position - menuDoor.position;
        transform.position += offset;
    }
    
    public void Show() {
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
            r.enabled = true;
        }
        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
            c.enabled = true;
        }
        foreach (Light l in gameObject.GetComponentsInChildren<Light>()) {
            l.enabled = true;
            l.gameObject.SetActive(false);
            l.gameObject.SetActive(true);
        }
    }
    
    public void Hide() {
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
            r.enabled = false;
        }
        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
            c.enabled = false;
        }
        foreach (Light l in gameObject.GetComponentsInChildren<Light>()) {
            l.enabled = false;
        }
    }
}
