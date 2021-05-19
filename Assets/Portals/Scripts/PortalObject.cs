using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalObject : MonoBehaviour
{
    public GameObject visualsPrefab;
    public bool dynamicPosition = true;
    public bool syncScale = false;
    public int onlyOneDuplicate = -1;
    
    GameObject[] visualClones;
    PortalManager portalManager;
    Transform mapParent;
    
    bool inPortal = false;
    
    void Start()
    {
        mapParent = GameObject.FindWithTag("Level").transform;
        portalManager = FindObjectOfType<PortalManager>();
        if (portalManager != null) {
            portalManager.portalsLinkedEvent.AddListener(OnPortalsLinked);
            
            //Check to see if we need to sync on spawn
            if (portalManager.portals[0] != null && portalManager.portals[1] != null) {
                OnPortalsLinked();
            }
        }
    }
    
    void OnDestroy() {
        if (visualClones != null) {
            foreach (GameObject c in visualClones) {
                if (c != null) {
                    GameObject.Destroy(c);
                }
            }
        }
    }
    
    void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "PortalColl") {
            int portalIndex = (c.transform.parent.gameObject == portalManager.portals[0] ? 0 : 1);
            EnterPortal(portalIndex);
        }
    }
    
    void OnTriggerExit (Collider c)
    {
        if (!dynamicPosition)
            return;
        
        if (c.gameObject.tag == "PortalColl") {
            gameObject.layer = LayerMask.NameToLayer("Default");
            int portalIndex = (c.transform.parent.gameObject == portalManager.portals[0] ? 0 : 1);
            ExitPortal(portalIndex);
        }
    }
    
    void OnTriggerStay (Collider c)
    {
        if (!dynamicPosition)
            return;
        
        if (c.gameObject.tag == "PortalColl" &&
                c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal == null)
            return;
        
        if (c.gameObject.tag == "PortalColl" && gameObject.layer != LayerMask.NameToLayer("PortalFrameLayer")) {
            gameObject.layer = LayerMask.NameToLayer("PortalFrameLayer");
            
            //Sometimes the object gets stuck on top of the portal, this is to jog it to move
            GetComponent<Rigidbody>().AddForce(new Vector3(0,0.01f,0), ForceMode.Impulse);
        }
        
        //Teleport
        if (c.gameObject.tag == "PortalColl" && c.transform.parent.InverseTransformPoint(transform.position).z > 0.01f)
        {
            Quaternion flipRot = Quaternion.AngleAxis(180.0f, Vector3.up);
            
            Transform linkedPortal = c.transform.parent.GetComponentInChildren<CorrespondingPortal>().correspondingPortal;

            //Handle position
            transform.position = linkedPortal.TransformPoint(
                    flipRot * c.transform.parent.InverseTransformPoint(transform.position));
            
            transform.rotation = linkedPortal.rotation * (flipRot * (Quaternion.Inverse(c.transform.parent.rotation) * transform.rotation));
            
            //Handle velocity
            transform.GetComponent<Rigidbody>().velocity = linkedPortal.TransformDirection(
                    flipRot * c.transform.parent.InverseTransformDirection(transform.GetComponent<Rigidbody>().velocity));
            
            transform.GetComponent<Rigidbody>().angularVelocity = linkedPortal.TransformDirection(
                    flipRot * c.transform.parent.InverseTransformDirection(transform.GetComponent<Rigidbody>().angularVelocity));
        }
    }
    
    public void OnPortalsLinked()
    {
        if (visualClones == null) {
            visualClones = new GameObject[2];
            for (int i = 0; i <= 1; i++) {
                if (onlyOneDuplicate != -1 && onlyOneDuplicate != i)
                    continue;
                
                //Create visual clones
                visualClones[i] = (GameObject)Instantiate(visualsPrefab);
                
                if (visualClones[i].GetComponent<PortalObject>() != null)
                    GameObject.Destroy(visualClones[i].GetComponent<PortalObject>());
                
                //Destroy colliders
                foreach (Collider c in visualClones[i].GetComponentsInChildren<Collider>()) {
                    Destroy(c);
                }
            }
        }
        for (int i = 0; i <= 1; i++) {
            if (onlyOneDuplicate != -1 && onlyOneDuplicate != i)
                continue;
            
            if (!inPortal) {
                int layer = LayerMask.NameToLayer("Portal" + (i + 1).ToString() + "LightMask");
                ConfigureClone(visualClones[i], layer, i + 1, true,
                        portalManager.portals[i].GetComponent<CorrespondingPortal>().correspondingPortal.position,
                        -portalManager.portals[i].GetComponent<CorrespondingPortal>().correspondingPortal.forward);
            } else {
                ConfigureClone(visualClones[i], 0, 0, false, Vector3.zero, Vector3.zero); 
            }
        }
        UpdateVisualClones();
    }
    
    void UpdateVisualClones() {
        if (visualClones != null) {
            for (int i = 0; i <= 1; i++) {
                if (onlyOneDuplicate != -1 && onlyOneDuplicate != i)
                    continue;
                
                Vector3 mapRelativePosition = mapParent.InverseTransformPoint(transform.position);
                visualClones[i].transform.position =
                    portalManager.levelDuplicates[i].transform.GetChild(0).TransformPoint(mapRelativePosition);
                
                visualClones[i].transform.rotation =
                    portalManager.levelDuplicates[i].transform.GetChild(0).rotation
                    * (Quaternion.Inverse(mapParent.rotation) * transform.rotation);
                
                if (syncScale)
                    visualClones[i].transform.localScale = transform.localScale;
            }
        }
    }
    
    void EnterPortal(int portalIndex) {
        inPortal = true;
        if (visualClones != null) {
            ConfigureClone(visualClones[portalIndex], 0, 0, false, Vector3.zero, Vector3.zero); 
        }
    }
    void ExitPortal(int portalIndex) {
        inPortal = false;
        if (visualClones != null) {
            int layer = LayerMask.NameToLayer("Portal" + (portalIndex + 1).ToString() + "LightMask");
            ConfigureClone(visualClones[portalIndex], layer, portalIndex + 1, true,
                    portalManager.portals[portalIndex].GetComponent<CorrespondingPortal>().correspondingPortal.position,
                    -portalManager.portals[portalIndex].GetComponent<CorrespondingPortal>().correspondingPortal.forward);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dynamicPosition)
            UpdateVisualClones();
    }
    
    public void ConfigureClone(GameObject toSet, int layer, int stencilMask, bool changeStencilComp, Vector3 planePos, Vector3 planeDir)
    {
        foreach (Renderer r in toSet.GetComponentsInChildren<Renderer>())
        {
            r.gameObject.layer = layer;
            foreach (Material m in r.materials)
            {
                m.SetInt("_StencilMask", stencilMask);
                m.SetInt("_StencilComp", changeStencilComp ? 3 : 8);
                
                if (planePos != null)
                    m.SetVector("_PlanePos", new Vector4(
                                planePos.x, planePos.y, planePos.z, 1.0f));
                if (planeDir != null)
                    m.SetVector("_PlaneDir", new Vector4(
                                planeDir.x, planeDir.y, planeDir.z, 1.0f));
                
                //So the object isn't masked by the depth mask
                m.renderQueue = 2000;
                //m.renderQueue = m.renderQueue - 50;
            }
        }
    }
}
