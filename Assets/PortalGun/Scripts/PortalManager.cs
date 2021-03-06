using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalManager : MonoBehaviour {

    public Transform mapParent;

    public GameObject[] portalProjectiles;

    public GameObject[] portalPrefabs = new GameObject[2];
    
    public GameObject[] portals = new GameObject[2];
    
    [System.NonSerialized]
    public UnityEvent portalsLinkedEvent = new UnityEvent();
    
    [System.NonSerialized]
    public GameObject[] levelDuplicates = new GameObject[2];
    
    public void ShootPortal(Vector3 position, Vector3 direction, int portal, float rotY) {
        Debug.Assert(portal == 0 || portal == 1);
        
        InputManager.DoHapticPulse(UnityEngine.XR.XRNode.RightHand);
        
        //Raycast to create portal
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Ground", "PortalTrigLayer", "StopPortal");
        if (Physics.Raycast(position, direction, out hit, Mathf.Infinity, layerMask))
        {
            //Prevent shooting a portal on another portal
            if ((hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer")
                    && hit.collider.transform.parent.gameObject != portals[portal])
                    || hit.collider.gameObject.layer == LayerMask.NameToLayer("StopPortal")) {
                //Could shoot a portal through a portal here
                return;
            } else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer") &&
                    hit.collider.transform.parent.gameObject == portals[portal]) {
                layerMask = LayerMask.GetMask("Ground");
                if (!Physics.Raycast(position, direction, out hit, Mathf.Infinity, layerMask)) {
                    return;
                }
            }
            
            //Check if unportalable
            if (hit.triangleIndex >= hit.collider.GetComponent<MeshFilter>().mesh.GetSubMesh(1).indexStart/3)
                return;
            
            //Should be a good surface
            
            //Create portal projectile
            Instantiate(portalProjectiles[portal], position, Quaternion.LookRotation(direction));

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
                foreach (Collider c in tempDuplicate.GetComponentsInChildren<Collider>()) {
                    Destroy(c);
                }
                Destroy(tempDuplicate.GetComponent<ProcLevelMesh>());
                
                tempDuplicate.parent = tempDuplicateParent.transform;
                foreach (Renderer r in tempDuplicate.GetComponentsInChildren<Renderer>()) {
                    foreach (Material m in r.materials) {
                        //Set up shader stenciling
                        m.SetInt("_StencilMask", portal + 1);
                        m.SetInt("_StencilComp", 3);
                        //m.renderQueue = 2010;
                        m.renderQueue = m.renderQueue - 40;
                    }
                    
                    //Set up lighting layers
                    r.gameObject.layer = LayerMask.NameToLayer("Portal" + (portal + 1).ToString() + "LightMask");
                }
                
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
            
            //Fix portal placement
            FixOverhangs(portals[portal].transform);
            FixIntersections(portals[portal].transform);
            FixUnportalable(portals[portal].transform);
            
            //Only allow portal rotation on the ground
            if (hit.normal == new Vector3(0, 1, 0) || hit.normal == new Vector3(0, -1, 0))
            {
                Vector3 tempRot = portals[portal].transform.localRotation.eulerAngles;
                tempRot.y = rotY;
                portals[portal].transform.rotation = Quaternion.Euler(tempRot);
            }
                
            //Handle level holes
            Vector4 rot = new Vector4(Mathf.Deg2Rad*(portals[portal].transform.localRotation.eulerAngles.x),
                    Mathf.Deg2Rad*(-portals[portal].transform.rotation.eulerAngles.y), 0, 0);
            foreach (Material m in mapParent.GetComponent<Renderer>().sharedMaterials) {
                m.SetVector("_PortalPos" + (portal + 1).ToString(), portals[portal].transform.position);
                m.SetVector("_PortalRot" + (portal + 1).ToString(), rot);
                m.SetFloat("_PortalTime" + (portal + 1).ToString(), Time.timeSinceLevelLoad);
                m.SetMatrix("_MatrixRotX" + (portal + 1).ToString(), MatrixRotX(rot.x));
                m.SetMatrix("_MatrixRotY" + (portal + 1).ToString(), MatrixRotY(rot.y));
            }

            //Connect portals if both exist
            LinkPortals();
            ScaleOnSpawn[] sosArr = tempPortal.GetComponentsInChildren<ScaleOnSpawn>(true);
            for (int i = 0; i < sosArr.Length; i++)
            {
                //Make sure objects are scaled
                sosArr[i].DoScale();
            }
        }
    }
    
    void FixOverhangs(Transform portal) {
        var testPoints = new List<Vector3>
        {
            new Vector3(-0.52f,  0.0f, 0.1f),
            new Vector3( 0.52f,  0.0f, 0.1f),
            new Vector3( 0.0f, -1.02f, 0.1f),
            new Vector3( 0.0f,  1.02f, 0.1f),
            
            new Vector3(-0.52f,  -1.02f, 0.1f),
            new Vector3( 0.52f,  1.02f, 0.1f),
            new Vector3( 0.52f, -1.02f, 0.1f),
            new Vector3( -0.52f,  1.02f, 0.1f)
        };

        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up,
            
            Vector3.right + Vector3.up,
            -(Vector3.right + Vector3.up),
            -Vector3.right + Vector3.up,
            -(-Vector3.right + Vector3.up),
        };
        for (int i = 0; i < 8; i++) {
            RaycastHit hit;
            Vector3 raycastPos = portal.TransformPoint(testPoints[i]);
            Vector3 raycastDir = portal.TransformDirection(testDirs[i]);
            
            //GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //tempCube.transform.position = raycastPos;
            //tempCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            if (Physics.CheckSphere(raycastPos, 0.05f, (1 << LayerMask.NameToLayer("Ground")))) {
                continue;
            } else if (Physics.Raycast(raycastPos, raycastDir, out hit, 1.02f, (1 << LayerMask.NameToLayer("Ground")))) {
                Vector3 offset = hit.point - raycastPos;
                portal.Translate(offset, Space.World);
            }
        }
    }
    
    void FixIntersections(Transform portal) {
        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up,
                
            Vector3.right + Vector3.up,
            -(Vector3.right + Vector3.up),
            -Vector3.right + Vector3.up,
            -(-Vector3.right + Vector3.up)
        };
        var testDists = new List<float> { 0.52f, 0.52f, 1.02f, 1.02f, 1.2f, 1.2f, 1.2f, 1.2f };
        
        for (int i = 0; i < 8; i++) {
            RaycastHit hit;
            Vector3 raycastPos = portal.TransformPoint(0.0f, 0.0f, -0.1f);
            Vector3 raycastDir = portal.TransformDirection(testDirs[i]);

            if (Physics.Raycast(raycastPos, raycastDir, out hit, testDists[i], LayerMask.GetMask("Ground", "StopPortal")))
            {
                var offset = (hit.point - raycastPos);
                var newOffset = -raycastDir * (testDists[i] - offset.magnitude);
                portal.Translate(newOffset, Space.World);
            }
        }
    }
    
    void FixUnportalable(Transform portal) {
        var testPoints = new List<Vector3>
        {
            new Vector3(-0.52f,  0.0f, -0.1f),
            new Vector3( 0.52f,  0.0f, -0.1f),
            new Vector3( 0.0f, -1.02f, -0.1f),
            new Vector3( 0.0f,  1.02f, -0.1f)
        };
        
        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up,
        };
        
        //50 iterations of fixing
        for (int j = 0; j < 50; j++) {
            for (int i = 0; i < 4; i++) {
                RaycastHit hit;
                Vector3 raycastPos = portal.TransformPoint(testPoints[i]);
                Vector3 raycastDir = portal.forward;
                
                if (Physics.Raycast(raycastPos, raycastDir, out hit, 0.2f, (1 << LayerMask.NameToLayer("Ground")))) {
                    if (hit.triangleIndex >= hit.collider.GetComponent<MeshFilter>().mesh.GetSubMesh(1).indexStart/3) {
                        //Unportalable Tile
                        portal.Translate(portal.TransformDirection(testDirs[i]) * 0.2f, Space.World);
                        break;
                    }
                }
                
                if (i == 3) {
                    //We have done checked all sides and have not translated/broken the loop, so we are done
                    return;
                }
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
                
                foreach (Renderer r in levelDuplicates[pair.Item1].GetComponentsInChildren<Renderer>()) {
                    foreach (Material m in r.materials) {
                        m.SetVector("_PlanePos",
                            new Vector4(portals[pair.Item2].transform.position.x, portals[pair.Item2].transform.position.y, portals[pair.Item2].transform.position.z, 1));
                        m.SetVector("_PlaneDir",
                            new Vector4(-portals[pair.Item2].transform.forward.x, -portals[pair.Item2].transform.forward.y, -portals[pair.Item2].transform.forward.z, 1));
                    }
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
            
            portalsLinkedEvent.Invoke();
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
    
    Matrix4x4 MatrixRotX(float theta) {
        Matrix4x4 out_matrix = new Matrix4x4();
        out_matrix.SetRow(0, new Vector4(1, 0, 0, 0));
        out_matrix.SetRow(1, new Vector4(0, Mathf.Cos(theta), -Mathf.Sin(theta), 0));
        out_matrix.SetRow(2, new Vector4(0, Mathf.Sin(theta), Mathf.Cos(theta), 0));
        out_matrix.SetRow(3, new Vector4(0, 0, 0, 1));
        return out_matrix;
    }
    
    Matrix4x4 MatrixRotY(float theta) {
        Matrix4x4 out_matrix = new Matrix4x4();
        out_matrix.SetRow(0, new Vector4(Mathf.Cos(theta), 0, Mathf.Sin(theta), 0));
        out_matrix.SetRow(1, new Vector4(0, 1, 0, 0));
        out_matrix.SetRow(2, new Vector4(-Mathf.Sin(theta), 0, Mathf.Cos(theta), 0));
        out_matrix.SetRow(3, new Vector4(0, 0, 0, 1));
        return out_matrix;
    }
}
