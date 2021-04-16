using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendyLook : MonoBehaviour
{
    void Update()
    {
        transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, 0, 0);
    }
}
