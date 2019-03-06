using System.Collections.Generic;
using UnityEngine;

public class EAI_TFreezer : EnemyAI
{
    List<AsteroidHealth> asteroidsWithTurret = new List<AsteroidHealth>();
    List<TurretStruct> currentTarget = new List<TurretStruct>();

    private bool isCurrentlyFreezing = false;

    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex, Enemies enemyType)
    {
        base.Initialize(newList, newHealthThreshHold, newMaster, newSpawner, newWaveIndex, enemyType);

        CheckForTurrets();
    }

    public override void SetPath(List<AsteroidHealth> newPath)
    {
        objectiveOrder = newPath;
        if (seekAndDestroy)
        {
            CheckForTurrets();
        }
    }

    private bool tmpBool = false;
    private int prevPathCount = 0;

    public void CheckForTurrets()
    {
        prevPathCount = asteroidsWithTurret.Count;

        if (isCurrentlyFreezing)
        {
            Debug.Log("Is freezing " + nextTargetIndex, this);
        }

        if (asteroidsWithTurret.Count > 0)
        {
            if (asteroidsWithTurret[nextTargetIndex].asteroid.alive == true)
            {
                nextTargetIndex--;
                if (nextTargetIndex < 0)
                    nextTargetIndex = 0;
                tmpBool = true;
            }
        }

        Debug.Log("Target index " + nextTargetIndex, this);

        asteroidsWithTurret.Clear();

        for (int i = 0; i < objectiveOrder.Count; i++)
        {
            if (objectiveOrder[i].asteroid.hasTurret && objectiveOrder[i].asteroid.alive)
                asteroidsWithTurret.Add(objectiveOrder[i]);
        }
        if (asteroidsWithTurret.Count > 0)
            objectiveOrder = asteroidsWithTurret;

        if (asteroidsWithTurret.Count == 0)
        {
            GoHome();
            return;
        }

        if (tmpBool == false)
        {
            int tmp = 0;
            for (int i = 0; i < asteroidsWithTurret.Count; i++)
            {
                if (asteroidsWithTurret[i].frozen)
                {
                    tmp++;
                }
            }

            if (tmp == asteroidsWithTurret.Count)
            {
                GoHome();
                return;
            }
        }
        tmpBool = false;

        Debug.Log("Target index " + nextTargetIndex, this);

        if (asteroidsWithTurret.Count == prevPathCount)
        {
            if (nextTargetIndex != 0)
                nextTargetIndex++;
        }

        currentTarget = objective.GetComponentInParent<TurretMenuMaster>().turrets;
        objective = objectiveOrder[nextTargetIndex];
    }

    public override void ShootingBehaviour()
    {
        if (distance <= range && gun.shoot == false)
        {
            gun.shoot = true;
            gun.StartShooting(waveIndex, home, objective.GetComponent<AsteroidHealth>());
            objective.Freeze();
            currentTarget = objective.GetComponentInParent<TurretMenuMaster>().turrets;
        }
        else if (distance > range && gun.shoot == true)
        {
            currentTarget.Clear();
            gun.shoot = false;
            objective.UnFreeze();
            isCurrentlyFreezing = false;
            gun.StopShooting();
        }
    }

    public override void Movement()
    {
        base.Movement();

        if (seekAndDestroy)
        {
            if (objectiveOrder[nextTargetIndex].frozen == true && isCurrentlyFreezing == false)
            {
                if (objectiveOrder[nextTargetIndex].frozen == true && isCurrentlyFreezing == false)
                {
                    if (asteroidsWithTurret.Count != prevPathCount)
                    {
                        nextTargetIndex++;
                        if (nextTargetIndex > asteroidsWithTurret.Count)
                        {
                            nextTargetIndex--;
                        }
                        Debug.Log("Target index " + nextTargetIndex, this);
                    }
                }
                else
                {
                    nextTargetIndex--;
                }

                if (nextTargetIndex > asteroidsWithTurret.Count)
                {
                    nextTargetIndex--;
                    if (nextTargetIndex < 0)
                    {
                        nextTargetIndex = 0;
                        GoHome();
                        return;
                    }
                }
                else
                {
                    objective = objectiveOrder[nextTargetIndex];
                    currentTarget = objective.GetComponentInParent<TurretMenuMaster>().turrets;
                }
            }

            if (nextTargetIndex > objectiveOrder.Count)
            {
                GoHome();
                return;
            }

            if (gun.freeze)
            {
                isCurrentlyFreezing = true;
                foreach (TurretStruct tai in currentTarget)
                {
                    tai.ai.frozen = true;
                }
                objectiveOrder[nextTargetIndex].frozen = true;
            }
        }
    }

    public override void GoHome()
    {
        base.GoHome();
        foreach (TurretStruct turret in currentTarget)
        {
            turret.ai.frozen = false;
        }
    }

    public override void Kill()
    {
        foreach (TurretStruct turret in currentTarget)
        {
            turret.ai.frozen = false;
        }
        objectiveOrder[nextTargetIndex].frozen = false;
        base.Kill();
    }
}