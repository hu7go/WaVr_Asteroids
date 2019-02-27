using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject deathEffect;

    Transform objective;
    SpaceGun gun;
    UnparentSound ups;
    public float speed = 1;

    RaycastHit hit;
    RaycastHit playerHit;
    RaycastHit tooCloseCast;
    Vector3 forward;
    public LayerMask layerMask;

    [SerializeField] private int health = 5;

    [HideInInspector] public bool tooClose = false;
    [HideInInspector] public Transform pushAwayFrom;
    [ReadOnly]
    private float range = 25;

    private int randomNmbrX;
    private int randomNmbrY;
    private int randomNmbrZ;
    private float privateSpeed;
    private float tmpSpeed;

    private List<AsteroidHealth> objectiveOrder;

    [SerializeField] private float healthThreshHold;

    [HideInInspector] public bool seekAndDestroy = true;
    private EnemySpawnPoint home;

    private float distance;

    Spawner spawner;

    int waveIndex;

    private void Start()
    {
        gun = GetComponent<SpaceGun>();
        StartShooting();  // Start shooting when in range of objective?
        ups = GetComponentInChildren<UnparentSound>();

        randomNmbrX = Random.Range(-10, 10);
        randomNmbrY = Random.Range(-10, 10);
        randomNmbrZ = Random.Range(-10, 10);

        privateSpeed = speed / 2;
        tmpSpeed = speed;

        health += Manager.Instance.tAe.waveCount;
    }

    public virtual void Initialize (List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex)
    {
        objectiveOrder = newList;
        healthThreshHold = newHealthThreshHold;
        home = newMaster;
        spawner = newSpawner;
        waveIndex = newWaveIndex;
    }

    public void FixedUpdate()
    {
        Movement();

        if (Manager.Instance.healthPercent <= healthThreshHold)
        {
            GoHome();
        }
    }

    public void GoHome ()
    {
        seekAndDestroy = false;
        objective = home.transform;
    }

    void Movement ()
    {
        if (seekAndDestroy == true)
        {
            objective = objectiveOrder[0].transform;

            distance = Vector3.Distance(transform.position, objective.position);

            //Stops a certain distance away from the target!
            if (distance > 9)
            {
                if (distance < 25)
                {
                    tmpSpeed = privateSpeed;
                    transform.RotateAround(objective.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed * 2) * Time.deltaTime);
                }
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
            }
            if (distance <= 9)
                transform.RotateAround(objective.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed) * Time.deltaTime);
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

        transform.LookAt(objective);
    }

    //Shoot at objective
    public void StartShooting ()
    {
        if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, out hit, range, layerMask))
            gun.Shoot(waveIndex, home);
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot ()
    {
        yield return new WaitForSeconds(Random.Range(gun.RetunrFireRate(), gun.RetunrFireRate() + 2));
        StartShooting();
    }

    //from turrets
    public void TakeDamage (int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Manager.Instance.tAe.killCount++;
            Instantiate(deathEffect, transform.position, transform.rotation);
            Kill();
        }
    }

    void Kill()
    {
        StopCoroutine(Shoot());
        ups.UnParent();

        Manager.Instance.waves[waveIndex].enemies.Remove(this);
        Manager.Instance.RemoveEnemy(waveIndex);
        spawner.RemoveEnemie(this);
        Destroy(gameObject);
    }

    public int ReturnHealth () => health;

    public void SetPath (List<AsteroidHealth> newPath) => objectiveOrder = newPath;

    //Debuging stuffs!

    [HideInInspector] public bool drawPath = false;

    private void OnDrawGizmos()
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