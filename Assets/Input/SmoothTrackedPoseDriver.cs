using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SmoothTrackedPoseDriver : MonoBehaviour
{
    public InputActionReference positionAction;
    public InputActionReference rotationAction;
    public float speed;
    
    void Update()
    {
        if (positionAction.action.phase != InputActionPhase.Canceled && positionAction.action.phase != InputActionPhase.Waiting)
            transform.localPosition = Vector3.Lerp(transform.localPosition, positionAction.action.ReadValue<Vector3>(), Time.deltaTime * speed);
        if (rotationAction.action.phase != InputActionPhase.Canceled && rotationAction.action.phase != InputActionPhase.Waiting)
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationAction.action.ReadValue<Quaternion>(), Time.deltaTime * speed);
    }
}
