using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaximumVelocity : MonoBehaviour {

    public float MaxVel;
    float MaxVelSquared;
    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        MaxVelSquared = MaxVel * MaxVel;
    }

    void FixedUpdate () {
        if (rigidbody.velocity.sqrMagnitude > MaxVelSquared)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * MaxVel;
        }
    }
}
