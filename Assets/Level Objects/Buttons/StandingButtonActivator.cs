using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StandingButtonActivator : Activator
{
    private Dictionary<string, object> _properties = new Dictionary<string, object>() {
        {"Press Time", (Int64)3},
        {"Permanent", false},
        {"Toggle", false}
    };
    public override Dictionary<string, object> properties {
        get {
            return _properties;
        }
        set {
            _properties = value;
        }
    }
    
    public InputActionReference pressAction;
    
    public void OnTriggerStay(Collider c) {
        if (c.gameObject.tag == "Hand" && pressAction.action.triggered && pressAction.action.ReadValue<float>() > 0) {
            Pressed();
        }
    }
    
    public void Pressed() {
        if ((bool)_properties["Permanent"]) {
            this.Activate();
            return;
        }
        
        bool toggle =(bool)_properties["Toggle"];
        if (toggle && !this.activated) {
            this.Activate();
        } else if (toggle && this.activated) {
            this.Deactivate();
        }
        
        if (!toggle && !this.activated) {
            this.Activate();
            StartCoroutine(DeactivateCoroutine());
        }
    }
    
    IEnumerator DeactivateCoroutine() {
        yield return new WaitForSeconds((Int64)_properties["Press Time"]);
        this.Deactivate();
    }
}
