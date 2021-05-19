using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorLook : MonoBehaviour
{
    public float lookSensitivity;
    public float moveSensitivity;
    public float scrollSensitivity;
    
    float targetZoom;
    public Transform rotY;
    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        targetZoom = camera.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        
        float mouseX = mouse.delta.x.ReadValue();
        float mouseY = mouse.delta.y.ReadValue();
        
        if (mouse.middleButton.isPressed) {
            transform.Rotate(0, mouseX * lookSensitivity * Time.deltaTime, 0);
            rotY.RotateAround(transform.position, transform.right, -mouseY * lookSensitivity * Time.deltaTime);
        } else if (mouse.rightButton.isPressed) {
            transform.position = transform.position
                + transform.forward * Time.deltaTime * -mouseY * moveSensitivity
                + transform.right * Time.deltaTime * -mouseX * moveSensitivity;
        }
        
        if (!keyboard.shiftKey.isPressed) {
            targetZoom += mouse.scroll.ReadValue().y * scrollSensitivity;
        } else {
            transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + mouse.scroll.ReadValue().y * scrollSensitivity,
                    transform.position.z);
        }
        
        targetZoom = Mathf.Clamp(targetZoom, -100.0f, -2.5f);
        
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, new Vector3(0, 0, targetZoom), Time.deltaTime * 10.0f);
    }
}
