using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    LineRenderer lineRenderer;
    public bool on;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        /*Ray ray = new Ray(transform.parent.position, transform.parent.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }*/
    }

    void LateUpdate()
    {
        if (on)
        {
            lineRenderer.enabled = true;
        } else
        {
            lineRenderer.enabled = false;
        }
    }
}
