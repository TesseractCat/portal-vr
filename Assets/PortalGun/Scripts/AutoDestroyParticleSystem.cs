using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour
{
    ParticleSystem ps;
    
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps != null) {
            if (!ps.IsAlive()) {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
