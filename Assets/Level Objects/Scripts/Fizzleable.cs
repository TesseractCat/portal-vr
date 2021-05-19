using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fizzleable : MonoBehaviour
{
    public void Fizzle() {
        StartCoroutine(FizzleCoroutine());
    }
    
    IEnumerator FizzleCoroutine() {
        if (GetComponent<Rigidbody>()) {
            foreach (Collider c in GetComponentsInChildren<Collider>()) {
                Destroy(c);
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity/3.0f + Random.onUnitSphere * 0.1f;
            GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity/3.0f + Random.onUnitSphere * 0.1f;
        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
            foreach (Material m in r.materials) {
                m.SetFloat("_FizzleTime", Time.timeSinceLevelLoad);
            }
        }
        yield return new WaitForSeconds(1.1f);
        GameObject.Destroy(this.gameObject);
    }
}
