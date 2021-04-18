using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    public List<Texture2D> symbols;
    public Renderer renderer;
    
    public void Assign(int i) {
        if (i == -1) {
            foreach (Renderer r in transform.GetComponentsInChildren<Renderer>()) {
                r.enabled = false;
            }
        } else {
            foreach (Renderer r in transform.GetComponentsInChildren<Renderer>()) {
                r.enabled = true;
            }
            renderer.material.SetTexture("_MainTex", symbols[i]);
        }
    }
}
