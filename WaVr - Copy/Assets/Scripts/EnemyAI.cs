using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GameObject objective;
    Transform lookingPos;
    SpaceGun gun;
    UnparentSound ups;
    public float speed = 1;

    RaycastHit hit;
    RaycastHit playerHit;
    int layerMask = 1 << 10;

    [SerializeField] private int health = 5;

    [HideInInspector] public bool tooClose = false;  //These two aren't used atm and needs to be used so that they aren't inside each other
    [HideInInspector] public Transform pushAwayFrom;

    private float range = 25;

    private void Start()
    {
        objective = GameObject.Find("VRTK_SDKMANAGER");  //needs to change to actual objective
        lookingPos = GameObject.Find("HeadSetFollower").transform;   //is this one needed?
        gun = GetComponent<SpaceGun>();
        StartShooting();  // Start shooting when in range of objective?
        ups = GetComponentInChildren<UnparentSound>();
    }

    public void FixedUpdate()
    {
        Movement();
        Debug.DrawRay(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, Color.red);
    }

    void Movement ()  // add some swaying?
    {
        transform.LookAt(lookingPos); // look at the objective instead of something more?

        if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward, out playerHit, range))
        {
            if (playerHit.collider.tag != "Player") // objective
            {
                transform.position = Vector3.Lerp(transform.position, transform.up, Time.deltaTime * speed);
                return;
            }
        }

        var distance = Vector3.Distance(transform.position, objective.transform.position);

        if (distance > 10)
            transform.position = Vector3.Lerp(transform.position, objective.transform.position, Time.deltaTime * speed);
    }

    bool TooClose () //make it work
    {
        if (tooClose)
            return true;
        else
            return false;
    }

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

    public void TakeDamage (int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StopAllCoroutines();
            ups.UnParent();
            Manager.Instance.RemoveEnemie();
            Destroy(gameObject);
        }
    }
}