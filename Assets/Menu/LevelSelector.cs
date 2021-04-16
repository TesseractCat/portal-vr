using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public SaveLoadHandler saveLoadHandler;
    
    public Transform mapParent;
    public Transform levelObjectsParent;
    
    public Transform menuDoor;
    
    void Start()
    {
        StartCoroutine(LateStart());
    }
    
    IEnumerator LateStart() {
        yield return new WaitForSeconds(1.0f);
        saveLoadHandler.Load(saveLoadHandler.levelPaths[0], false);
        
        Transform entryDoor = GameObject.Find("Entry Door").transform;
        Vector3 offset = menuDoor.position - entryDoor.position;
        
        mapParent.transform.position += offset;
        levelObjectsParent.transform.position += offset;
        
        menuDoor.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
