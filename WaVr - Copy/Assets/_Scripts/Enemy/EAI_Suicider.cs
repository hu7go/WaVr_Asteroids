using System.Collections.Generic;
using UnityEngine;

public class EAI_Suicider : EnemyAI
{
    public GameObject deathEffect2;

    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex, Enemies enemyType)
    {
        base.Initialize(newList, newHealthThreshHold, newMaster, newSpawner, newWaveIndex, enemyType);
    }

    public override void Movement()
    {
        base.Movement();
        if(distance < 1.5f)
        {
            objective.TakeDamage(gun.damage, home);
            Instantiate(deathEffect, transform.position, transform.rotation);
            Instantiate(deathEffect2, transform.position, transform.rotation);
            Manager.Instance.tAe.killCount++;
            Kill();
        }
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (distance < 2f)
                objective.TakeDamage(gun.damage, home);

            Manager.Instance.tAe.killCount++;
            Instantiate(deathEffect, transform.position, transform.rotation);
            Instantiate(deathEffect2, transform.position, transform.rotation);
            Kill();
        }
    }
}