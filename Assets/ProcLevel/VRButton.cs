using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButton : MonoBehaviour {
    public bool selected = false;
    public EditorMode buttonMode;
    public int buttonData;

    public void OnHover()
    {
        var buttons = FindObjectsOfType<VRButton>();
        foreach (VRButton button in buttons)
        {
            button.OnUnHover();
        }
        if (!selected)
        {
            this.GetComponent<Renderer>().sharedMaterial.color = new Color32(0xC7, 0xFB, 0xFF, 0xFF);
        }
    }

    public void OnUnHover()
    {
        if (!selected)
        {
            this.GetComponent<Renderer>().sharedMaterial.color = Color.white;
        } else
        {
            this.GetComponent<Renderer>().sharedMaterial.color = new Color32(0xFF, 0xE4, 0x69, 0xFF);
        }
    }

    public void OnSelect()
    {
        var buttons = FindObjectsOfType<VRButton>();
        foreach (VRButton button in buttons)
        {
            button.OnUnSelect();
        }
        selected = true;
        this.GetComponent<Renderer>().sharedMaterial.color = new Color32(0xFF, 0xE4, 0x69, 0xFF);
    }

    public void OnUnSelect()
    {
        selected = false;
        this.GetComponent<Renderer>().sharedMaterial.color = Color.white;
    }
}
