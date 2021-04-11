using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCameraTeleport : MonoBehaviour {
    
    Transform lastSyncedPortal;

    void OnTriggerStay (Collider c) {

        if (c.gameObject.tag == "PortalColl" && c.transform.parent.InverseTransformPoint(this.transform.position).z > 0.01f)
        {
            //Do regular portal syncing, detailed in update function (but only once)
            /*Vector3 clonePos = c.transform.parent.InverseTransformPoint(this.transform.parent.position);
            clonePos.x = -clonePos.x;
            clonePos.z = -clonePos.z;
            this.transform.parent.SetParent(c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal);
            this.transform.parent.localPosition = clonePos;
            this.transform.parent.localRotation = Quaternion.Euler((Quaternion.Inverse(c.transform.parent.rotation) * this.transform.parent.rotation).eulerAngles + new Vector3(0, 180, 0));
            this.transform.parent.SetParent(null);*/
            
            Quaternion flipRot = Quaternion.AngleAxis(180.0f, c.transform.parent.up);
            transform.position = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal.TransformPoint(
                    flipRot * c.transform.parent.InverseTransformPoint(transform.position));
            
            transform.forward = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal.TransformDirection(
                    flipRot * c.transform.parent.InverseTransformDirection(transform.forward));
            
            lastSyncedPortal = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal;

            //Handle velocity
            Vector3 velRelToPortal = c.transform.parent.InverseTransformDirection(this.transform.parent.GetComponent<Rigidbody>().velocity);
            velRelToPortal = -velRelToPortal;
            velRelToPortal.y = -velRelToPortal.y;
            velRelToPortal = lastSyncedPortal.TransformDirection(velRelToPortal);

            Debug.Log("FINAL VEL " + velRelToPortal + " ... " + gameObject.name);

            this.transform.parent.GetComponent<Rigidbody>().velocity = velRelToPortal;
        }
    }

    void Update()
    {
        //Handle up
        if ((this.transform.parent.up - Vector3.up).magnitude > 0.01f)
        {
            //Make sure body is right side up
            this.transform.parent.RotateAround(transform.position,
                lastSyncedPortal.right,
                Vector3.SignedAngle(this.transform.parent.up, Vector3.up, lastSyncedPortal.right) * Time.smoothDeltaTime * 4);
        }
    }
}
