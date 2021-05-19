using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fizzler : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Fizzleable>()) {
            c.GetComponent<Fizzleable>().Fizzle();
        }
    }
}
