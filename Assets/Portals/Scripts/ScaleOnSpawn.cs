using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnSpawn : MonoBehaviour {

    float speed = 50;
    Vector3 lastPos = Vector3.zero;
    Vector3 defaultScale;
    public Vector3 restrictAxis = Vector3.zero;
    float scaleTime = 0;

	void Awake ()
    {
        defaultScale = transform.localScale;
        transform.localScale = Vector3.Scale(transform.localScale, restrictAxis);
	}

	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, SmoothStep(0.0f, 2.0f, (Time.time - scaleTime)));

        if (transform.position != lastPos)
        {
            lastPos = transform.position;
            DoScale();
        }
	}

    public void DoScale()
    {
        transform.localScale = Vector3.Scale(transform.localScale, restrictAxis);
        scaleTime = Time.time;
    }
    
    float SmoothStep(float edge0, float edge1, float x) {
        float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
        return t * t * (3.0f - 2.0f * t);
    }
}
