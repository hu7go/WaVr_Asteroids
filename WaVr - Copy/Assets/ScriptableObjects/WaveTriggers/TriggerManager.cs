﻿using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : ScriptableObject
{
    protected EnemySpawnPoint prevSpawnPoint;

    public virtual bool Trigger()
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

[CreateAssetMenu(fileName = "TimerTrigger", menuName = "ScriptableObject/Wave/TimerTrigger", order = 4)]
public class Timer : TriggerManager
{
    bool countDown = true;
    private float timer = 0;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            countDown = true;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
            //prevSpawnPoint.myWaveInfo.timer = 0;
        }

        if (prevSpawnPoint.myWaveInfo.timer > 10 && countDown)
        {
            countDown = false;
            //prevSpawnPoint.myWaveInfo.timer = 0;
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            timer += Time.deltaTime;
            Utils.ClearLogConsole();
            Debug.Log(timer);
            //prevSpawnPoint.myWaveInfo.timer += Time.deltaTime;
            return false;
        }
    }
}