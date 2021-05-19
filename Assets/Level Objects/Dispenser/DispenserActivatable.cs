using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserActivatable : Activatable
{
    public GameObject spawnPrefab;
    public Transform spawnLocation;
    
    GameObject lastDropped;
    
    bool activated = false;
    
    public override void Activate() {
        if (lastDropped != null && lastDropped.GetComponent<Fizzleable>())
            lastDropped.GetComponent<Fizzleable>().Fizzle();
        lastDropped = Instantiate(spawnPrefab, spawnLocation.position, spawnLocation.rotation, FindObjectOfType<SaveLoadHandler>().levelObjectsParent);
        
        activated = true;
    }
    
    public override void Deactivate() {
        activated = false;
    }
    
    void Update() {
        //If activated and the last dropped cube has been fizzled/destroyed
        if (activated && lastDropped == null) {
            //Drop a new one
            Activate();
        }
    }
}
