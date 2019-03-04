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
    [HideInInspector] public float timer = 0;

    [Tooltip("BEWARE!! This should not be more than there are triggers in the triggers list!!!")]
    public int numberOfrequiredTriggers = 1;
    public List<TriggerManager> triggers;
}