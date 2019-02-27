using System.Collections;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    public enum BulletType
    {
        bullet,
        beam
    }
    public BulletType bulletType;
    public GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;
    [Tooltip("Damage dealt per bullet hit.")]
    [SerializeField] private float damage = 1;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool spawnEffect = true;
    private bool canFire = true;
    private Quaternion muzzleOriginalRot;

    RaycastHit target;
    public LayerMask layerMask;

    private Vector3 fireDirection;
    private bool shoot = false;

    LineRenderer lineRend;
    public Color colorFront;
    public Color colorBack;
    EnemyAI ai;

    private void Start()
    {
        muzzleOriginalRot = muzzle.rotation;
        ai = GetComponent<EnemyAI>();
        if (bulletType == BulletType.beam)
        {
            lineRend = GetComponent<LineRenderer>();
            lineRend.material.SetColor("_ColorBack", colorFront);
            lineRend.material.SetColor("_ColorFront", colorBack);
            lineRend.material.SetFloat("_ScrollSpeed", Random.Range(.3f, .4f));
        }
    }

    private void Update()
    {
        if (bulletType == BulletType.beam && shoot)
        {
            lineRend.SetPosition(0, muzzle.position);
            lineRend.SetPosition(1, ai.objective.position);
        }
        else
        {
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, transform.position);
        }
    }

    public void Shoot (int waveIndex, EnemySpawnPoint enemyOrigin)
    {
        if (!canFire)
            return;

        canFire = false;

        StartCoroutine(FireRate());

        fireDirection = muzzle.forward;

        switch (bulletType)
        {
            case BulletType.bullet:
                GameObject tmpBullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation * Quaternion.Euler(fireDirection));
                break;
            case BulletType.beam:
                shoot = true;
                break;
        }

        if (Physics.Raycast(muzzle.position, muzzle.transform.forward, out target, 150, layerMask))
        {
            AsteroidHealth targetHealth = target.collider.GetComponent<AsteroidHealth>();

            if (spawnEffect)
            {
                GameObject newEffect = Instantiate(hitEffect, target.point, target.collider.transform.rotation);
                Destroy(newEffect, 1f);
            }
            targetHealth.TakeDamage(damage, enemyOrigin);
            if (targetHealth.asteroid.health <= 0)
            {
                shoot = false;
            }
        }
    }

    public void StopShooting()
    {
        shoot = false;
    }

    private IEnumerator FireRate ()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public float RetunrFireRate () => fireRate;

    public Transform ReturnMuzzle () => muzzle;
}