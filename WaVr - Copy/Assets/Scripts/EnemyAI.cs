using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GameObject player;
    Transform lookingPos;
    SpaceGun gun;
    UnparentSound ups;
    public float speed = 1;

    RaycastHit hit;
    RaycastHit playerHit;
    int layerMask = 1 << 10;

    [SerializeField] private int health = 5;

    [HideInInspector] public bool tooClose = false;
    [HideInInspector] public Transform pushAwayFrom;

    private float range = 25;

    private void Start()
    {
        player = GameObject.Find("VRTK_SDKMANAGER");
        lookingPos = GameObject.Find("HeadSetFollower").transform;
        gun = GetComponent<SpaceGun>();
        StartShooting();
        ups = GetComponentInChildren<UnparentSound>();
    }

    public void FixedUpdate()
    {
        MoveMent();
        Debug.DrawRay(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward * range, Color.red);
    }

    void MoveMent ()
    {
        transform.LookAt(lookingPos);

        if (Physics.Raycast(gun.ReturnMuzzle().position, gun.ReturnMuzzle().forward, out playerHit, range))
        {
            if (playerHit.collider.tag != "Player")
            {
                transform.position = Vector3.Lerp(transform.position, transform.up, Time.deltaTime * speed);
                return;
            }
        }

        var distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 10)
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * speed);
    }

    bool TooClose ()
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
            ups.UnParent();
            Manager.Instance.RemoveEnemie();
            Destroy(gameObject);
        }
    }
}