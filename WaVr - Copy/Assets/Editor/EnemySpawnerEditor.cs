using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawnPoint))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EnemySpawnPoint ep = (EnemySpawnPoint)target;

        if (GUILayout.Button("Show path"))
        {
            ep.drawPath = !ep.drawPath;
        }
    }
}
