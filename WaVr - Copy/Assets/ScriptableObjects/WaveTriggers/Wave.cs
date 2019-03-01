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

    public TriggerManager triggerManager;
}

public class TriggerManager : ScriptableObject
{
    public virtual bool Trigger ()
    {
        return false;
    }
}

[CreateAssetMenu(fileName = "ThreasholdReachedTrigger", menuName = "ScriptableObject/Wave/ThreasholdReachedTrigger", order = 1)]
public class ThreasholdReached : TriggerManager
{
    public override bool Trigger()
    {
        return true;
    }
}

[CreateAssetMenu(fileName = "EnemiesDiedTrigger", menuName = "ScriptableObject/Wave/EnemiesDiedTrigger", order = 2)]
public class EnemiesDied : TriggerManager
{
    public override bool Trigger()
    {
        return true;
    }
}
