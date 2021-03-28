using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EditorMode {Editor, Painter, Objects}

public class LevelEditor : MonoBehaviour {

    //Selection variables
    Transform selectionStart;
    Transform selectionEnd;
    Vector3 selectionNormal = Vector3.zero;

    //Controller related variables
    public Transform leftControllerTransform;
    
    public Transform rightControllerTransform;

    //Prefabs
    public GameObject selectionNode;
    public GameObject laser;
    public GameObject palette;

    //Editor variables
    GameObject currentObject;
    VRButton button = null;

    /*void Update () {
        /*if (leftTrackedObject.index == SteamVR_TrackedObject.EIndex.None)
        {
            return;
        }
        if (rightTrackedObject.index == SteamVR_TrackedObject.EIndex.None)
        {
            return;
        }
        leftDevice = SteamVR_Controller.Input((int)leftTrackedObject.index);
        rightDevice = SteamVR_Controller.Input((int)rightTrackedObject.index);

        if (leftDevice.GetHairTrigger())
        {
            laser.SetActive(true);
        } else
        {
            laser.SetActive(false);
        }

        if (SteamVR_Actions.default_LeftMenuButton.GetLastStateDown(SteamVR_Input_Sources.LeftHand))
        {
            palette.transform.position = palette.GetComponent<LerpObjToPos>().target.position;
            palette.SetActive(!palette.activeSelf);

            //Disable portal and reenable default model on right controller for pallete selection
            rightControllerTransform.transform.Find("Portal Gun").gameObject.SetActive(!rightControllerTransform.transform.Find("Portal Gun").gameObject.activeSelf);
            rightControllerTransform.transform.Find("Portal Manager").gameObject.SetActive(!rightControllerTransform.transform.Find("Portal Manager").gameObject.activeSelf);
            rightControllerTransform.transform.Find("LaserPointer").gameObject.SetActive(true);

            //Remove markers
            try
            {
                GameObject.Destroy(selectionStart.gameObject);
                GameObject.Destroy(selectionEnd.gameObject);
            } catch
            {
                //Ignore error
            }
        }

        if (palette.activeSelf)
        {
            if (SteamVR_Actions.default_RightTrigger.GetLastStateDown(SteamVR_Input_Sources.RightHand))
            {
                Ray ray = new Ray(rightControllerTransform.transform.position, rightControllerTransform.transform.TransformDirection(Vector3.forward));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("VRUI")))
                {
                    hit.collider.GetComponent<VRButton>().OnSelect();
                    button = hit.collider.GetComponent<VRButton>();
                    if (hit.collider.GetComponent<PaletteIcon>())
                    {
                        currentObject = hit.collider.GetComponent<PaletteIcon>().placeablePrefab;
                    }
                }
            } else
            {
                Ray ray = new Ray(rightControllerTransform.transform.position, rightControllerTransform.transform.TransformDirection(Vector3.forward));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("VRUI")))
                {
                    hit.collider.GetComponent<VRButton>().OnHover();
                }
                }
        }

        if (!palette.activeSelf && button != null)
        {
            #region EditorMode Editor
            if (button.buttonMode == EditorMode.Editor)
            {
                if (SteamVR_Actions.default_LeftTrigger.GetLastStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    SteamVR_Actions.default_Haptic.Execute(0, 0.2f, 50, 1, SteamVR_Input_Sources.LeftHand);

                    Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.TransformDirection(Vector3.forward));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        Vector3 hitPos = hit.point + (hit.normal * 0.5f);
                        hitPos = roundVectorToDivisible(2, hitPos);
                        if (selectionStart != null)
                        {
                            GameObject.Destroy(selectionStart.gameObject);
                            GameObject.Destroy(selectionEnd.gameObject);
                        }
                        selectionStart = Instantiate(selectionNode, hitPos, Quaternion.identity).transform;
                        selectionNormal = hit.normal;
                    }
                }

                if (SteamVR_Actions.default_LeftTrigger.GetLastStateUp(SteamVR_Input_Sources.LeftHand))
                {
                    SteamVR_Actions.default_Haptic.Execute(0, 0.2f, 50, 1, SteamVR_Input_Sources.LeftHand);

                    Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.TransformDirection(Vector3.forward));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        Vector3 hitPos = hit.point + (hit.normal * 0.5f);
                        hitPos = roundVectorToDivisible(2, hitPos);
                        if (hit.normal == selectionNormal)
                        {
                            selectionEnd = Instantiate(selectionNode, hitPos, Quaternion.identity).transform;
                        }
                        else
                        {
                            SteamVR_Actions.default_Haptic.Execute(0, 0.2f, 50, 1, SteamVR_Input_Sources.LeftHand);

                            GameObject.Destroy(selectionStart.gameObject);
                        }
                    }
                }

                if (SteamVR_Actions.default_LevelEditorButton.GetLastStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    ProcLevelMesh pMesh = GetComponent<ProcLevelMesh>();
                    if (SteamVR_Actions.default_Movement.GetAxis(SteamVR_Input_Sources.LeftHand).y > 0)
                    {
                        Vector3 selStartPos = (selectionStart.position / 2) - (transform.position / 2);
                        Vector3 selEndPos = (selectionEnd.position / 2) - (transform.position / 2);
                        for (int x = (int)Mathf.Min(selStartPos.x, selEndPos.x); x <= Mathf.Max(selStartPos.x, selEndPos.x); x++)
                        {
                            for (int y = (int)Mathf.Min(selStartPos.y, selEndPos.y); y <= Mathf.Max(selStartPos.y, selEndPos.y); y++)
                            {
                                for (int z = (int)Mathf.Min(selStartPos.z, selEndPos.z); z <= Mathf.Max(selStartPos.z, selEndPos.z); z++)
                                {
                                    pMesh.levelArr[x, y, z] = 0;
                                }
                            }
                        }
                        selectionStart.transform.position += selectionNormal * 2;
                        selectionEnd.transform.position += selectionNormal * 2;
                    }
                    else if (SteamVR_Actions.default_Movement.GetAxis(SteamVR_Input_Sources.LeftHand).y < 0)
                    {
                        Vector3 selStartPos = (selectionStart.position / 2) + -selectionNormal - (transform.position / 2);
                        Vector3 selEndPos = (selectionEnd.position / 2) + -selectionNormal - (transform.position / 2);
                        for (int x = (int)Mathf.Min(selStartPos.x, selEndPos.x); x <= Mathf.Max(selStartPos.x, selEndPos.x); x++)
                        {
                            for (int y = (int)Mathf.Min(selStartPos.y, selEndPos.y); y <= Mathf.Max(selStartPos.y, selEndPos.y); y++)
                            {
                                for (int z = (int)Mathf.Min(selStartPos.z, selEndPos.z); z <= Mathf.Max(selStartPos.z, selEndPos.z); z++)
                                {
                                    pMesh.levelArr[x, y, z] = 1;
                                }
                            }
                        }
                        selectionStart.transform.position -= selectionNormal * 2;
                        selectionEnd.transform.position -= selectionNormal * 2;
                    }
                    pMesh.Generate();
                }
            }
            #endregion

            #region EditorMode Painter
            if (button.buttonMode == EditorMode.Painter)
            {
                if (SteamVR_Actions.default_LeftTrigger.GetLastStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    SteamVR_Actions.default_Haptic.Execute(0, 0.2f, 50, 1, SteamVR_Input_Sources.LeftHand);

                    Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.TransformDirection(Vector3.forward));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        Vector3 hitPos = hit.point + (hit.normal * 0.5f);
                        hitPos = roundVectorToDivisible(2, hitPos);
                        hitPos = (hitPos / 2) - (transform.position / 2);
                        GetComponent<ProcLevelMesh>().wallDict[new Tuple<Vector3, Vector3>(hitPos, hit.normal)] = button.buttonData;
                    }
                    GetComponent<ProcLevelMesh>().Generate();
                }
            }
            #endregion

            #region EditorMode Objects
            if (button.buttonMode == EditorMode.Objects)
            {
                if (SteamVR_Actions.default_LeftTrigger.GetLastStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    SteamVR_Actions.default_Haptic.Execute(0, 0.2f, 50, 1, SteamVR_Input_Sources.LeftHand);

                    Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.TransformDirection(Vector3.forward));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        Vector3 hitPos = hit.point + (hit.normal * 0.5f);
                        hitPos = roundVectorToDivisible(2, hitPos);
                        hitPos = hitPos - (hit.normal * 0.99f);
                        GameObject tempObj = Instantiate(button.GetComponent<PaletteIcon>().placeablePrefab, hitPos, Quaternion.identity);
                        tempObj.transform.up = hit.normal;
                        tempObj.transform.position = hitPos;
                        //tempObj.transform.parent = this.transform;
                        tempObj.transform.parent = GameObject.Find("LevelObjects").transform;
                    }
                }
            }
            #endregion
        }
    }*/

    Vector3 roundVectorToDivisible(int divisible, Vector3 vector)
    {
        return new Vector3(roundNumberToDivisible(2, vector.x), roundNumberToDivisible(2, vector.y), roundNumberToDivisible(2, vector.z));
    }

    int roundNumberToDivisible(int divisible, float number)
    {
        return (int)Mathf.Round(number / divisible) * divisible;
    }
}
