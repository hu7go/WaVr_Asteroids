using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject deathEffect;

     public AsteroidHealth objective;
    protected SpaceGun gun;
    protected UnparentSound ups;
    [HideInInspector] public float speed = 1;

    protected RaycastHit hit;
    protected RaycastHit playerHit;
    protected RaycastHit tooCloseCast;
    protected Vector3 forward;
    public LayerMask layerMask;

    protected float health = 5;

    [HideInInspector] public bool tooClose = false;
    [HideInInspector] public Transform pushAwayFrom;
    [HideInInspector] public float range = 25;

    protected int randomNmbrX;
    protected int randomNmbrY;
    protected int randomNmbrZ;
    protected float privateSpeed;
    protected float tmpSpeed;
    protected float originalSpeed;
    protected bool freezing;
    [HideInInspector] public List<AsteroidHealth> objectiveOrder;

    protected float healthThreshHold;

     public bool seekAndDestroy = true;
    protected EnemySpawnPoint home;

    protected float distance;
    public int nextTargetIndex = 0;
    protected bool onTheWay = false;
    protected float stopDistance = 9;

    protected Spawner spawner;

    protected int waveIndex;

    private void Start()
    {
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
        gun.minFireRate = enemyType.minFireRate;
        gun.maxFireRate = enemyType.maxFireRate;
        gun.bulletType = enemyType.bulletType;
        gun.bulletPrefab = enemyType.bullet;
        speed = enemyType.speed;
        range = enemyType.range;
        stopDistance = enemyType.stopDistance;
        health = (int)enemyType.health;
        originalSpeed = speed;
        privateSpeed = speed / 2;
        tmpSpeed = speed;
        objectiveOrder = newList;
        healthThreshHold = newHealthThreshHold;
        home = newMaster;
        spawner = newSpawner;
        waveIndex = newWaveIndex;

        objective = objectiveOrder[nextTargetIndex];
    }

    public virtual void Update()
    {
        Movement();

        ShootingBehaviour();

        if (Manager.Instance.healthPercent <= healthThreshHold || home.damageDonePercent >= 100 - healthThreshHold + 1)
        {
            //If max health is low enough
            //If the damage they have done is high enough
            GoHome();
        }
    }

    public virtual void ShootingBehaviour ()
    {
        if (distance <= range && gun.shoot == false)
        {
            gun.shoot = true;
            gun.StartShooting(waveIndex, home, objective.GetComponent<AsteroidHealth>());
        }
        else if (distance > range && gun.shoot == true)
        {
            gun.shoot = false;
            gun.StopShooting();
        }
    }

    public virtual void Movement()
    {
        if (seekAndDestroy == true)
        {
            objective = objectiveOrder[nextTargetIndex];

            if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
            {
                Debug.Log("Test target index upping Main!");
                nextTargetIndex++;
            }

            distance = Vector3.Distance(transform.position, objective.transform.position);

            if (distance > range)
            {
                freezing = false;
                gun.freeze = false;
            }
            else
            {
                gun.freeze = true;
            }

            //Stops a certain distance away from the target!
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
        }
        else
        {
            //If seekAndDestroy is false they go back to there home portal!
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

    bool waiting = false;

    public virtual void GoHome()
    {
        seekAndDestroy = false;
        objective = home.GetComponent<AsteroidHealth>();
    }

    //from turrets
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Manager.Instance.tAe.killCount++;
            Instantiate(deathEffect, transform.position, transform.rotation);
            Kill();
        }
    }

    public virtual void Kill()
    {
        ups.UnParent();

        Manager.Instance.waves[waveIndex].enemies.Remove(this);
        Manager.Instance.RemovedEnemy(waveIndex);
        spawner.RemoveEnemie(this);
        Destroy(gameObject);
    }

    public virtual void KilledTarget()
    {

    }

    public float ReturnHealth() => health;

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