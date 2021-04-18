using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StandingButtonActivator : Activator
{
    public InputActionReference pressAction;
    
    public void OnTriggerStay(Collider c) {
        if (c.gameObject.tag == "Hand" && pressAction.action.triggered && pressAction.action.ReadValue<float>() > 0) {
            if (this.oneShot) {
                this.Activate();
                return;
            }
            
            if (this.toggle && !this.activated) {
                this.Activate();
            } else if (this.toggle && this.activated) {
                this.Deactivate();
            } else if (!this.toggle && !this.activated) {
                this.Activate();
                StartCoroutine(DeactivateCoroutine());
            }
        }
    }
    
    IEnumerator DeactivateCoroutine() {
        yield return new WaitForSeconds(3);
        this.Deactivate();
    }
}
