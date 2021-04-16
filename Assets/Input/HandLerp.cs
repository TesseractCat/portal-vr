using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLerp : MonoBehaviour
{
    public Transform toLerp;
    
    Vector3 startingPos;
    Quaternion startingRot;
    
    bool inZone = false;
    
    void Start()
    {
        startingPos = toLerp.localPosition;
        startingRot = toLerp.localRotation;
    }
    
    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "Zone") {
            inZone = true;
        }
    }
    
    void OnTriggerExit(Collider c) {
        if (c.gameObject.tag == "Zone") {
            inZone = false;
        }
    }
    
    void OnTriggerStay(Collider c) {
        if (c.gameObject.tag == "Zone") {
            toLerp.position = Vector3.Lerp(toLerp.position, c.transform.position, Time.deltaTime * 50.0f);
            toLerp.rotation = Quaternion.Lerp(toLerp.rotation, Quaternion.LookRotation(c.transform.up, c.transform.forward), Time.deltaTime * 25.0f);
        }
    }
    
    void Update() {
        if (!inZone) {
            toLerp.localPosition = Vector3.Lerp(toLerp.localPosition, startingPos, Time.deltaTime * 50.0f);
            toLerp.localRotation = Quaternion.Lerp(toLerp.localRotation, startingRot, Time.deltaTime * 50.0f);
        }
    }
}
