using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;


public class TouchpadMovement : MonoBehaviour {

    [Header("Movement Properties")]
    public float speed;

    [Header("Object References")]
    public Transform lookCamera;
    public PostProcessVolume mainVolume;
    public InputActionReference movementAction;
    
    bool onGround = true;
    Vector3 currentDir;
    Vector3 lastPos = Vector3.zero;
    Transform lastPortal = null;
    
    Vector2 touchpadAxis = Vector2.zero;
    
    //public void OnMove(InputAction.CallbackContext context) {
    //    touchpadAxis = context.ReadValue<Vector2>();
    //}
    
    void FixedUpdate () {
        touchpadAxis = movementAction.action.ReadValue<Vector2>();
        
        if (onGround)
        {
            if (Mathf.Abs(touchpadAxis.magnitude) > 0)
            {
                Vector3 forwardDir = Quaternion.AngleAxis(lookCamera.rotation.eulerAngles.y, transform.up) * Vector3.forward * touchpadAxis.y;
                Vector3 rightDir = Quaternion.AngleAxis(lookCamera.rotation.eulerAngles.y + 90, transform.up) * Vector3.forward * touchpadAxis.x;
                Vector3 dir = forwardDir + rightDir;
                dir = dir * speed;
                dir.y = 0;// this.GetComponent<Rigidbody>().velocity.y;
                          //this.GetComponent<Rigidbody>().velocity = dir;
                          //transform.position += dir;
                GetComponent<Rigidbody>().velocity = dir;
                currentDir = dir;
            }
            else
            {
                currentDir = Vector3.zero;
            }
        }

        // Slow down effect
        /*if (InputManager.LeftHandGrab())
        {
            Time.timeScale = 0.25f;
            Vignette vignetteLayer;
            mainVolume.profile.TryGetSettings(out vignetteLayer);
            vignetteLayer.active = true;
        }
        else
        {
            Time.timeScale = 1;
            Vignette vignetteLayer;
            mainVolume.profile.TryGetSettings(out vignetteLayer);
            vignetteLayer.active = false;
        }*/

        //Handle elevation changes and falling
        Ray ray = new Ray(lookCamera.transform.position + Vector3.up/2, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground", "PortalTrigLayer")))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer")/* || cameraInLastPortal()*/)
            {
                //Player has stepped onto a portal, or fallen into one
                /*if (onGround)
                {
                    GetComponent<Rigidbody>().velocity = currentDir * 100;
                }*/
                onGround = false;
                //GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                GetComponent<Rigidbody>().drag = 0.3f;

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer")) {
                    lastPortal = hit.collider.gameObject.transform.parent;
                }
            } else if (hit.distance - Vector3.up.magnitude/2 < lookCamera.localPosition.y + 0.5 && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") /*Lower elevation tolerance*/)
            {
                //Higher elevation, for example steps or a ramp. Also could be slightly lower elevation, no need to start falling for this type of thing.
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), Time.deltaTime * 10);
                //transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                onGround = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                GetComponent<Rigidbody>().drag = 10;

            } else
            {
                //Player should be falling here (should limit movement)
                onGround = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                GetComponent<Rigidbody>().drag = 0.3f;
            }
        }

        lastPos = transform.position;
    }

    bool cameraInLastPortal()
    {
        if (lastPortal != null && lookCamera != null)
        {
            return lastPortal.Find("GeneralCollider").GetComponent<Collider>().bounds.Contains(lookCamera.position);
        }
        else
        {
            return false;
        }
    }
}
