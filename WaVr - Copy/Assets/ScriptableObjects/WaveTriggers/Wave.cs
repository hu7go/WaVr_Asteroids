using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string tag;
    public int index = 0;
    public float damageThreshHold;
    public Transform spawnPosition;
    public SO_EnemyWave enemyController;
    public bool extraWaitTimeBool = true;
    public float extraWaitTime = 10f;
    [Tooltip("This time will only be in effect if the time trigger is active!")]
    [HideInInspector] public float currentNumberOfEnemies;
    [HideInInspector] public List<EnemyAI> enemies;
    [HideInInspector] public float damageDone;

    [Header("Trigger to spawn this wave!")]
    [Space(20)]
    [Tooltip("BEWARE!! This should not be more than there are triggers in the triggers list!!!")]
    public int numberOfRequiredTriggers = 1;
    public List<TriggerManager> triggers;
    public float waitTime = 0;
}