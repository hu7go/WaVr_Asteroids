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
    int objIndex = 0;

    private float healthThreshHold;

    private bool seekAndDestroy = true;
    private Transform home;

    private float distance;

    Spawner spawner;

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

        health += Manager.Instance.turretsAndEnemies.waveCounter;
    }

    public void Initialize (List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner)
    {
        objectiveOrder = newList;
        healthThreshHold = newHealthThreshHold;
        home = newMaster.transform;
        spawner = newSpawner;
    }

    public void FixedUpdate()
    {
        CheckHealthThreshHold();
        Movement();
        //Debug.DrawRay(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, Color.red);
    }

    private void CheckHealthThreshHold()
    {
        if (Manager.Instance.healthPercent <= healthThreshHold)
        {
            seekAndDestroy = false;
            objective = home;
        }
    }

    void Movement ()
    {
        //This needs to check if the current objective target is still alive and if not change to the next one in the list that is alive!
        //! might not work right now, needs testing!

        if (seekAndDestroy == true)
        {
            if (objectiveOrder[objIndex].asteroid.alive == true)
            {
                objective = objectiveOrder[objIndex].transform;
            }
            else
            {
                objIndex++;
                objective = objectiveOrder[objIndex].transform;
            }
            //
            spawner.CheckForNewPath();

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
            distance = Vector3.Distance(transform.position, objective.position);

            if (distance < 2)
            {
                Kill();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
            }
        }

        transform.LookAt(objective);
    }

    //Shoot at objective
    public void StartShooting ()
    {
        if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, out hit, range, layerMask))
            gun.Shoot();
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
            Instantiate(deathEffect, transform.position, transform.rotation);
            Kill();
        }
    }

    void Kill()
    {
        StopCoroutine(Shoot());
        ups.UnParent();

        Manager.Instance.RemoveEnemy();
        Destroy(gameObject);
    }

    public int ReturnHealth ()
    {
        return health;
    }

    public void SetPath (List<AsteroidHealth> newPath)
    {
        objectiveOrder = newPath;
    }
}