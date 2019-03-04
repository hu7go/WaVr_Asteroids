using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor
{
    int wv = 1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Manager mS = (Manager)target;

        if(GUILayout.Button("Start Enemies without turret"))
        {
            if (EditorApplication.isPlaying)
                mS.StartEnemiesEditor();
        }

        if (GUILayout.Button("Switch world version"))
        {
            if (EditorApplication.isPlaying)
            {
                switch (wv)
                {
                    case 1:
                        mS.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.one;
                        break;
                    case 2:
                        mS.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.two;
                        break;
                    case 3:
                        mS.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.three;
                        wv = 0;
                        break;
                }

                mS.SetWorldVersion();

                wv++;
            }
            else
                Debug.Log("Not currently in play mode!");
        }
    }

}
