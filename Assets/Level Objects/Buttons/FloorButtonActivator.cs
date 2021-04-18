using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButtonActivator : Activator
{
    public void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "Moveable") {
            if (!this.activated)
                Activate();
        }
    }
    
    public void OnTriggerExit(Collider c) {
        if (c.gameObject.tag == "Moveable") {
            if (this.activated)
                Deactivate();
        }
    }
}
