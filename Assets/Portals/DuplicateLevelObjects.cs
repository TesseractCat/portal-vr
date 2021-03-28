using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DuplicateLevelObjects : MonoBehaviour {

    Dictionary<int, GameObject> duplicatePortals;
    public Dictionary<GameObject, GameObject> duplicateLevelObjects;

    Transform duplicatePortalGun = null;

    public Transform levelObjectContainer;
    public GameObject characterBody;
    Transform defaultLevel;
    Vector3 lastPos;

    void Start()
    {
        duplicatePortals = new Dictionary<int, GameObject>();
        duplicateLevelObjects = new Dictionary<GameObject, GameObject>();
        defaultLevel = GameObject.Find("ProcLevel").transform;
        lastPos = transform.position;
    }

    void Update () {
        //Sync portals
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        for (int i = 0; i < portals.Length; i++)
        {
            if (!duplicatePortals.ContainsKey(portals[i].GetInstanceID()) && portals[i].name != GetComponentInParent<CorrespondingPortal>().name)
            {
                duplicatePortals[portals[i].GetInstanceID()] = (GameObject)Instantiate(portals[i].GetComponent<CorrespondingPortal>().fakeEquivalent,
                    getDuplicatePosFromOriginal(portals[i].transform), Quaternion.identity);

                duplicatePortals[portals[i].GetInstanceID()].transform.rotation = Quaternion.LookRotation(
                    getDuplicateDirectionFromOriginal(Vector3.forward, portals[i].transform), getDuplicateDirectionFromOriginal(Vector3.up, portals[i].transform));

                //Debug.Log(Vector3.Distance(Vector3.Scale(duplicatePortals[portals[i].GetInstanceID()].transform.forward, duplicatePortals[portals[i].GetInstanceID()].transform.position),
                //    Vector3.Scale(-GetComponentInParent<CorrespondingPortal>().transform.forward, GetComponentInParent<CorrespondingPortal>().transform.position)));

                if (Vector3.Distance(Vector3.Scale(duplicatePortals[portals[i].GetInstanceID()].transform.forward, duplicatePortals[portals[i].GetInstanceID()].transform.position),
                    Vector3.Scale(-GetComponentInParent<CorrespondingPortal>().transform.forward, GetComponentInParent<CorrespondingPortal>().transform.position)) <= 0.1f)
                {
                    Destroy(duplicatePortals[portals[i].GetInstanceID()]);
                }
            }
        }

        //Sync level objects
        List<Transform> levelObjectContainerObjects = new List<Transform>();
        for (int i = 0; i < levelObjectContainer.childCount; i++)
            levelObjectContainerObjects.Add(levelObjectContainer.GetChild(i));

        levelObjectContainerObjects.Add(characterBody.transform);
        for (int i = 0; i < levelObjectContainerObjects.Count; i++)
        {
            if (!duplicateLevelObjects.ContainsKey(levelObjectContainerObjects[i].gameObject))
            {
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject] = (GameObject)Instantiate(levelObjectContainerObjects[i],
                    getDuplicatePosFromOriginal(levelObjectContainerObjects[i].transform),
                    Quaternion.LookRotation(getDuplicateDirectionFromOriginal(Vector3.forward, levelObjectContainerObjects[i].transform), getDuplicateDirectionFromOriginal(Vector3.up, levelObjectContainerObjects[i].transform))).gameObject;

                if (duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].GetComponent<ChangeLayerOnCollIntersect>())
                    Destroy(duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].GetComponent<ChangeLayerOnCollIntersect>());
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].AddComponent<SetStencilMask>();
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].AddComponent<SetRenderQueue>().renderQueue = 2010;
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].GetComponent<SetStencilMask>().stencilMask = Int32.Parse(new String(GetComponentInParent<CorrespondingPortal>().correspondingPortal.name.Where(Char.IsDigit).ToArray()));
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].GetComponent<SetStencilMask>().changeStencilComp = true;
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].AddComponent<DuplicateObjectPortalIntersectHandler>();
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].layer = 16 - (Int32.Parse(new String(GetComponentInParent<CorrespondingPortal>().correspondingPortal.name.Where(Char.IsDigit).ToArray())) - 1);
                for (int j = 0; j < duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].transform.childCount; j++)
                {
                    duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].transform.GetChild(j).gameObject.layer = 16 - (Int32.Parse(new String(GetComponentInParent<CorrespondingPortal>().correspondingPortal.name.Where(Char.IsDigit).ToArray())) - 1);
                }
            } else
            {
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].transform.position = getDuplicatePosFromOriginal(levelObjectContainerObjects[i].transform);
                duplicateLevelObjects[levelObjectContainerObjects[i].gameObject].transform.rotation = Quaternion.LookRotation(getDuplicateDirectionFromOriginal(Vector3.forward, levelObjectContainerObjects[i].transform), getDuplicateDirectionFromOriginal(Vector3.up, levelObjectContainerObjects[i].transform));
            }
        }

        //Sync portal gun
        if (duplicatePortalGun == null)
        {
            duplicatePortalGun = ((GameObject)Instantiate(GameObject.Find("Portal Gun"))).transform;
            duplicatePortalGun.transform.position = getDuplicatePosFromOriginal(GameObject.Find("Portal Gun").transform);
            duplicatePortalGun.transform.rotation = Quaternion.LookRotation(
                getDuplicateDirectionFromOriginal(Vector3.forward, GameObject.Find("Portal Gun").transform),
                getDuplicateDirectionFromOriginal(Vector3.up, GameObject.Find("Portal Gun").transform));

            duplicatePortalGun.gameObject.AddComponent<SetStencilMask>();
            duplicatePortalGun.gameObject.AddComponent<SetRenderQueue>().renderQueue = 2010;
            duplicatePortalGun.gameObject.GetComponent<SetStencilMask>().stencilMask = Int32.Parse(new String(GetComponentInParent<CorrespondingPortal>().correspondingPortal.name.Where(Char.IsDigit).ToArray()));
            duplicatePortalGun.gameObject.GetComponent<SetStencilMask>().changeStencilComp = true;
        }
        else
        {
            duplicatePortalGun.transform.position = getDuplicatePosFromOriginal(GameObject.Find("Portal Gun").transform);
            duplicatePortalGun.transform.rotation = Quaternion.LookRotation(
                getDuplicateDirectionFromOriginal(Vector3.forward, GameObject.Find("Portal Gun").transform),
                getDuplicateDirectionFromOriginal(Vector3.up, GameObject.Find("Portal Gun").transform));
        }

        //Reset objects on portal movement
        if (transform.position != lastPos)
        {
            ResetObjects();
            lastPos = transform.position;
        }

    }

    void ResetObjects()
    {
        for (int i = 0; i < duplicatePortals.Count; i++)
        {
            Destroy(new List<GameObject>(duplicatePortals.Values)[i]);
        }
        /*for (int i = 0; i < duplicateLevelObjects.Count; i++)
        {
            Destroy(new List<GameObject>(duplicateLevelObjects.Values)[i]);
        }*/
        duplicatePortals = new Dictionary<int, GameObject>();
        //duplicateLevelObjects = new Dictionary<int, GameObject>();
        //Destroy(duplicatePortalGun.gameObject);
        //duplicatePortalGun = null;
    }

    Vector3 RoundVec3(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    Vector3 getDuplicatePosFromOriginal(Transform originalObject)
    {
        return transform.TransformPoint(defaultLevel.InverseTransformPoint(originalObject.position));
    }

    Vector3 getDuplicateDirectionFromOriginal(Vector3 direction, Transform originalObject)
    {
        return transform.TransformDirection(defaultLevel.transform.InverseTransformDirection(originalObject.TransformDirection(direction)));
    }
}