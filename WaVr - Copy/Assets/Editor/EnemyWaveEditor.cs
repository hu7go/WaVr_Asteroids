using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_EnemyWave))]
public class EnemyWaveEditor : Editor
{
    int prevLength = 0;
    int prevNumberOfEnemies = 0;
    public List<int> testPrev = new List<int>();

    private void Awake()
    {
        SO_EnemyWave ew = (SO_EnemyWave)target;
        UpdatePercentList(ew);
        UpdateEnemyList(ew);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SO_EnemyWave ew = (SO_EnemyWave)target;

        if (prevLength != 0 && prevNumberOfEnemies != 0 && testPrev.Count != 0)
        {
            //! Update lists!
            if (ew.enemyTypes.Count != prevLength || prevNumberOfEnemies != ew.totalNumberOfEnemies)
            {
                UpdatePercentList(ew);
                UpdateEnemyList(ew);
            }
            //

            if (ew.numberOfEnemyTypes != ew.enemyTypes.Count)
            {
                UpdatePercentList(ew);
                UpdateEnemyList(ew);
            }

            GUILayout.Space(20);

            for (int i = 0; i < ew.numberOfEnemyTypes; i++)
            {
                ew.enemyTypes[i] = EditorGUILayout.ObjectField("Enemy type " + i + " : " + ew.enemyTypePercent[i], ew.enemyTypes[i], typeof(Enemies), true) as Enemies;
            }

            GUILayout.Space(20);

            //! show unit amount list only if there are more then 1 enemy type!
            if (ew.enemyTypes.Count > 1)
            {
                for (int i = 0; i < ew.enemyTypes.Count; i++)
                {
                    ew.enemyTypePercent[i] = EditorGUILayout.IntField(ew.enemyTypePercent[i]);
                }
            }
            else
            {
                ew.enemyTypePercent[0] = ew.totalNumberOfEnemies;
            }
            //

            for (int i = 0; i < ew.enemyTypePercent.Count; i++)
            {
                if (ew.enemyTypePercent[i] != testPrev[i])
                {
                    //testPrev[i] = ew.enemyTypePercent[i];
                    //! This is whenever you change a number in the percentage fields!
                    //UpdatePercentages(i, ew);
                }
            }
        }
        else
        {
            //UpdatePercentList(ew);
        }

        //Script management!
        prevNumberOfEnemies = ew.totalNumberOfEnemies;
        prevLength = ew.enemyTypes.Count;
        testPrev.Clear();
        for (int i = 0; i < ew.enemyTypePercent.Count; i++)
        {
            testPrev.Add(ew.enemyTypePercent[i]);
        }
    }

    private void UpdatePercentList (SO_EnemyWave ew)
    {
        ew.enemyTypePercent.Clear();

        for (int i = 0; i < ew.numberOfEnemyTypes; i++)
        {
            ew.enemyTypePercent.Add(0);
        }

        int tmp = 0;
        for (int i = 0; i < ew.totalNumberOfEnemies; i++)
        {
            ew.enemyTypePercent[tmp]++;
            tmp++;
            if (tmp >= ew.enemyTypePercent.Count)
            {
                tmp = 0;
            }
        }
    }

    private void UpdateEnemyList (SO_EnemyWave ew)
    {
        if (ew.numberOfEnemyTypes < ew.enemyTypes.Count)
        {
            for (int i = 0; i < ew.enemyTypes.Count - ew.numberOfEnemyTypes; i++)
            {
                ew.enemyTypes.RemoveAt(ew.enemyTypes.Count - 1);
            }
        }
        else if (ew.numberOfEnemyTypes > ew.enemyTypes.Count)
        {
            for (int i = 0; i <  ew.numberOfEnemyTypes - ew.enemyTypes.Count; i++)
            {
                ew.enemyTypes.Add(new Enemies());
            }
        }
    }

    private void UpdatePercentages (int index, SO_EnemyWave ew)
    {
        //int tmp = 0;
        //for (int i = 0; i < ew.enemyTypePercent.Count; i++)
        //{
        //    tmp += ew.enemyTypePercent[i];
        //}

        //int tmp2 = tmp - ew.totalNumberOfEnemies;

        //if (tmp2 > 0)
        //{
        //    for (int i = 0; i < ew.enemyTypePercent.Count; i++)
        //    {
        //        if (tmp2 == 0)
        //            break;

        //        if (i != index)
        //        {
        //            ew.enemyTypePercent[i]--;
        //            tmp2--;
        //        }
        //    }
        //}
        //else if (tmp2 < 0)
        //{
        //    for (int i = 0; i < ew.enemyTypePercent.Count; i++)
        //    {
        //        if (tmp2 == 0)
        //            break;

        //        if (i != index)
        //        {
        //            ew.enemyTypePercent[i]++;
        //            tmp2++;
        //        }
        //    }
        //}
    }
}
