using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public int clampMin;
    public int clampMax;
    
    public float baseSensitivity;
    public Transform body;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    float ClampAngle(float angle, float from_angle, float to_angle)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360+from_angle);
        return Mathf.Min(angle, to_angle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.visible) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(-mouseY * baseSensitivity, 0, 0);
        body.Rotate(0, mouseX * baseSensitivity, 0);
        
        transform.localEulerAngles = new Vector3(
                ClampAngle(transform.localEulerAngles.x, clampMin, clampMax),
                0,
                0);
    }
}
