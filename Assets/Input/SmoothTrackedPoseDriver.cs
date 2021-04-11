using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SmoothTrackedPoseDriver : MonoBehaviour
{
    public XRNode device;
    //InputDevice inputDevice;
    public float speed;
    
    //void Start() {
    //    inputDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(device);
    //}
    
    public void OnPosition(InputAction.CallbackContext context) {
        //transform.localPosition = Vector3.Lerp(transform.localPosition, context.ReadValue<Vector3>(), Time.smoothDeltaTime * speed);
        transform.localPosition = context.ReadValue<Vector3>();
    }
    public void OnRotation(InputAction.CallbackContext context) {
        //transform.localRotation = Quaternion.Lerp(transform.localRotation, context.ReadValue<Quaternion>(), Time.smoothDeltaTime * speed);
        transform.localRotation = context.ReadValue<Quaternion>();
    }

    void Update()
    {
        //transform.localPosition = Vector3.Lerp(transform.localPosition, UnityEngine.XR.InputTracking.GetLocalPosition(device), Time.smoothDeltaTime * speed);
        //transform.localRotation = Quaternion.Lerp(transform.localRotation, UnityEngine.XR.InputTracking.GetLocalRotation(device), Time.smoothDeltaTime * speed);
        
        //Vector3 trackedPosition;
        //if (inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out trackedPosition)) {
        //    transform.localPosition = Vector3.Lerp(transform.localPosition, trackedPosition, Time.smoothDeltaTime * speed);
        //}
        //Quaternion trackedRotation;
        //if (inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out trackedRotation)) {
        //    transform.localRotation = Quaternion.Lerp(transform.localRotation, trackedRotation, Time.smoothDeltaTime * speed);
        //}
    }
}
