using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Activator : MonoBehaviour
{
    public int channel = -1;
    public List<Activatable> connectedList;
    
    public bool oneShot = false;
    public bool toggle = false;
    public bool activated = false;
    
    public virtual void Activate() {
        foreach (Activatable a in connectedList) {
            a.Activate();
        }
        activated = true;
    }
    public virtual void Deactivate() {
        foreach (Activatable a in connectedList) {
            a.Deactivate();
        }
        activated = false;
    }
    
    //public abstract 
}
