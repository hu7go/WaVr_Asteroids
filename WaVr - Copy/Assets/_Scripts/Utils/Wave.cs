using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public float maxNumberOfEnemies;
    public float damageThreshHold;
    public Transform spawnPosition;
    public List<Enemies> enemyTypes;
    [HideInInspector] public float currentNumberOfEnemies;
    [HideInInspector] public List<EnemyAI> enemies;
    [HideInInspector] public float damageDone;
}
