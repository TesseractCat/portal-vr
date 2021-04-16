using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StandingButtonActivator : Activator
{
    public InputActionReference pressAction;
    
    public void OnTriggerStay(Collider c) {
        if (c.gameObject.tag == "Hand" && pressAction.action.triggered) {
            if (this.oneShot) {
                this.connected.Activate();
                return;
            }
            
            if (this.toggle && !this.activated) {
                this.connected.Activate();
                this.activated = true;
            } else if (this.toggle && this.activated) {
                this.connected.Deactivate();
                this.activated = false;
            } else if (!this.toggle && !this.activated) {
                this.connected.Activate();
                this.activated = true;
                StartCoroutine(DeactivateCoroutine());
            }
        }
    }
    
    IEnumerator DeactivateCoroutine() {
        yield return new WaitForSeconds(3);
        this.connected.Activate();
        this.activated = false;
    }
}
