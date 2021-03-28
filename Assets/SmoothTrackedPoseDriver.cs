using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SmoothTrackedPoseDriver : MonoBehaviour
{
    public XRNode device;
    public float speed;

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, UnityEngine.XR.InputTracking.GetLocalPosition(device), Time.smoothDeltaTime * speed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, UnityEngine.XR.InputTracking.GetLocalRotation(device), Time.smoothDeltaTime * speed);
    }
}
