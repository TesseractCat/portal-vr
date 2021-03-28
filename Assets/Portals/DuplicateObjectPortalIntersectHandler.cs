using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DuplicateObjectPortalIntersectHandler : MonoBehaviour
{

    public string ColliderTag = "PortalColl";

    int defaultStencilMask;

    void Start()
    {
        defaultStencilMask = GetComponent<SetStencilMask>().stencilMask;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == ColliderTag)
        {
            GetComponent<SetStencilMask>().changeStencilComp = false;
            GetComponent<SetStencilMask>().stencilMask = 0;
            GetComponent<SetStencilMask>().Change();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == ColliderTag)
        {
            GetComponent<SetStencilMask>().changeStencilComp = true;
            GetComponent<SetStencilMask>().stencilMask = defaultStencilMask;
            GetComponent<SetStencilMask>().Change();

            //If in front of portal, swap objects
            if (Vector3.Scale(transform.position, c.transform.forward).magnitude < Vector3.Scale(c.transform.position, c.transform.forward).magnitude)
            {
                Debug.Log("SWAPPING OBJECTS");
                GameObject realObj = c.transform.parent.GetComponent<CorrespondingPortal>().correspondingPortal.GetComponentInChildren<DuplicateLevelObjects>().duplicateLevelObjects.FirstOrDefault(x => x.Value == transform.gameObject).Key;
                var realPos = realObj.transform.position;
                var realRot = realObj.transform.rotation;
                Vector3 newVel = c.transform.parent.GetComponent<CorrespondingPortal>().correspondingPortal.InverseTransformDirection(realObj.GetComponent<Rigidbody>().velocity);
                newVel = -newVel;
                newVel.y = -newVel.y;
                newVel = c.transform.parent.TransformDirection(newVel);
                Vector3 newAngularVel = c.transform.parent.GetComponent<CorrespondingPortal>().correspondingPortal.InverseTransformVector(realObj.GetComponent<Rigidbody>().angularVelocity);
                newAngularVel = -newAngularVel;
                newAngularVel.y = -newAngularVel.y;
                newAngularVel = c.transform.parent.TransformVector(newAngularVel);
                var clonePos = transform.position;
                var cloneRot = transform.rotation;
                transform.position = realPos;
                transform.rotation = realRot;
                realObj.transform.position = clonePos;
                realObj.transform.rotation = cloneRot;
                realObj.GetComponent<Rigidbody>().velocity = newVel;
                realObj.GetComponent<Rigidbody>().angularVelocity = newAngularVel;
            }
        }
    }
}
