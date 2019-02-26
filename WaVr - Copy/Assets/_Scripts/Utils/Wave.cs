using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public float damageThreshHold;
    public Transform spawnPosition;
    public SO_EnemyWave enemyController;
    [HideInInspector] public float currentNumberOfEnemies;
    [HideInInspector] public List<EnemyAI> enemies;
    [HideInInspector] public float damageDone;
}
