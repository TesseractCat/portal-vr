using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalGun : MonoBehaviour
{
    public PortalManager portalManager;
    
    public void OnShoot1(InputAction.CallbackContext context) {
        portalManager.ShootPortal(transform.position, transform.forward, 0, transform.rotation.eulerAngles.y);
    }
    
    public void OnShoot2(InputAction.CallbackContext context) {
        portalManager.ShootPortal(transform.position, transform.forward, 1, transform.rotation.eulerAngles.y);
    }
}
