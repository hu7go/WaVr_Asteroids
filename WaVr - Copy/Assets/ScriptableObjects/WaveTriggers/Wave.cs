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

    public List<TriggerManager> triggers;
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
    float currentMaxHealth = 100;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            currentMaxHealth = Manager.Instance.healthPercent;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (Manager.Instance.healthPercent <= prevSpawnPoint.myWaveInfo.damageThreshHold)
        {
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}

[CreateAssetMenu(fileName = "EnemiesDiedTrigger", menuName = "ScriptableObject/Wave/EnemiesDiedTrigger", order = 3)]
public class EnemiesDied : TriggerManager
{
    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;

        if (prevSpawnPoint.mySpawner == null)
        {
            return false;
        }

        if (prevSpawnPoint.mySpawner.enemies.Count == 0 && prevSpawnPoint.spawned == true)
        {
            if (prevSpawnPoint.mySpawner.doneSpawning)
            {
                prevSpawnPoint = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}

[CreateAssetMenu(fileName = "DamageDoneTrigger", menuName = "ScriptableObject/Wave/DamageDoneTrigger", order = 3)]
public class DamageDone : TriggerManager
{
    float currentMaxHealth = 100;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            currentMaxHealth = Manager.Instance.healthPercent;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (prevSpawnPoint.damageDonePercent >= 100 - prevSpawnPoint.myWaveInfo.damageThreshHold)
        {
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}

[CreateAssetMenu(fileName = "TimerTrigger", menuName = "ScriptableObject/Wave/TimerTriggerNOTDONEYET", order = 4)]
public class Timer : TriggerManager
{
    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (prevSpawnPoint.damageDonePercent >= 100 - prevSpawnPoint.myWaveInfo.damageThreshHold)
        {
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}
