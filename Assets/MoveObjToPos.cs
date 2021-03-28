using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjToPos : MonoBehaviour {

    public Transform target;

	void Awake()
    {
        transform.position = target.position;
    }
	
	void Update () {
        transform.position = target.position;
    }
}
