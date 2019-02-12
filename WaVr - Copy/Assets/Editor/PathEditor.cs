using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAI))]
public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EnemyAI ai = (EnemyAI)target;

        if (GUILayout.Button("Show path"))
        {
            ai.drawPath = !ai.drawPath;
        }
    }
}
