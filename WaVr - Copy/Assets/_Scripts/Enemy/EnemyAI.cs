using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject deathEffect;

    [HideInInspector] public Transform objective;
    protected SpaceGun gun;
    protected UnparentSound ups;
    [HideInInspector] public float speed = 1;

    protected RaycastHit hit;
    protected RaycastHit playerHit;
    protected RaycastHit tooCloseCast;
    protected Vector3 forward;
    public LayerMask layerMask;

    protected int health = 5;

    [HideInInspector] public bool tooClose = false;
    [HideInInspector] public Transform pushAwayFrom;
    [HideInInspector] public float range = 25;

    protected int randomNmbrX;
    protected int randomNmbrY;
    protected int randomNmbrZ;
    protected float privateSpeed;
    protected float tmpSpeed;
    protected bool freezing;
    [HideInInspector] public List<AsteroidHealth> objectiveOrder;

    protected float healthThreshHold;

    [HideInInspector] public bool seekAndDestroy = true;
    protected EnemySpawnPoint home;

    protected float distance;
    public int nextTargetIndex = 0;
    protected bool onTheWay = false;
    protected float stopDistance = 9;

    protected Spawner spawner;

    protected int waveIndex;

    private void Start()
    {
        StartShooting();
        ups = GetComponentInChildren<UnparentSound>();

        randomNmbrX = Random.Range(-10, 10);
        randomNmbrY = Random.Range(-10, 10);
        randomNmbrZ = Random.Range(-10, 10);


        health += Manager.Instance.tAe.waveCount;
    }

    public virtual void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex, Enemies enemyType)
    {
        gun = GetComponent<SpaceGun>();

        gun.damage = enemyType.damage;
        gun.fireRate = enemyType.fireRate;
        gun.bulletType = enemyType.bulletType;
        gun.bulletPrefab = enemyType.bullet;
        speed = enemyType.speed;
        range = enemyType.range;
        stopDistance = enemyType.stopDistance;
        health = (int)enemyType.health;

        privateSpeed = speed / 2;
        tmpSpeed = speed;
        objectiveOrder = newList;
        healthThreshHold = newHealthThreshHold;
        home = newMaster;
        spawner = newSpawner;
        waveIndex = newWaveIndex;

        objective = objectiveOrder[nextTargetIndex].transform;
    }

    public void Update()
    {
        Movement();

        if (Manager.Instance.healthPercent <= healthThreshHold || home.damageDonePercent >= 100 - healthThreshHold + 1)
        {
            //If max health is low enough
            //If the damage they have done is high enough
            GoHome();
        }
    }

    public void GoHome()
    {
        seekAndDestroy = false;
        objective = home.transform;
    }

    public virtual void Movement()
    {
        if (seekAndDestroy == true)
        {
            objective = objectiveOrder[nextTargetIndex].transform;

            if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
            {
                nextTargetIndex++;
            }

            distance = Vector3.Distance(transform.position, objective.position);

            if (distance > range)
            {
                freezing = false;
                gun.shoot = false;
                gun.freeze = false;
            }
            else
            {
                gun.shoot = true;
                gun.freeze = true;
            }

            //Stops a certain distance away from the target!
            if (distance > stopDistance)
            {
                onTheWay = true;
                if (distance < range)
                {
                    tmpSpeed = privateSpeed;
                    transform.RotateAround(objective.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed * 2) * Time.deltaTime);
                }
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
            }
            if (distance <= stopDistance)
            {
                onTheWay = false;
                transform.RotateAround(objective.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed) * Time.deltaTime);
            }
        }
        else
        {
            //If seekAndDestroy is false they go back to there home portal!
            distance = Vector3.Distance(transform.position, objective.position);

            if (distance < 2)
            {
                Kill();
                home.Over();
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
        }

        transform.LookAt(objective, Manager.Instance.GetWorldAxis());
    }

    //Shoot at objective
    public void StartShooting()
    {
        if (seekAndDestroy)
        {
            if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, out hit, range, layerMask))
                gun.Shoot(waveIndex, home);
        }
        StartCoroutine(Shoot());
    }

    protected IEnumerator Shoot()
    {
        yield return new WaitForSeconds(Random.Range(gun.RetunrFireRate(), gun.RetunrFireRate() + 2));
        StartShooting();
    }

    //from turrets
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Manager.Instance.tAe.killCount++;
            Instantiate(deathEffect, transform.position, transform.rotation);
            Kill();
        }
    }

    protected void Kill()
    {
        StopCoroutine(Shoot());
        ups.UnParent();

        Manager.Instance.waves[waveIndex].enemies.Remove(this);
        Manager.Instance.RemovedEnemy(waveIndex);
        spawner.RemoveEnemie(this);
        Destroy(gameObject);
    }

    public virtual void KilledTarget()
    {

    }

    public int ReturnHealth() => health;

    public virtual void SetPath(List<AsteroidHealth> newPath)
    {
        if (objectiveOrder[nextTargetIndex].asteroid.alive == true)
        {
            nextTargetIndex--;
            if (nextTargetIndex < 0)
                nextTargetIndex = 0;
        }
        objectiveOrder = newPath;
    }

    //Debuging stuffs!

    [HideInInspector] public bool drawPath = false;

    protected void OnDrawGizmos()
    {
        if (drawPath)
        {
            for (int i = 0; i < objectiveOrder.Count; i++)
            {
                if (i - 1 >= 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(objectiveOrder[i - 1].asteroid.postition, objectiveOrder[i].asteroid.postition);
                }
            }
        }
    }
}