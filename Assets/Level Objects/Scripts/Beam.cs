using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Activatable
{
    [Header("Object References")]
    public GameObject beamPrefab;
    public Transform emissionPoint;
    
    [Header("Config")]
    public bool canPortal = true;
    
    Transform beamInstance;
    Transform duplicateBeam;
    
    bool activated = false;
    
    private Dictionary<string, object> _properties = new Dictionary<string, object>() {
        {"Start On", true}
    };
    public override Dictionary<string, object> properties {
        get {
            return _properties;
        }
        set {
            _properties = value;
        }
    }
    
    void Start()
    {
        PortalManager portalManager = FindObjectOfType<PortalManager>();
        if (portalManager != null) {
            portalManager.portalsLinkedEvent.AddListener(OnPortalsLinked);

            if ((bool)_properties["Start On"])
                Activate();
        }
    }

    void OnPortalsLinked()
    {
        if (!activated)
            return;
        
        if (duplicateBeam != null)
            GameObject.Destroy(duplicateBeam.gameObject);
        
        RaycastHit hit;
        if (Physics.Raycast(emissionPoint.position, transform.up, out hit, Mathf.Infinity, LayerMask.GetMask("Ground", "PortalTrigLayer"))) {
            beamInstance.localScale = new Vector3(1, hit.distance + 0.5f, 1);
            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalTrigLayer") && canPortal) {
                Transform correspondingPortal = hit.collider.transform.parent.GetComponent<CorrespondingPortal>().correspondingPortal;
                Vector3 inversePoint = hit.collider.transform.parent.InverseTransformPoint(hit.point);
                inversePoint = Quaternion.AngleAxis(180, Vector3.up) * inversePoint;
                Quaternion inverseRotation = Quaternion.AngleAxis(180, Vector3.up) * Quaternion.Inverse(hit.collider.transform.parent.rotation) * transform.rotation;
                
                GameObject tempBeam = Instantiate(beamPrefab, correspondingPortal.TransformPoint(inversePoint), correspondingPortal.rotation * inverseRotation, transform);
                
                duplicateBeam = tempBeam.transform;
                
                RaycastHit recursiveHit;
                if (Physics.Raycast(correspondingPortal.TransformPoint(inversePoint), correspondingPortal.rotation * inverseRotation * Vector3.up, out recursiveHit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
                    tempBeam.transform.localScale = new Vector3(1, recursiveHit.distance, 1);
                }
            }
        }
    }
    
    public override void Activate() {
        if (activated)
            return;
        
        beamInstance = Instantiate(beamPrefab, emissionPoint.position, transform.rotation, transform).transform;
        activated = true;
        OnPortalsLinked();
    }
    
    public override void Deactivate() {
        activated = false;
        if (beamInstance != null)
            GameObject.Destroy(beamInstance.gameObject);
        if (duplicateBeam != null)
            GameObject.Destroy(duplicateBeam.gameObject);
    }
}
