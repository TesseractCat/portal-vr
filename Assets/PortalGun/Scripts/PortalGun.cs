using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalGun : MonoBehaviour
{
    public PortalManager portalManager;
    public LaserPointer laser;
    public Transform grabTransform;
    Rigidbody grabbedRigidbody;
    
    public void OnShoot1(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
            portalManager.ShootPortal(transform.position, transform.forward, 0, transform.rotation.eulerAngles.y);
    }
    
    public void OnShoot2(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
            portalManager.ShootPortal(transform.position, transform.forward, 1, transform.rotation.eulerAngles.y);
    }
    
    public void OnLaser(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
            laser.on = true;
        if (context.phase == InputActionPhase.Canceled)
            laser.on = false;
    }
    
    public void OnGrab(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            if (grabbedRigidbody != null) {
                grabbedRigidbody = null;
                return;
            }
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                if (hit.collider.gameObject.tag == "Moveable") {
                    grabbedRigidbody = hit.collider.GetComponent<Rigidbody>();
                }
            }
        }
    }
    
    void FixedUpdate() {
        if (grabbedRigidbody != null) {
            grabbedRigidbody.MovePosition(
                    Vector3.Lerp(grabbedRigidbody.transform.position, grabTransform.position, Time.fixedDeltaTime * 25.0f));
            grabbedRigidbody.velocity = Vector3.zero;
        }
    }
}
