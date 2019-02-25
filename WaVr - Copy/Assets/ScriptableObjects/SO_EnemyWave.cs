using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_EnemyWave : ScriptableObject
{
    public List<Enemies> enemies;
    [Range(0, 100), Tooltip("Dont change list lenght here! it will not work if you do")]
    public List<float> enemyPercent;
}
