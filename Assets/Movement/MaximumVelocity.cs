using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaximumVelocity : MonoBehaviour {

    public float MaxVel;
    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        float speed = Vector3.Magnitude(rigidbody.velocity);

        if (speed > MaxVel)

        {
            rigidbody.velocity *= 0.98f;
        }
    }
}
