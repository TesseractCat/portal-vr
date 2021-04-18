using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableRelay : Activatable
{
    public UnityEvent onActivate;
    
    public override void Activate() {
        onActivate.Invoke();
    }
}
