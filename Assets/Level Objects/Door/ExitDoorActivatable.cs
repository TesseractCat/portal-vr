using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorActivatable : Activatable
{
    public Transform leftSide;
    Vector3 leftSideDefaultPosition;
    
    public Transform rightSide;
    Vector3 rightSideDefaultPosition;
    
    public float openSpeed = 5.0f;
    
    bool opened;
    
    LevelSelector levelSelector;
    
    void Awake() {
        opened = false;
        leftSideDefaultPosition = leftSide.localPosition;
        rightSideDefaultPosition = rightSide.localPosition;
        
        levelSelector = FindObjectOfType<LevelSelector>();
    }
    
    void Update() {
        if (opened) {
            //leftSide.localPosition = Vector3.Lerp(leftSide.localPosition,
            //        leftSideDefaultPosition + -Vector3.right, Time.deltaTime * openSpeed);
            //rightSide.localPosition = Vector3.Lerp(rightSide.localPosition,
            //        rightSideDefaultPosition + Vector3.right, Time.deltaTime * openSpeed);
            leftSide.localPosition = Vector3.Lerp(leftSide.localPosition,
                    leftSideDefaultPosition + -Vector3.up * 2.0f, Time.deltaTime * openSpeed);
            rightSide.localPosition = Vector3.Lerp(rightSide.localPosition,
                    rightSideDefaultPosition + -Vector3.up * 2.0f, Time.deltaTime * openSpeed);
        } else {
            leftSide.localPosition = Vector3.Lerp(leftSide.localPosition, leftSideDefaultPosition, Time.deltaTime * openSpeed);
            rightSide.localPosition = Vector3.Lerp(rightSide.localPosition, rightSideDefaultPosition, Time.deltaTime * openSpeed);
        }
    }
    
    public override void Activate() {
        opened = true;
        levelSelector.Show();
    }
    
    public override void Deactivate() {
        opened = false;
        StartCoroutine(levelSelector.HideCoroutine());
    }
}
