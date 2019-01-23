using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Manager mS = (Manager)target;

        if (GUILayout.Button("Start spawing enemies!"))
        {
            if (mS.StartedGame())
                mS.StartSpawningEnemies();
        }
    }

}
