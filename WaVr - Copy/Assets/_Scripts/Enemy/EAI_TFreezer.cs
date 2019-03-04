using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_TFreezer : EnemyAI
{
    List<AsteroidHealth> asteroidsWithTurret;
    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex, Enemies enemyType)
    {
        base.Initialize(newList, newHealthThreshHold, newMaster, newSpawner, newWaveIndex, enemyType);
        for(int i = 0; i < Manager.Instance.asteroidList.Count; i++)
        {
            if (Manager.Instance.asteroidList[i].asteroid.hasTurret)
                asteroidsWithTurret.Add(Manager.Instance.asteroidList[i]);
        }
        objectiveOrder = asteroidsWithTurret;
    }

    public override void SetPath(List<AsteroidHealth> newPath)
    {
        if (objectiveOrder[nextTargetIndex].asteroid.hasTurret)
        {
            nextTargetIndex--;
            if (nextTargetIndex < 0)
                nextTargetIndex = 0;
        }
        objectiveOrder = newPath;
    }

    public override void Movement()
    {
        base.Movement();
        if (gun.freeze && freezing == false)
            foreach  (TurretAI tai in objective.GetComponentsInChildren<TurretAI>())
            {
                tai.frozen = false;
                freezing = true;
            }
    }
}