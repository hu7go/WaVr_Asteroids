using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineBender))]
public class LineBendEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LineBender lineBend = (LineBender)target;

        if (GUILayout.Button("In range!"))
        {
            lineBend.TargetToggle();
        }
    }
}
