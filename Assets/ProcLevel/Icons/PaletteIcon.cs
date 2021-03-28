using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteIcon : MonoBehaviour {

    public Vector2 iconPos;
    public GameObject placeablePrefab;

	void Start () {
        Material newMat = new Material(this.GetComponent<Renderer>().sharedMaterial);
        newMat.mainTextureOffset = new Vector2((1.0f / 4.0f) * iconPos.x, -(1.0f / 8.0f) * (iconPos.y + 1));
        this.GetComponent<Renderer>().sharedMaterial = newMat;
	}
}