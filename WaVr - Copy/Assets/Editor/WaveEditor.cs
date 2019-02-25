using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_EnemyWave))]
public class WaveEditor : Editor
{
    float prev = 0;
    List<float> myList;
    float numberThatChanged;
    int notChange;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SO_EnemyWave ew = (SO_EnemyWave)target;

        if (ew.enemies.Count != prev)
        {
            myList = new List<float>();

            UpdateList(ew);
        }

        if (ew.enemyPercent.Count != ew.enemies.Count)
        {
            UpdateList(ew);
        }

        bool change = false;

        for (int i = 0; i < ew.enemyPercent.Count; i++)
        {
            if (ew.enemyPercent[i] != myList[i])
            {
                notChange = i;
                Debug.Log(i);
                numberThatChanged = ew.enemyPercent[i];
                change = true;
            }
        }

        if (change)
        {
            for (int i = 0; i < ew.enemyPercent.Count; i++)
            {
                if (i != notChange)
                {
                    float newPercent = (100 - numberThatChanged) / (ew.enemies.Count - 1);
                    ew.enemyPercent[i] = newPercent;
                    myList[i] = newPercent;
                }
            }
        }

        prev = ew.enemies.Count;
    }

    private void UpdateList (SO_EnemyWave ew)
    {
        ew.enemyPercent.Clear();

        for (float i = 0; i < ew.enemies.Count; i++)
        {
            float percent = 100 / ew.enemies.Count;
            ew.enemyPercent.Add(percent);
            myList.Add(percent);
        }
    }
}
