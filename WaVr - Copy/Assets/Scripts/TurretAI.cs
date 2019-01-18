﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretAI : MonoBehaviour
{
    [SerializeField]
    private GameObject tMuzzle,bullet,enemyParent,turretSphere;

    private int closest = 0;
    public Turret turretInfo;

    public List<EnemyAI> enemies;
    bool shooting = false;

    EnemyAI currentTarget;

	void Start ()
    {
        enemies = new List<EnemyAI>();
	}

    public void EnteredRange (EnemyAI newEnemy)
    {
        enemies.Add(newEnemy);
        if (enemies.Count > 1)
            enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, newEnemy.transform.position)).ToList();

        if (!shooting)
        {
            shooting = true;
            currentTarget = enemies[0];
            StartCoroutine(Shooting());
        }
    }

    private IEnumerator Shooting ()
    {
        while (shooting)
        {
            yield return new WaitForSeconds(turretInfo.attackSpeed);
            if (currentTarget == null)
                enemies.Remove(currentTarget);
            if (enemies.Count > 0)
                currentTarget = enemies[0];
            Shoot();
        }
    }

    private void Shoot ()
    {
        if (enemies.Count > 0)
        {
            if (currentTarget == null)
            {
                enemies.Remove(currentTarget);
                if (enemies.Count > 0)
                    currentTarget = enemies[0];
            }

            if (enemies.Count > 0)
                turretSphere.transform.LookAt(currentTarget.transform);
            currentTarget.TakeDamage(turretInfo.damage);
            if (currentTarget.ReturnHealth() <= 0)
            {
                enemies.Remove(currentTarget);
                Destroy(currentTarget.gameObject);
                if (enemies.Count > 0)
                    currentTarget = enemies[0];
                else
                {
                    shooting = false;
                    return;
                }
            }
            if (currentTarget == null)
            {
                enemies.RemoveAt(0);
                if (enemies.Count > 0)
                    currentTarget = enemies[0];
            }

            tMuzzle.transform.GetChild(0).gameObject.SetActive(true);
            MuzzleParticle();

            if (enemies.Count <= 0)
            {
                shooting = false;
            }
        }
    }

    private void MuzzleParticle()
    {
        StartCoroutine(StopThatParticle());
    }

    IEnumerator StopThatParticle()
    {
        yield return new WaitForSeconds(1.5f);
        tMuzzle.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ExitedRange(EnemyAI enemy)
    {
        enemies.Remove(enemy);
        if (currentTarget == null)
            enemies.RemoveAt(0);
        if (enemies.Count > 1)
            enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, enemy.transform.position)).ToList();
    }
}