using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdateClipPlane : MonoBehaviour {
    
	void Update () {
        Renderer[] childRenderers = this.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < childRenderers.Length; i++)
        {
            //childRenderers[i].sharedMaterial.SetVector("_PlanePos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
            //childRenderers[i].sharedMaterial.SetVector("_PlaneDir", transform.forward);
        }
	}
}
