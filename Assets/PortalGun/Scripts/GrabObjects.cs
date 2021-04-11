using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrabObjects : MonoBehaviour {

    public Transform objectLerpPos;

    public GameObject grabbedObject;

    public float grabSpeed = 5f;

    void Update()
    {
        if (InputManager.RightHandGrab())
        {
            if (grabbedObject != null)
            {
                grabbedObject = null;
            }
            else
            {
                Debug.Log("Grabbing object!");
                InputManager.DoHapticPulse(UnityEngine.XR.XRNode.RightHand);

                //Raycast to grab object
                RaycastHit hit;
                var layerMask = LayerMask.GetMask("Default", "PortalEdgeLayer");
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
                {
                    //Can grab object
                    if (hit.collider.gameObject.CompareTag("Moveable"))
                    {
                        grabbedObject = hit.collider.gameObject;
                    }
                }
            }
        }

        if (grabbedObject != null)
        {
            //grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, objectLerpPos.position, 0.1f * Time.deltaTime * grabSpeed);
            grabbedObject.GetComponent<Rigidbody>().velocity = (objectLerpPos.position - grabbedObject.transform.position) * grabSpeed;
        }
    }

}
