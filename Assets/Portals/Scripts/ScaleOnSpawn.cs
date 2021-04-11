using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnSpawn : MonoBehaviour {

    float speed = 50;
    Vector3 lastPos = Vector3.zero;
    Vector3 defaultScale;
    public Vector3 restrictAxis = Vector3.zero;

	void Awake ()
    {
        defaultScale = transform.localScale;
        transform.localScale = Vector3.Scale(transform.localScale, restrictAxis);
	}

	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, 0.1f * Time.deltaTime * speed);

        if (transform.position != lastPos)
        {
            lastPos = transform.position;
            DoScale();
        }
	}

    public void DoScale()
    {
        transform.localScale = Vector3.Scale(transform.localScale, restrictAxis);
    }
}
