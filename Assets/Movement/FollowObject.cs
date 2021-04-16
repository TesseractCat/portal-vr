using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform toFollow;
    
    void Update()
    {
        transform.position = toFollow.position;
        transform.rotation = toFollow.rotation;
    }
}
