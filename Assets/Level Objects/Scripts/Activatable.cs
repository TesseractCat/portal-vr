using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public virtual Dictionary<string, object> properties {
        get {
            return new Dictionary<string, object>() { };
        }
        set {
            return;
        }
    }
    
    public int channel = -1;
    
    public virtual void Activate() { }
    public virtual void Deactivate() { }
}
