using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//https://docs.google.com/spreadsheets/d/1XqSzYRHGAbzW9uRi_8t2Xm_zfBEJHCS2Bp3dkAYsOQc/edit#gid=0

public class InputManager : MonoBehaviour
{
    public static Vector2 GetMovementVec2()
    {
        return SquareToCircle(new Vector2(Input.GetAxis("wmr-movement-x"), Input.GetAxis("wmr-movement-y")));
    }

    static Vector2 SquareToCircle(Vector2 in_vec)
    {
        return new Vector2(in_vec.x * Mathf.Sqrt(1f - in_vec.y * (in_vec.y / 2)),
            in_vec.y * Mathf.Sqrt(1f - in_vec.x * (in_vec.x / 2)));
    }

    public static float GetSelectedPortal()
    {
        return Input.GetAxisRaw("wmr-portal-touchpad-x");
    }

    public static bool PortalShot()
    {
        return Input.GetButtonDown("wmr-portal-touchpad-click");
    }

    public static bool RightHandGrab()
    {
        return false;
    }

    public static bool LeftHandGrab()
    {
        return Input.GetButton("wmr-left-grab");
    }

    public static bool LeftTriggerClick()
    {
        return Input.GetButtonDown("wmr-left-trigger-click");
    }


    public static bool LeftTriggerDown()
    {
        return Input.GetButton("wmr-left-trigger-click");
    }

    public static void DoHapticPulse(XRNode node)
    {

        InputDevices.GetDeviceAtXRNode(node).SendHapticImpulse(0, 0.5f, 0.1f);
    }
}
