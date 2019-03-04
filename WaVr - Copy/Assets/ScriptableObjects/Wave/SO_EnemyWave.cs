using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "ScriptableObject/Wave/Wave", order = 1)]
public class SO_EnemyWave : ScriptableObject
{
    public int totalNumberOfEnemies = 20;
    public int numberOfEnemyTypes = 1;
    [HideInInspector] public List<Enemies> enemyTypes;
    [Space(10)]
    [HideInInspector] public List<int> enemyTypePercent;
    [HideInInspector] public List<int> prevEnemyTypePercent;
}
