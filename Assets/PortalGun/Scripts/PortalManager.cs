using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour {

    public Transform mapParent;

    public GameObject portalProjectile;

    public GameObject[] portalPrefabs = new GameObject[2];
    
    public GameObject[] portals = new GameObject[2];
    
    GameObject[] levelDuplicates = new GameObject[2];
    
    void Start() {
        //Handle level holes
        foreach (Material m in mapParent.GetComponent<Renderer>().sharedMaterials) {
            for (int i = 1; i <= 2; i++) {
                m.SetVector("_PortalPos" + (i).ToString(), new Vector3(1000, 1000, 1000));
                m.SetVector("_PortalRot" + (i).ToString(), Vector4.zero);
            }
        }
    }
    
    public void ShootPortal(Vector3 position, Vector3 direction, int portal, float rotY) {
        Debug.Log("Portal shot!");
        Debug.Assert(portal == 0 || portal == 1);
        
        InputManager.DoHapticPulse(UnityEngine.XR.XRNode.RightHand);
        
        //Raycast to create portal
        RaycastHit hit;
        var layerMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(position, direction, out hit, Mathf.Infinity, layerMask))
        {
            //Hit detected, create the portal
            Debug.Log("Hit point");
            
            //Create portal projectile
            //Instantiate(portalProjectile, transform.position, Quaternion.LookRotation(transform.forward));

            var wallDist = Vector3.zero;
            GameObject tempPortal = null;
            
            //Create/reposition portal and handle level duplication
            if (portals[portal] == null) { //Portal hasn't been created yet
                tempPortal = Instantiate(portalPrefabs[portal], hit.point + wallDist, Quaternion.LookRotation(-hit.normal));
                portals[portal] = tempPortal;
                
                //Create duplicate of the level
                GameObject tempDuplicateParent = new GameObject("Duplicate" + (portal + 1).ToString());
                tempDuplicateParent.transform.parent = transform;
                tempDuplicateParent.transform.position = portals[portal].transform.position;
                levelDuplicates[portal] = tempDuplicateParent;
                
                Transform tempDuplicate = (Transform)Instantiate(GameObject.FindObjectOfType<ProcLevelMesh>().transform);
                //Remove unnessecary components
                Destroy(tempDuplicate.GetComponent<LevelEditor>());
                Destroy(tempDuplicate.GetComponent<Collider>());
                Destroy(tempDuplicate.GetComponent<ProcLevelMesh>());
                
                //Set up shader stenciling
                tempDuplicate.parent = tempDuplicateParent.transform;
                tempDuplicate.GetComponent<Renderer>().material.SetInt("_StencilMask", portal + 1);
                tempDuplicate.GetComponent<Renderer>().material.SetInt("_StencilComp", 3);
                tempDuplicate.GetComponent<Renderer>().material.renderQueue = 2010;
                
                //Set up lighting layers
                tempDuplicate.gameObject.layer = LayerMask.NameToLayer("Portal" + (portal + 1).ToString() + "LightMask");
                
                Light[] tempLights = tempDuplicate.gameObject.GetComponentsInChildren<Light>();
                foreach (Light l in tempLights)
                {
                    l.cullingMask = LayerMask.GetMask("Portal" + (portal + 1).ToString() + "LightMask");
                }
                
                //Deactivate duplicate
                tempDuplicateParent.SetActive(false);
            } else { //Portal already exists
                portals[portal].transform.position = hit.point + wallDist;
                portals[portal].transform.rotation = Quaternion.LookRotation(-hit.normal);
                tempPortal = portals[portal];
            }
                
            //Handle level holes
            foreach (Material m in mapParent.GetComponent<Renderer>().sharedMaterials) {
                m.SetVector("_PortalPos" + (portal + 1).ToString(), portals[portal].transform.position);
                Vector4 rot = new Vector4(Mathf.Deg2Rad*(portals[portal].transform.localRotation.eulerAngles.x),
                        Mathf.Deg2Rad*(-portals[portal].transform.rotation.eulerAngles.y), 0, 0);
                m.SetVector("_PortalRot" + (portal + 1).ToString(), rot);
                m.SetFloat("_PortalTime" + (portal + 1).ToString(), Time.timeSinceLevelLoad);
            }
            
            //Only allow portal rotation on the ground
            if (hit.normal == new Vector3(0, 1, 0) || hit.normal == new Vector3(0, -1, 0))
            {
                Vector3 tempRot = portals[portal].transform.localRotation.eulerAngles;
                tempRot.y = rotY;
                portals[portal].transform.rotation = Quaternion.Euler(tempRot);
            }

            //Connect portals if both exist
            LinkPortals();
            ScaleOnSpawn[] sosArr = tempPortal.GetComponentsInChildren<ScaleOnSpawn>(true);
            for (int i = 0; i < sosArr.Length; i++)
            {
                //Make sure objects are scaled
                sosArr[i].DoScale();
            }
            
            //Update DuplicateLevelObjects
            for (int i = 0; i <= 1; i++) {
                if (levelDuplicates[i] != null)
                    levelDuplicates[i].GetComponentInChildren<DuplicateLevelObjects>().PortalMoved(portals[i].transform);
            }
        }
    }

    void LinkPortals()
    {
        if (portals[0] != null && portals[1] != null)
        {
            List<(int, int)> portalPairs = new List<(int, int)>() {
                (0,1),(1,0)
            };
            
            foreach ((int,int) pair in portalPairs) {
                portals[pair.Item1].GetComponentInChildren<CorrespondingPortal>().correspondingPortal = portals[pair.Item2].transform;
                
                //Activate and set clip plane
                levelDuplicates[pair.Item1].SetActive(true);
                levelDuplicates[pair.Item1].GetComponentInChildren<DuplicateLevelObjects>().duplicateIndex = pair.Item1;
                levelDuplicates[pair.Item1].GetComponentInChildren<DuplicateLevelObjects>().enabled = true;
                
                foreach (Material m in levelDuplicates[pair.Item1].transform.GetChild(0).GetComponent<Renderer>().materials) {
                    m.SetVector("_PlanePos",
                        new Vector4(portals[pair.Item2].transform.position.x, portals[pair.Item2].transform.position.y, portals[pair.Item2].transform.position.z, 1));
                    m.SetVector("_PlaneDir",
                        new Vector4(-portals[pair.Item2].transform.forward.x, -portals[pair.Item2].transform.forward.y, -portals[pair.Item2].transform.forward.z, 1));
                }

                //Reposition
                levelDuplicates[pair.Item1].transform.position = portals[pair.Item1].transform.position;
                levelDuplicates[pair.Item1].transform.rotation =
                    Quaternion.LookRotation(portals[pair.Item1].transform.forward, portals[pair.Item1].transform.up);
                
                levelDuplicates[pair.Item1].transform.GetChild(0).position = mapParent.position;
                levelDuplicates[pair.Item1].transform.GetChild(0).rotation = mapParent.rotation;
                
                levelDuplicates[pair.Item1].transform.position = portals[pair.Item2].transform.position;
                levelDuplicates[pair.Item1].transform.rotation =
                    Quaternion.LookRotation(-portals[pair.Item2].transform.forward, portals[pair.Item2].transform.up);
            }
            //Handle stencil mask render queues
            if (Vector3.Scale(portals[1].transform.position, portals[0].transform.forward).magnitude >
                    Vector3.Scale(portals[0].transform.position, portals[0].transform.forward).magnitude)
            {
                portals[1].transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1800;
                portals[0].transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1900;
            } else
            {
                portals[1].transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1900;
                portals[0].transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1800;
            }
        }
    }

    Vector3 Vector3Abs(Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    Vector3 Vector3Deg2Rad(Vector3 vector)
    {
        return new Vector3(Mathf.Deg2Rad * (vector.x), Mathf.Deg2Rad * (vector.y), Mathf.Deg2Rad * (vector.z));
    }
}
