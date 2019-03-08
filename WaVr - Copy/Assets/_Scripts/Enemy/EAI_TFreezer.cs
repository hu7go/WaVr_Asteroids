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
            CheckForTurrets();
    }

    private bool tmpBool = false;
    private int prevPathCount = 0;

    public void CheckForTurrets()
    {
        if (seekAndDestroy == false)
        {
            return;
        }

        prevPathCount = asteroidsWithTurret.Count;

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

        asteroidsWithTurret.Clear();

        for (int i = 0; i < objectiveOrder.Count; i++)
            if (objectiveOrder[i].asteroid.hasTurret && objectiveOrder[i].asteroid.alive)
                asteroidsWithTurret.Add(objectiveOrder[i]);

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
                if (asteroidsWithTurret[i].frozen)
                    tmp++;

            if (tmp == asteroidsWithTurret.Count)
            {
                GoHome();
                return;
            }
        }
        tmpBool = false;

        if (asteroidsWithTurret.Count == prevPathCount)
            if (nextTargetIndex != 0)
            {
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
        if (seekAndDestroy)
        {
            if (objectiveOrder[nextTargetIndex] != null)
            {
                objective = objectiveOrder[nextTargetIndex];
            }
            else
            {
                nextTargetIndex--;
                if (nextTargetIndex < 0)
                {
                    nextTargetIndex = 0;
                    GoHome();
                    return;
                }
            }

            if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
            {
                nextTargetIndex++;
            }

            distance = Vector3.Distance(transform.position, objective.transform.position);

            if (distance > range)
            {
                isCurrentlyFreezing = false;
                gun.freeze = false;
            }
            else
                gun.freeze = true;

            if (distance > stopDistance)
            {
                onTheWay = true;
                if (distance < range)
                {
                    tmpSpeed = privateSpeed;
                    transform.RotateAround(objective.transform.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed * 2) * Time.deltaTime);
                }
                transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
            }
            if (distance <= stopDistance)
            {
                onTheWay = false;
                transform.RotateAround(objective.transform.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed) * Time.deltaTime);
            }

            if (objectiveOrder[nextTargetIndex].frozen == true && isCurrentlyFreezing == false)
            {
                if (asteroidsWithTurret.Count != prevPathCount)
                {
                    nextTargetIndex++;
                    if (nextTargetIndex >= asteroidsWithTurret.Count)
                    {
                        nextTargetIndex = 0;
                        GoHome();
                        return;
                    }
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
                    tai.ai.frozen = true;

                objectiveOrder[nextTargetIndex].frozen = true;
            }
        }
        else
        {
            distance = Vector3.Distance(transform.position, objective.transform.position);

            if (distance < 2)
            {
                Kill();
                home.Over();
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
        }

        transform.LookAt(objective.transform, Manager.Instance.GetWorldAxis());
    }

    public override void GoHome()
    {
        base.GoHome();
        foreach (TurretStruct turret in currentTarget)
            turret.ai.frozen = false;
    }

    public override void Kill()
    {
        foreach (TurretStruct turret in currentTarget)
            turret.ai.frozen = false;

        objectiveOrder[nextTargetIndex].frozen = false;
        base.Kill();
    }
}