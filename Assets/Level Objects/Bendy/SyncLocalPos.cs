using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncLocalPos : MonoBehaviour {

    public Transform syncObject;
    public Vector3 syncPos = Vector3.one;
    public Vector3 syncRot = Vector3.zero;

	void Update () {
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.Scale(syncObject.localPosition, syncPos);
            transform.localRotation = Quaternion.Euler(Vector3.Scale(syncObject.localRotation.eulerAngles, syncRot));
        }
	}
}
