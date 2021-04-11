/*
	SetRenderQueue.cs
 
	Sets the RenderQueue of an object's materials on Awake. This will instance
	the materials, so the script won't interfere with other renderers that
	reference the same materials.
*/

using UnityEngine;
public class SetRenderQueue : MonoBehaviour
{

    public int renderQueue;

    void Start()
    {
        if (GetComponent<Renderer>() != null)
        {
            Material[] self_materials = GetComponent<Renderer>().materials;
            for (int i = 0; i < self_materials.Length; ++i)
            {
                self_materials[i].renderQueue = renderQueue;
            }
        }

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].renderQueue = renderQueue;
            }
        }
    }
}