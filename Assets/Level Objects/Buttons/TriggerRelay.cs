using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEvent : UnityEvent<Collider>
{ }

public class TriggerRelay : MonoBehaviour
{
    public TriggerEvent onTriggerEnter;
    public TriggerEvent onTriggerExit;
    public TriggerEvent onTriggerStay;
    
    void OnTriggerEnter(Collider c) {
        if (onTriggerEnter != null)
            onTriggerEnter.Invoke(c);
    }
    
    void OnTriggerExit(Collider c) {
        if (onTriggerExit != null)
            onTriggerExit.Invoke(c);
    }
    
    void OnTriggerStay(Collider c) {
        if (onTriggerStay != null)
            onTriggerStay.Invoke(c);
    }
}
