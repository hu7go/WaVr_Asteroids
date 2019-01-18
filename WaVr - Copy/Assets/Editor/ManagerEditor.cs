using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor
{
    bool foldOutTest = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Manager mS = (Manager)target;

        //foldOutTest = EditorGUILayout.Foldout(foldOutTest, "Test");
        //if (foldOutTest)
        //{

        //}
    }

}
