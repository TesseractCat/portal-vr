using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Noclip : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 50.0f;
    
    [Header("Object References")]
    public PortalManager portalManager;
    public Camera camera;
    
    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        float horizontal = (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0);
        float vertical = (keyboard.sKey.isPressed ? -1 : 0) + (keyboard.wKey.isPressed ? 1 : 0);
        
        float tempSpeed = speed;
        
        transform.position = transform.position +
            Vector3.ClampMagnitude(transform.forward * vertical +
            transform.right * horizontal +
            (keyboard.spaceKey.isPressed ? new Vector3(0, 1, 0) : Vector3.zero) +
            (keyboard.leftCtrlKey.isPressed ? new Vector3(0, -1, 0) : Vector3.zero), 1.0f) * tempSpeed;
    }
    
    void OnTriggerStay(Collider c) {
        if (c.gameObject.tag == "PortalColl" && c.transform.parent.InverseTransformPoint(transform.position).z > 0.01f)
        {
            Quaternion flipRot = Quaternion.AngleAxis(180.0f, c.transform.parent.up);
            transform.position = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal.TransformPoint(
                    flipRot * c.transform.parent.InverseTransformPoint(transform.position));
            
            transform.forward = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal.TransformDirection(
                    flipRot * c.transform.parent.InverseTransformDirection(transform.forward));
        }
    }
    
    void Update() {
        var mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame) {
            portalManager.ShootPortal(transform.position, camera.transform.forward, 0, 0);
        }
        if (mouse.rightButton.wasPressedThisFrame) {
            portalManager.ShootPortal(transform.position, camera.transform.forward, 1, 45);
        }
    }
}
