using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserActivatable : Activatable
{
    public GameObject spawnPrefab;
    public Transform spawnLocation;
    
    public override void Activate() {
        Instantiate(spawnPrefab, spawnLocation.position, spawnLocation.rotation, FindObjectOfType<SaveLoadHandler>().levelObjectsParent);
    }
}
