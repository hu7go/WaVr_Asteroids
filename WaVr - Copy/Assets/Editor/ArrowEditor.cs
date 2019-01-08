using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeSide)), CanEditMultipleObjects]
public class ArrowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChangeSide script = (ChangeSide)target;

        //if (GUILayout.Button("Do arrow function"))
        //    script.DoFunction();
    }
}