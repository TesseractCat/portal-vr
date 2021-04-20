using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleHeadCollider : MonoBehaviour {

    public Transform headCollider;
    
    IEnumerator enableCoroutine;
    
    Vector3 lastPosition;
    Vector3 lastVelocity;
    float lastFixedUpdateTime;
    
    List<Collider> ignoring;
    void Start() {
        ignoring = new List<Collider>();
    }
    
    void OnCollisionEnter(Collision c) {
        //Debug.Log("Collided! " + Time.time);
    }
    
    /*void OnCollisionEnter(Collision c)
    {
        //If we are in a portal, set to trigger and recalculate position
        //This is for redundancy, both of these systems sometimes don't disable the collider for whatever reason...
        bool inPortal = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("PortalColl"))
        {
            if (g.GetComponent<Collider>().bounds.Contains(headCollider.transform.position))
            {
                inPortal = true;
            }
        }
        if (inPortal) {
            if (enableCoroutine != null)
                StopCoroutine(enableCoroutine);
            if (!ignoring.Contains(c.collider)) {
                Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), c.collider, true);
                ignoring.Add(c.collider);
                
                GetComponent<Rigidbody>().velocity = lastVelocity;
                transform.position = lastPosition;
                GetComponent<Rigidbody>().MovePosition(transform.position + lastVelocity * (Time.time - lastFixedUpdateTime));
            }
        }
    }*/
    void FixedUpdate() {
        lastPosition = transform.position;
        lastVelocity = GetComponent<Rigidbody>().velocity;
        lastFixedUpdateTime = Time.time;
        
        //Check if in portal
        bool inPortal = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("PortalColl"))
        {
            if (g.GetComponent<Collider>().bounds.Contains(headCollider.transform.position))
            {
                inPortal = true;
            }
        }
        
        //Predicts ahead to see if we are about to hit a portal
        RaycastHit hit;
        Ray ray = new Ray(headCollider.transform.position, GetComponent<Rigidbody>().velocity.normalized);
        
        if (inPortal || Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("PortalTrigLayer")))
        {
            if (enableCoroutine != null)
                StopCoroutine(enableCoroutine);
            if (!ignoring.Contains(GameObject.Find("Level").GetComponent<Collider>())) {
                //Debug.Log("Ray-ignoring! " + Time.time);
                Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), GameObject.Find("Level").GetComponent<Collider>(), true);
                ignoring.Add(GameObject.Find("Level").GetComponent<Collider>());
            }
        } else {
            if (ignoring.Count != 0) {
                foreach (Collider c in ignoring) {
                    Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), c, false);
                }
                ignoring.Clear();
            }
        }
    }
    
    /*void OnTriggerEnter(Collider c) {
        if (c.tag == "PortalColl")
        {
            //Debug.Log("Triggered! " + Time.time);
            if (enableCoroutine != null)
                StopCoroutine(enableCoroutine);
            if (!ignoring.Contains(GameObject.Find("Level").GetComponent<Collider>())) {
                Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), GameObject.Find("Level").GetComponent<Collider>(), true);
                ignoring.Add(GameObject.Find("Level").GetComponent<Collider>());
            }
        }
    }*/
    void OnTriggerStay(Collider c) {
        if (c.tag == "PortalColl")
        {
            if (enableCoroutine != null)
                StopCoroutine(enableCoroutine);
            if (!ignoring.Contains(GameObject.Find("Level").GetComponent<Collider>())) {
                Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), GameObject.Find("Level").GetComponent<Collider>(), true);
                ignoring.Add(GameObject.Find("Level").GetComponent<Collider>());
            }
        }
    }
    
    void OnTriggerExit(Collider c)
    {
        if (c.tag == "PortalColl")
        {
            enableCoroutine = EnableHeadCollider();
            StartCoroutine(enableCoroutine);
        }
    }
    
    IEnumerator EnableHeadCollider() {
        yield return new WaitForSeconds(0.5f);
        
        //Don't enable when in a portal
        bool inPortal = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("PortalColl"))
        {
            if (g.GetComponent<Collider>().bounds.Contains(headCollider.transform.position))
            {
                inPortal = true;
            }
        }
        if (!inPortal) {
            foreach (Collider c in ignoring) {
                Physics.IgnoreCollision(headCollider.GetComponent<Collider>(), c, false);
            }
            ignoring.Clear();
        }
    }

    /*
    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "PortalColl")
        {
            if (enableCoroutine != null)
                StopCoroutine(enableCoroutine);
            headCollider.GetComponent<Collider>().isTrigger = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "PortalColl")
        {
            enableCoroutine = EnableHeadCollider();
            StartCoroutine(enableCoroutine);
        }
    }
    
    IEnumerator EnableHeadCollider() {
        yield return new WaitForSeconds(0.1f);
        headCollider.GetComponent<Collider>().isTrigger = false;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(headCollider.transform.position, GetComponent<Rigidbody>().velocity.normalized);
        
        if (Physics.Raycast(ray, out hit, 1.5f, LayerMask.GetMask("Ground", "PortalTrigLayer")))
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
    }*/
}
