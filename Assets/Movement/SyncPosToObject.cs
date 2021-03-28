using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPosToObject : MonoBehaviour {

    public Transform syncTo;
    public bool syncX;
    public bool syncY;
    public bool syncZ;

    void Update () {
        Vector3 newPos = transform.localPosition;
        if (syncX)
        {
            newPos.x = syncTo.localPosition.x;
        }
        if (syncY)
        {
            newPos.y = syncTo.localPosition.y;
        }
        if (syncZ)
        {
            newPos.z = syncTo.localPosition.z;
        }
        transform.localPosition = newPos;
	}
}
