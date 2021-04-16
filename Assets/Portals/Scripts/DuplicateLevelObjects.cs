using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DuplicateLevelObjects : MonoBehaviour {

    [Header("Duplicate Objects")]
    public int duplicateIndex = 0;
    GameObject duplicatePortal;
    Transform duplicatePortalGun = null;
    public Dictionary<GameObject, GameObject> duplicateLevelObjects;

    [Header("Object References")]
    public PortalManager portalManager;
    public Transform levelObjectContainer;
    public GameObject characterBody;
    
    Transform mapParent;

    void Awake()
    {
        duplicateLevelObjects = new Dictionary<GameObject, GameObject>();
        mapParent = GameObject.Find("ProcLevel").transform;
    }

    void Update () {
        //Sync level objects
        List<Transform> levelObjectContainerObjects = new List<Transform>();
        for (int i = 0; i < levelObjectContainer.childCount; i++)
            levelObjectContainerObjects.Add(levelObjectContainer.GetChild(i));

        //levelObjectContainerObjects.Add(characterBody.transform);
        //Iterate through level objects
        for (int i = 0; i < levelObjectContainerObjects.Count; i++)
        {
            GameObject toDuplicate = levelObjectContainerObjects[i].gameObject;
            
            if (!duplicateLevelObjects.ContainsKey(toDuplicate))
            {
                //Insantiate duplicate object
                duplicateLevelObjects[toDuplicate] = (GameObject)Instantiate(levelObjectContainerObjects[i],
                    GetDuplicatePosFromOriginal(levelObjectContainerObjects[i].transform),
                    Quaternion.LookRotation(GetDuplicateDirectionFromOriginal(Vector3.forward, levelObjectContainerObjects[i].transform), GetDuplicateDirectionFromOriginal(Vector3.up, levelObjectContainerObjects[i].transform))).gameObject;

                if (duplicateLevelObjects[toDuplicate].GetComponent<ChangeLayerOnCollIntersect>())
                    Destroy(duplicateLevelObjects[toDuplicate].GetComponent<ChangeLayerOnCollIntersect>());
                
                //Configure render queue and stencil
                duplicateLevelObjects[toDuplicate].AddComponent<SetRenderQueue>().renderQueue = 2010;
                
                duplicateLevelObjects[toDuplicate].AddComponent<SetStencilMask>();
                duplicateLevelObjects[toDuplicate].GetComponent<SetStencilMask>().stencilMask = duplicateIndex + 1;
                duplicateLevelObjects[toDuplicate].GetComponent<SetStencilMask>().changeStencilComp = true;
                
                //duplicateLevelObjects[toDuplicate].AddComponent<DuplicateObjectPortalIntersectHandler>();
                
                //Recursively set layers
                int layer = LayerMask.NameToLayer("Portal" + (duplicateIndex + 1).ToString() + "LightMask");
                duplicateLevelObjects[toDuplicate].layer = layer;
                for (int j = 0; j < duplicateLevelObjects[toDuplicate].transform.childCount; j++)
                {
                    duplicateLevelObjects[toDuplicate].transform.GetChild(j).gameObject.layer = layer;
                }
            } else
            {
                duplicateLevelObjects[toDuplicate].transform.position =
                    GetDuplicatePosFromOriginal(levelObjectContainerObjects[i].transform);
                duplicateLevelObjects[toDuplicate].transform.rotation =
                    Quaternion.LookRotation(GetDuplicateDirectionFromOriginal(Vector3.forward, levelObjectContainerObjects[i].transform), GetDuplicateDirectionFromOriginal(Vector3.up, levelObjectContainerObjects[i].transform));
            }
        }

        //Sync portal gun
        /*if (duplicatePortalGun == null)
        {
            duplicatePortalGun = ((GameObject)Instantiate(GameObject.Find("Portal Gun"))).transform;
            duplicatePortalGun.transform.position = GetDuplicatePosFromOriginal(GameObject.Find("Portal Gun").transform);
            duplicatePortalGun.transform.rotation = Quaternion.LookRotation(
                GetDuplicateDirectionFromOriginal(Vector3.forward, GameObject.Find("Portal Gun").transform),
                GetDuplicateDirectionFromOriginal(Vector3.up, GameObject.Find("Portal Gun").transform));

            duplicatePortalGun.gameObject.AddComponent<SetStencilMask>();
            duplicatePortalGun.gameObject.AddComponent<SetRenderQueue>().renderQueue = 2010;
            duplicatePortalGun.gameObject.GetComponent<SetStencilMask>().stencilMask = Int32.Parse(new String(GetComponentInParent<CorrespondingPortal>().correspondingPortal.name.Where(Char.IsDigit).ToArray()));
            duplicatePortalGun.gameObject.GetComponent<SetStencilMask>().changeStencilComp = true;
        }
        else
        {
            duplicatePortalGun.transform.position = GetDuplicatePosFromOriginal(GameObject.Find("Portal Gun").transform);
            duplicatePortalGun.transform.rotation = Quaternion.LookRotation(
                GetDuplicateDirectionFromOriginal(Vector3.forward, GameObject.Find("Portal Gun").transform),
                GetDuplicateDirectionFromOriginal(Vector3.up, GameObject.Find("Portal Gun").transform));
        }*/
    }

    public void PortalMoved(Transform portal)
    {
        if (portal == null || portal.GetComponent<CorrespondingPortal>().correspondingPortal == null)
            return;
        
        Destroy(duplicatePortal);
            
        //Sync portals
        portal = portal.GetComponent<CorrespondingPortal>().correspondingPortal;
        
        duplicatePortal = (GameObject)Instantiate(portal.GetComponent<CorrespondingPortal>().fakePrefab,
            GetDuplicatePosFromOriginal(portal.transform), Quaternion.identity);

        duplicatePortal.transform.rotation = Quaternion.LookRotation(
            GetDuplicateDirectionFromOriginal(Vector3.forward, portal.transform),
            GetDuplicateDirectionFromOriginal(Vector3.up, portal.transform));
    }

    Vector3 RoundVec3(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    Vector3 GetDuplicatePosFromOriginal(Transform originalObject)
    {
        return transform.TransformPoint(mapParent.InverseTransformPoint(originalObject.position));
    }

    Vector3 GetDuplicateDirectionFromOriginal(Vector3 direction, Transform originalObject)
    {
        return transform.TransformDirection(mapParent.transform.InverseTransformDirection(originalObject.TransformDirection(direction)));
    }
}
