using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPortalObjects : MonoBehaviour {
    
    public Transform syncToPortal;

    Dictionary<GameObject, GameObject> portalClones;

    void Start()
    {
        portalClones = new Dictionary<GameObject, GameObject>();
    }
	
	void OnTriggerStay(Collider c)
    {
        if (c.gameObject.GetComponent<Camera>() != null)
        {
            //Camera being synced -- this is handled somewhere else
            //Debug.Log("It's a camera!");
            return;
        }

        if (c.gameObject.tag == "DontClone")
        {
            c.gameObject.layer = 9;
            return;
        }

        if (!c.gameObject.CompareTag("Clone") && !portalClones.ContainsKey(c.gameObject))
        {
            Debug.Log("Object entered to be synced!");

            //Other object
            var tempClone = (GameObject)Instantiate(c.gameObject, Vector3.zero, Quaternion.identity);

            tempClone.tag = "Clone";
            portalClones.Add(c.gameObject, tempClone);
            //Set parent
            tempClone.transform.SetParent(syncToPortal.GetComponentInChildren<SyncPortalObjects>().transform);
            
            //Set to layer 9 PortalEdgeLayer (so things can fall through portals)
            c.gameObject.layer = 9;

            Debug.Log(c.gameObject.name + " has been set to layer 9!");
        }
    }

    void Update()
    {
        foreach (KeyValuePair<GameObject,GameObject> pair in portalClones)
        {
            //Ensure on layer 9
            if (pair.Key.layer != 9)
            {
                pair.Key.layer = 9;
            }

            GameObject tempClone = portalClones[pair.Key];
            //Get local position in relation to portal
            Vector3 clonePos = this.transform.InverseTransformPoint(pair.Key.transform.position);
            //Reflect it (portals are mirrored)
            clonePos.x = -clonePos.x;
            clonePos.z = -clonePos.z;
            //Set clone position in local space
            tempClone.transform.localPosition = clonePos;
            //Set rotation
            tempClone.transform.localRotation = Quaternion.Euler((Quaternion.Inverse(this.transform.parent.rotation) * pair.Key.transform.rotation).eulerAngles + new Vector3(0, 180, 0));
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetComponent<Camera>() != null)
        {
            //Ignore
            return;
        }
        if (c.gameObject.tag == "DontClone")
        {
            c.gameObject.layer = 0;
            return;
        }

        if (!c.gameObject.CompareTag("Clone") && this.transform.parent.InverseTransformPoint(c.transform.position).z < 0)
        {
            //If an object has left the portal towards the outside, destroy the clone
            GameObject.Destroy(portalClones[c.gameObject]);
            portalClones.Remove(c.gameObject);
        }
        if (c.gameObject.CompareTag("Moveable") && this.transform.parent.InverseTransformPoint(c.transform.position).z > 0)
        {
            //If a moveable object is inside a portal (like a cube), remove the original and keep the clone.
            //Remove clone tag
            portalClones[c.gameObject].tag = "Moveable";
            portalClones[c.gameObject].name = portalClones[c.gameObject].name.Replace("(Clone)", "");
            //Change clone parent
            portalClones[c.gameObject].transform.parent = null;
            //Transfer velocity
            //Rotate velocity
            Quaternion difRot = syncToPortal.transform.rotation * Quaternion.Inverse(this.transform.parent.rotation);
            Vector3 tempVel = c.GetComponent<Rigidbody>().velocity;
            tempVel = -(difRot * tempVel);
            //Flip y in local space of other portal
            tempVel = syncToPortal.InverseTransformDirection(tempVel);
            tempVel.y = -tempVel.y;
            tempVel = syncToPortal.TransformDirection(tempVel);
            //Set its vel
            portalClones[c.gameObject].GetComponent<Rigidbody>().velocity = tempVel;
            //Remove layer 9
            portalClones[c.gameObject].gameObject.layer = 0;
            //Remove original
            portalClones.Remove(c.gameObject);
            GameObject.Destroy(c.gameObject);
        }
        //Remove layer 9 (so things can interact)
        c.gameObject.layer = 0;
    }
}
