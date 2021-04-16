using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCameraTeleport : MonoBehaviour {
    
    Transform lastSyncedPortal;
    GameObject lastUsedPortal;

    void OnTriggerStay (Collider c)
    {
        if (c.gameObject.tag == "PortalColl" && c.transform.parent.InverseTransformPoint(transform.position).z > 0.01f && c.gameObject != lastUsedPortal)
        {
            Quaternion flipRot = Quaternion.AngleAxis(180.0f, Vector3.up);
            
            lastUsedPortal = c.gameObject;
            StartCoroutine(ResetLastUsedPortal());
            lastSyncedPortal = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal;

            //Handle position
            transform.parent.position = lastSyncedPortal.TransformPoint(
                    flipRot * c.transform.parent.InverseTransformPoint(transform.parent.position));
            
            transform.parent.rotation = lastSyncedPortal.rotation * (flipRot * (Quaternion.Inverse(c.transform.parent.rotation) * transform.parent.rotation));
            
            //Handle velocity
            transform.parent.GetComponent<Rigidbody>().velocity = lastSyncedPortal.TransformDirection(
                    flipRot * c.transform.parent.InverseTransformDirection(transform.parent.GetComponent<Rigidbody>().velocity));
        }
    }
    
    IEnumerator ResetLastUsedPortal() {
        yield return new WaitForSeconds(0.5f);
        lastUsedPortal = null;
    }

    void FixedUpdate()
    {
        //Handle up
        if ((this.transform.parent.up - Vector3.up).magnitude > 0.01f)
        {
            //Make sure body is right side up
            this.transform.parent.RotateAround(transform.position,
                lastSyncedPortal.right,
                //Vector3.Cross(transform.parent.up, Vector3.up),
                Vector3.SignedAngle(this.transform.parent.up, Vector3.up, lastSyncedPortal.right) * Time.fixedDeltaTime * 4);
        }
    }
}
