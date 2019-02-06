using System.Collections;
using Unity.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject deathEffect;

    GameObject objective;
    Transform lookingPos;
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

    private void Start()
    {
        objective = Manager.Instance.objective;
        lookingPos = GameObject.Find("HeadSetFollower").transform;   //is this one needed?
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

    public void FixedUpdate()
    {
        Movement();
        Debug.DrawRay(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, Color.red);
    }

    void Movement ()
    {
        transform.LookAt(objective.transform); 

        var distance = Vector3.Distance(transform.position, objective.transform.position);

        //Stops a certain distance away from the target!
        if (distance > 9)
        {
            if (distance < 25)
            {
                tmpSpeed = privateSpeed;
                transform.RotateAround(objective.transform.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed * 2) * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
        }
        if (distance <= 9)
            transform.RotateAround(objective.transform.position, new Vector3(randomNmbrX, randomNmbrY, randomNmbrZ), (tmpSpeed) * Time.deltaTime);
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
            StopCoroutine(Shoot());
            ups.UnParent();

            Instantiate(deathEffect, transform.position, transform.rotation);
            Manager.Instance.RemoveEnemy();
            Destroy(gameObject);
        }
    }

    public int ReturnHealth ()
    {
        return health;
    }
}