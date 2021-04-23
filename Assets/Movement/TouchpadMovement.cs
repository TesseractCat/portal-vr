using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;


public class TouchpadMovement : MonoBehaviour {

    [Header("Movement Properties")]
    public float speed;

    [Header("Object References")]
    public Collider headCollider;
    public Transform lookCamera;
    public PostProcessVolume mainVolume;
    public InputActionReference movementAction;
    
    bool onGround = true;
    Vector3 currentDir;
    Vector3 lastPos = Vector3.zero;
    Transform lastPortal = null;
    
    Vector2 touchpadAxis = Vector2.zero;
    
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
        //Calculate if in portal
        bool inPortal = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("PortalColl"))
        {
            if (g.GetComponent<Collider>().bounds.Contains(lookCamera.position))
            {
                inPortal = true;
            }
        }

        //Handle elevation changes and falling
        Ray ray = new Ray(lookCamera.transform.position + Vector3.up/10.0f, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground", "Menu", "PortalTrigLayer", "LightBridge")))
        {
            if ((!onGround && inPortal) || hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer"))
            {
                //If layer==PortalTrigLayer -> It's a non-wall portal
                if (!(hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer")) || Mathf.Abs(hit.collider.transform.parent.forward.y) > 0.1f) {
                    //Player has stepped onto a portal, or fallen into one
                    onGround = false;

                    lastPortal = hit.collider.gameObject.transform.parent;
                    //TODO: Disable headCollider here or something
                    //headCollider.isTrigger = true;
                    
                    //Change rigidbody properties
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    GetComponent<Rigidbody>().drag = 0.3f;
                }
            } else if (hit.distance - Vector3.up.magnitude/2 < lookCamera.localPosition.y + 0.25//0.5
                    && (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("Menu") ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("LightBridge")))
            {
                //Higher elevation, for example steps or a ramp. Also could be slightly lower elevation, no need to start falling for this type of thing.
                onGround = true;
                
                //Lerp position
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), Time.deltaTime * 10);
                //headCollider.isTrigger = false;
                
                //Change rigidbody properties
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                GetComponent<Rigidbody>().drag = 10;
            } else {
                //Player should be falling here (should limit movement)
                onGround = false;
                
                //Change rigidbody properties
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                GetComponent<Rigidbody>().drag = 0.3f;
            }
        }

        lastPos = transform.position;
    }
}
