using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TurretMenu)), CanEditMultipleObjects]
public class TurretMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TurretMenu script = (TurretMenu)target;

        if (GUILayout.Button("Start Turret Menu"))
        {
            script.CheckWhichSideCanSeePlayer();
        }
    }
}
