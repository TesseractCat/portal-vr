using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTimeOnSpawn : MonoBehaviour
{
    public string timeProperty;
    Vector3 lastPos = Vector3.zero;
    
    void Update()
    {
        if (transform.position != lastPos)
        {
            lastPos = transform.position;
            SetTime();
        }
    }
    
    public void SetTime() {
        GetComponent<Renderer>().material.SetFloat(timeProperty, Time.timeSinceLevelLoad);
    }
    public void ResetTime() {
        GetComponent<Renderer>().material.SetFloat(timeProperty, 0);
    }
}
