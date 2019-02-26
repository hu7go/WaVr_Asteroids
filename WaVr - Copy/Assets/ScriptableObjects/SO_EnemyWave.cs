using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_EnemyWave : ScriptableObject
{
    public int totalNumberOfEnemies;
    public int numberOfEnemyTypes;
    [HideInInspector] public List<Enemies> enemyTypes;
    [Space(10)]
    [HideInInspector] public List<int> enemyTypePercent;
    [HideInInspector] public List<int> prevEnemyTypePercent;
}
