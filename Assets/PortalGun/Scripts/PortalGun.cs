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
        if (context.phase == InputActionPhase.Started) {
            portalManager.ShootPortal(transform.position, transform.forward, 0, transform.rotation.eulerAngles.y);
            Recoil();
        }
    }
    
    public void OnShoot2(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            portalManager.ShootPortal(transform.position, transform.forward, 1, transform.rotation.eulerAngles.y);
            Recoil();
        }
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
    
    void Recoil() {
        transform.localPosition = new Vector3(0, 0.05f, 0);
        transform.localRotation = Quaternion.Euler(-10.0f, 0, 0);
    }
    
    void Update() {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 10.0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 10.0f);
    }
}
