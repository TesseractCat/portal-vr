using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandButton : MonoBehaviour
{
    public Vector3 offset;
    Vector3 originalPos;
    Vector3 targetPos;

    Material gloveMat = null;
    float targetOutlineWidth = 0;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Glove")
        {
            //c.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0.007f);
            gloveMat = c.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            targetOutlineWidth = 0.007f;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Glove")
        {
            //c.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0f);
            targetOutlineWidth = 0f;
        }
    }

    void Update()
    {
        if (InputManager.LeftTriggerDown())
        {
            targetPos = originalPos + offset;
        } else
        {
            targetPos = originalPos;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 5);
        if (gloveMat != null)
        {
            gloveMat.SetFloat("_OutlineWidth", Mathf.Lerp(gloveMat.GetFloat("_OutlineWidth"), targetOutlineWidth, Time.deltaTime * 5));
        }
    }
}
