using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Suicider : EnemyAI
{
    public GameObject deathEffect2;
    public float tempdistancecheck;
    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex, Enemies enemyType)
    {
        base.Initialize(newList, newHealthThreshHold, newMaster, newSpawner, newWaveIndex, enemyType);
    }

    public override void Movement()
    {
        objective = objectiveOrder[nextTargetIndex];

        if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
        {
            Debug.Log("Test target index upping Main!");
            nextTargetIndex++;
        }
        distance = Vector3.Distance(transform.position, objective.transform.position);
        StartCoroutine(IMovement());
    }

       public bool timeDone = false;
    public bool zeroed = false;
    public bool goForth = false;
    public bool newBool = false;
    private IEnumerator IMovement()
    {
        tempdistancecheck = distance;
        if(distance < 20f && !zeroed)
        {
            speed = 0;
            tmpSpeed = 0;
            privateSpeed = 0;
            transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, 0);
            zeroed = true;
            print("step one");
        }
        if (zeroed)
        {
            transform.RotateAround(objective.transform.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (originalSpeed) * Time.deltaTime);
            if (!newBool)
            {
                newBool = true;
                yield return new WaitForSeconds(2f);
                timeDone = true;
            }
            print("step two");
        }
        transform.LookAt(objective.transform);
        if (timeDone == true && goForth == false)
        {
            gameObject.transform.RotateAround(objective.transform.position, Vector3.forward, 0);
            speed = originalSpeed + 5;
            privateSpeed = speed / 2;
            tmpSpeed = speed;
            goForth = true;
            print("I make 3 speed");
        }

        //BUGGEN KANSKE ÄR ATT DEN ÄR IN RANGE NÄR DEN BYTER TARGET OCH DÅ FUCKAR UT OCH INTE VILL ATTAKERA
        if (distance > 21f && zeroed == true)
        {
            StopAllCoroutines();
            zeroed = false;
            newBool = false;
            timeDone = false;
            goForth = false;
            speed = originalSpeed;
            privateSpeed = speed / 2;
            tmpSpeed = speed;
            print("I make back to normal 4 ");
        }

        transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);

        if (distance < 1.5f)
        {
            objective.TakeDamage(gun.damage, home);
            Instantiate(deathEffect, transform.position, transform.rotation);
            Instantiate(deathEffect2, transform.position, transform.rotation);
            Manager.Instance.tAe.killCount++;
            StopAllCoroutines();
            Kill();
        }
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (distance < 2f)
                objective.TakeDamage(gun.damage, home);

            Manager.Instance.tAe.killCount++;
            Instantiate(deathEffect, transform.position, transform.rotation);
            Instantiate(deathEffect2, transform.position, transform.rotation);
            StopAllCoroutines();
            Kill();
        }
    }
    public override void ShootingBehaviour()
    {
        
    }
}