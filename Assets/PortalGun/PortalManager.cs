using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalManager : MonoBehaviour {

    public Transform trackedObject;

    public GameObject portalPrefab1;
    public GameObject portalPrefab2;

    public GameObject portal1 = null;
    public GameObject portal2 = null;

    public LaserPointer laser;

    public GameObject portalProjectile;
	
	void Update () {

        if (InputManager.PortalShot())
        {
            Debug.Log("Portal shot!");
            InputManager.DoHapticPulse(UnityEngine.XR.XRNode.RightHand);
            //Raycast to create portal
            RaycastHit hit;
            var layerMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //Hit detected, create the portal
                Debug.Log("Hit point");
                
                //Create portal projectile
                Instantiate(portalProjectile, transform.position, Quaternion.LookRotation(transform.forward));

                var wallDist = Vector3.zero;//hit.normal/18;
                GameObject tempPortal = null;
                
                if (InputManager.GetSelectedPortal() < 0)
                {
                    //Blue portal
                    if (portal1 == null)
                    {
                        tempPortal = Instantiate(portalPrefab1, hit.point + wallDist, Quaternion.LookRotation(-hit.normal));
                        GameObject.Destroy(portal1);
                        portal1 = tempPortal;

                        Transform temp_duplicate = (Transform)Instantiate(GameObject.FindObjectOfType<ProcLevelMesh>().transform);
                        Destroy(temp_duplicate.GetComponent<LevelEditor>());
                        Destroy(temp_duplicate.GetComponent<Collider>());
                        Destroy(temp_duplicate.GetComponent<ProcLevelMesh>());
                        temp_duplicate.SetParent(portal1.transform.Find("DuplicateLevel"));
                        temp_duplicate.GetComponent<Renderer>().material.SetInt("_StencilMask", 2);
                        temp_duplicate.GetComponent<Renderer>().material.SetInt("_StencilComp", 3);
                        temp_duplicate.GetComponent<Renderer>().material.renderQueue = 2010;
                        temp_duplicate.gameObject.layer = LayerMask.NameToLayer("Portal1LightMask");
                        Light[] temp_lights = temp_duplicate.gameObject.GetComponentsInChildren<Light>();
                        for (int i = 0; i < temp_lights.Length; i++)
                        {
                            temp_lights[i].cullingMask = LayerMask.GetMask("Portal1LightMask");
                        }
                        temp_duplicate.gameObject.SetActiveRecursively(false);
                    } else
                    {
                        portal1.transform.position = hit.point + wallDist;
                        portal1.transform.rotation = Quaternion.LookRotation(-hit.normal);
                        tempPortal = portal1;
                    }

                    //Only allow portal rotation on the ground
                    if (hit.normal == new Vector3(0, 1, 0) || hit.normal == new Vector3(0, -1, 0))
                    {
                        var tempRot = portal1.transform.localRotation.eulerAngles;
                        tempRot.y = trackedObject.gameObject.transform.rotation.eulerAngles.y;
                        portal1.transform.rotation = Quaternion.Euler(tempRot);
                    }
                } else if (InputManager.GetSelectedPortal() >= 0)
                {
                    //Orange portal
                    if (portal2 == null)
                    {
                        tempPortal = Instantiate(portalPrefab2, hit.point + wallDist, Quaternion.LookRotation(-hit.normal));
                        GameObject.Destroy(portal2);
                        portal2 = tempPortal;

                        Transform temp_duplicate = (Transform)Instantiate(GameObject.FindObjectOfType<ProcLevelMesh>().transform);
                        Destroy(temp_duplicate.GetComponent<LevelEditor>());
                        Destroy(temp_duplicate.GetComponent<Collider>());
                        Destroy(temp_duplicate.GetComponent<ProcLevelMesh>());
                        temp_duplicate.SetParent(portal2.transform.Find("DuplicateLevel"));
                        temp_duplicate.GetComponent<Renderer>().material.SetInt("_StencilMask", 1);
                        temp_duplicate.GetComponent<Renderer>().material.SetInt("_StencilComp", 3);
                        temp_duplicate.GetComponent<Renderer>().material.renderQueue = 2010;
                        temp_duplicate.gameObject.layer = LayerMask.NameToLayer("Portal2LightMask");
                        Light[] temp_lights = temp_duplicate.gameObject.GetComponentsInChildren<Light>();
                        for (int i = 0; i < temp_lights.Length; i++)
                        {
                            temp_lights[i].cullingMask = LayerMask.GetMask("Portal2LightMask");
                        }
                        temp_duplicate.gameObject.SetActiveRecursively(false);
                    } else
                    {
                        portal2.transform.position = hit.point + wallDist;
                        portal2.transform.rotation = Quaternion.LookRotation(-hit.normal);
                        tempPortal = portal2;
                    }

                    //Only allow portal rotation on the ground
                    if (hit.normal == new Vector3(0, 1, 0) || hit.normal == new Vector3(0, -1, 0))
                    {
                        var tempRot = portal2.transform.localRotation.eulerAngles;
                        tempRot.y = trackedObject.gameObject.transform.rotation.eulerAngles.y;
                        portal2.transform.rotation = Quaternion.Euler(tempRot);
                    }
                }

                LinkPortals();
                ScaleOnSpawn[] sosArr = tempPortal.GetComponentsInChildren<ScaleOnSpawn>(true);
                for (int i = 0; i < sosArr.Length; i++)
                {
                    //Make sure objects are scaled
                    sosArr[i].DoScale();
                }
            }
        } else
        {
            if (InputManager.GetSelectedPortal() != 0)
            {
                //Laser pointer
                laser.on = true;
            } else
            {
                laser.on = false;
            }
        }
	}

    void LinkPortals()
    {
        if (portal2 != null && portal1 != null)
        {
            /*var portal1_renderer = portal1.GetComponentInChildren<HTC.UnityPlugin.StereoRendering.StereoRenderer>();
            var portal2_renderer = portal2.GetComponentInChildren<HTC.UnityPlugin.StereoRendering.StereoRenderer>();

            portal1_renderer.anchorPos = portal2.GetComponentInChildren<SyncPortalObjects>().transform.position;//portal2.transform.position;
            portal1_renderer.anchorRot = Quaternion.LookRotation(-portal2.transform.forward, portal2.transform.up);
            portal2_renderer.anchorPos = portal1.GetComponentInChildren<SyncPortalObjects>().transform.position;//portal1.transform.position;
            portal2_renderer.anchorRot = Quaternion.LookRotation(-portal1.transform.forward, portal1.transform.up);*/

            portal1.GetComponentInChildren<CorrespondingPortal>().correspondingPortal = portal2.transform;
            //Activate and set clip plane
            portal1.transform.Find("DuplicateLevel").GetChild(0).gameObject.SetActiveRecursively(true);
            portal1.transform.Find("DuplicateLevel").GetChild(0).GetComponent<DuplicateLevelObjects>().enabled = true;
            portal1.transform.Find("DuplicateLevel").GetChild(0).GetComponent<Renderer>().material.SetVector("_PlanePos",
                new Vector4(portal2.transform.position.x, portal2.transform.position.y, portal2.transform.position.z, 1));
            portal1.transform.Find("DuplicateLevel").GetChild(0).GetComponent<Renderer>().material.SetVector("_PlaneDir",
                new Vector4(-portal2.transform.forward.x, -portal2.transform.forward.y, -portal2.transform.forward.z, 1));

            //Reset to default position for repositioning
            portal1.transform.Find("DuplicateLevel").transform.localPosition = Vector3.zero;
            portal1.transform.Find("DuplicateLevel").transform.localRotation = Quaternion.identity;
            portal1.transform.Find("DuplicateLevel").GetChild(0).transform.position = GameObject.FindObjectOfType<ProcLevelMesh>().transform.position;
            portal1.transform.Find("DuplicateLevel").GetChild(0).transform.rotation = GameObject.FindObjectOfType<ProcLevelMesh>().transform.rotation;

            //Reposition
            portal1.transform.Find("DuplicateLevel").transform.position = portal2.transform.position;
            portal1.transform.Find("DuplicateLevel").transform.rotation = Quaternion.LookRotation(-portal2.transform.forward, portal2.transform.up);

            //Handle level holes
            GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalPos1", portal1.transform.position);
            GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalScale1", new Vector3(0.5f, 1f, 0.1f));
            //GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalRot1", portal1.transform.rotation.eulerAngles);
            Vector4 rot = new Vector4(portal1.transform.forward.x, portal1.transform.forward.y, portal1.transform.forward.z,
                Vector3.SignedAngle(portal1.transform.up, portal1.transform.up, portal1.transform.forward));
            GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalRot1", rot);

            // -- Portal 2

            portal2.GetComponentInChildren<CorrespondingPortal>().correspondingPortal = portal1.transform;
            //Activate and set clip plane
            portal2.transform.Find("DuplicateLevel").GetChild(0).gameObject.SetActiveRecursively(true);
            portal2.transform.Find("DuplicateLevel").GetChild(0).GetComponent<DuplicateLevelObjects>().enabled = true;
            portal2.transform.Find("DuplicateLevel").GetChild(0).GetComponent<Renderer>().material.SetVector("_PlanePos",
                new Vector4(portal1.transform.position.x, portal1.transform.position.y, portal1.transform.position.z, 1));
            portal2.transform.Find("DuplicateLevel").GetChild(0).GetComponent<Renderer>().material.SetVector("_PlaneDir",
                new Vector4(-portal1.transform.forward.x, -portal1.transform.forward.y, -portal1.transform.forward.z, 1));

            //Reset to default position for repositioning
            portal2.transform.Find("DuplicateLevel").transform.localPosition = Vector3.zero;
            portal2.transform.Find("DuplicateLevel").transform.localRotation = Quaternion.identity;
            portal2.transform.Find("DuplicateLevel").GetChild(0).transform.position = GameObject.FindObjectOfType<ProcLevelMesh>().transform.position;
            portal2.transform.Find("DuplicateLevel").GetChild(0).transform.rotation = GameObject.FindObjectOfType<ProcLevelMesh>().transform.rotation;
            
            //Reposition
            portal2.transform.Find("DuplicateLevel").transform.position = portal1.transform.position;
            portal2.transform.Find("DuplicateLevel").transform.rotation = Quaternion.LookRotation(-portal1.transform.forward, portal1.transform.up);

            //Handle level holes
            GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalPos2", portal2.transform.position);
            GameObject.Find("ProcLevel").GetComponent<Renderer>().sharedMaterial.SetVector("_PortalScale2", Vector3.Scale(Vector3.one - (portal2.transform.forward * 0.99f), Vector3.one - (portal2.transform.right * 0.5f)));

            //Handle stencil mask render queues
            if (Vector3.Scale(portal2.transform.position, portal1.transform.forward).magnitude > Vector3.Scale(portal1.transform.position, portal1.transform.forward).magnitude)
            {
                portal2.transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1800;
                portal1.transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1900;
            } else
            {
                portal2.transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1900;
                portal1.transform.Find("StencilModel").GetComponent<Renderer>().material.renderQueue = 1800;
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
