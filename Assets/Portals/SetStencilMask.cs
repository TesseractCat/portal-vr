using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStencilMask : MonoBehaviour {

    public int stencilMask;
    public bool changeStencilComp = false;

    void Start()
    {
        Change();
    }

    public void Change()
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetInt("_StencilMask", stencilMask);
                if (changeStencilComp)
                {
                    materials[j].SetInt("_StencilComp", 3);
                }
                else
                {
                    materials[j].SetInt("_StencilComp", 8);
                }
            }
        }
    }
}
