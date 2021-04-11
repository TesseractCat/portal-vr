using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayerOnCollIntersect : MonoBehaviour {

    public string ColliderTag;

    int defaultLayer;
    Dictionary<GameObject, int> defaultRenderQueues;
    public int newRenderQueue;
    public int inColliderLayer;

    void Start()
    {
        defaultLayer = gameObject.layer;
        defaultRenderQueues = new Dictionary<GameObject, int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Renderer>())
            {
                defaultRenderQueues.Add(transform.GetChild(i).gameObject, transform.GetChild(i).GetComponent<Renderer>().material.renderQueue);
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == ColliderTag)
        {
            gameObject.layer = inColliderLayer;
            new List<GameObject>(defaultRenderQueues.Keys).ForEach(childObject =>
            {
                childObject.GetComponent<Renderer>().material.renderQueue = newRenderQueue;
            });
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == ColliderTag)
        {
            gameObject.layer = defaultLayer;
            new List<GameObject>(defaultRenderQueues.Keys).ForEach(childObject =>
            {
                childObject.GetComponent<Renderer>().material.renderQueue = defaultRenderQueues[childObject];
            });
        }
    }

}
