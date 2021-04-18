using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Activator), true)]
public class ActivatorEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        Activator a = (Activator)target;
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Activate")) {
            a.Activate();
        }
        if (GUILayout.Button("Deactivate")) {
            a.Deactivate();
        }
        GUILayout.EndHorizontal();
    }
}
