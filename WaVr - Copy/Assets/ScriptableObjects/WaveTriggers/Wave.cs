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
    protected EnemySpawnPoint prevSpawnPoint = Manager.Instance.currentSpawnPoint;

    public virtual bool Trigger ()
    {
        return false;
    }
}

[CreateAssetMenu(fileName = "ThreasholdReachedTrigger", menuName = "ScriptableObject/Wave/ThreasholdReachedTrigger", order = 2)]
public class ThreasholdReached : TriggerManager
{
    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            prevSpawnPoint = Manager.Instance.currentSpawnPoint
        }
        Debug.Log(prevSpawnPoint.name);
        if (prevSpawnPoint.damageDonePercent <= prevSpawnPoint.myWaveInfo.damageThreshHold)
        {
            Debug.Log("Get triggered");
            return true;
        }
        else
        {
            Debug.Log("Not yet!");
            return false;
        }
    }
}

[CreateAssetMenu(fileName = "EnemiesDiedTrigger", menuName = "ScriptableObject/Wave/EnemiesDiedTrigger", order = 3)]
public class EnemiesDied : TriggerManager
{
    public override bool Trigger()
    {
        if (prevSpawnPoint.mySpawner.enemies.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
