using System.Collections;
using Unity.Collections;
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
    RaycastHit tooCloseCast;
    Vector3 forward;
    public LayerMask layerMask;

    [SerializeField] private int health = 5;

    [HideInInspector] public bool tooClose = false;
    [HideInInspector] public Transform pushAwayFrom;
    [ReadOnly]
    private float range = 25;

    private void Start()
    {
        objective = Manager.Instance.referenceTD;
        lookingPos = GameObject.Find("HeadSetFollower").transform;   //is this one needed?
        gun = GetComponent<SpaceGun>();
        StartShooting();  // Start shooting when in range of objective?
        ups = GetComponentInChildren<UnparentSound>();
    }

    public void FixedUpdate()
    {
        Movement();
        TooClose();
        Debug.DrawRay(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, Color.red);
    }

    void Movement ()  // add some swaying?
    {
        transform.LookAt(objective.transform); 

        var distance = Vector3.Distance(transform.position, objective.transform.position);

        if (distance > 7)
            transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, speed * Time.deltaTime);
        else
        {
            //TODO Make it so enemies wont clump up at the end!
        }
    }

    private void TooClose () //make it work
    {
        //forward = transform.TransformDirection(Vector3.forward);
        //if (Physics.Raycast(transform.position, forward, out tooCloseCast, 5))
        //{
        //    if (tooCloseCast.collider.CompareTag("Enemy"))
        //    {
        //        tooClose = true;
        //    }
        //}
        //if (tooClose)
        //{

        //    //MAKE IT MOVE
        //    transform.Rotate(Vector3.up *10);
        //    tooClose = false;
        //}            
    }
            //Shoot at objective
    public void StartShooting ()
    {
        if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, out hit, range, layerMask))
            gun.Shoot();
        //StartCoroutine(Shoot());
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
            Manager.Instance.RemoveEnemie();
        }
    }

    public int ReturnHealth ()
    {
        return health;
    }
}