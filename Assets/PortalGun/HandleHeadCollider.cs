using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleHeadCollider : MonoBehaviour {

    public Transform headCollider;

    /*void OnTriggerEnter(Collider c)
    {
        if (c.tag == "PortalColl")
        {
            headCollider.GetComponent<Collider>().isTrigger = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "PortalColl")
        {
            headCollider.GetComponent<Collider>().isTrigger = false;
        }
    }*/

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(headCollider.transform.position, GetComponent<Rigidbody>().velocity.normalized);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.tag == "PortalColl")
            {
                headCollider.GetComponent<Collider>().isTrigger = true;
            } else
            {
                bool inPortal = false;
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("PortalColl"))
                {
                    if (g.GetComponent<Collider>().bounds.Contains(headCollider.transform.position))
                    {
                        headCollider.GetComponent<Collider>().isTrigger = true;
                        inPortal = true;
                    }
                }
                if (!inPortal)
                {
                    headCollider.GetComponent<Collider>().isTrigger = false;
                }
            }
        } else
        {
            headCollider.GetComponent<Collider>().isTrigger = false;
        }
    }
}
