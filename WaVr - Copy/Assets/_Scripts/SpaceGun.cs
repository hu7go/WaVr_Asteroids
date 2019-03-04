using System.Collections;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    public enum BulletType
    {
        bullet,
        beam,
        freeze
    }
    [HideInInspector] public BulletType bulletType;
    [HideInInspector] public GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;
    [Tooltip("Damage dealt per bullet hit.")]
    [HideInInspector] public float damage = 1;
    [HideInInspector] public float fireRate = 1;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool spawnEffect = true;
    private bool canFire = true;
    private Quaternion muzzleOriginalRot;

    RaycastHit target;
    public LayerMask layerMask;

    private Vector3 fireDirection;
    [HideInInspector] public bool shoot = false;
    [HideInInspector] public bool freeze = false;

    LineRenderer lineRend;
    [Header("This is if the enemy uses beam stuffs!")]
    public Color colorFront;
    public Color colorBack;
    EnemyAI ai;

    AudioSource audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioSource>();
        muzzleOriginalRot = muzzle.rotation;
        ai = GetComponent<EnemyAI>();
        if (bulletType == BulletType.beam)
        {
            lineRend = GetComponent<LineRenderer>();
            lineRend.material.SetColor("_ColorBack", colorFront);
            lineRend.material.SetColor("_ColorFront", colorBack);
            lineRend.material.SetFloat("_ScrollSpeed", Random.Range(.3f, .4f));
        }
        if (bulletType == BulletType.freeze)
        {
            lineRend = GetComponent<LineRenderer>();
            lineRend.material.SetColor("_ColorBack", Color.blue);
            lineRend.material.SetColor("_ColorFront", Color.white);
            lineRend.material.SetFloat("_ScrollSpeed", Random.Range(.3f, .4f));
        }
    }

    private void Update()
    {
        if (bulletType == BulletType.beam)
        {
            if (shoot)
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
        if (bulletType == BulletType.freeze)
        {
            if (freeze)
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
    }

    public void Shoot (int waveIndex, EnemySpawnPoint enemyOrigin)
    {
        if (!canFire)
            return;

        switch (bulletType)
        {
            case BulletType.bullet:
                if (audioManager.isPlaying == false)
                    audioManager.Play();
                break;
            case BulletType.beam:
                if (!audioManager.isPlaying)
                    audioManager.Play();
                break;
            case BulletType.freeze:
                if (!audioManager.isPlaying)
                    audioManager.Play();
                break;
        }

        canFire = false;

        StartCoroutine(FireRate());

        fireDirection = muzzle.forward;

        switch (bulletType)
        {
            case BulletType.bullet:
                GameObject tmpBullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation * Quaternion.Euler(fireDirection));
                break;
            case BulletType.beam:
                break;
            case BulletType.freeze:
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
                ai.KilledTarget();
            }
        }
    }

    public void StopShooting()
    {
        if (audioManager.isPlaying)
            audioManager.Stop();
    }

    private IEnumerator FireRate ()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public float RetunrFireRate () => fireRate;

    public Transform ReturnMuzzle () => muzzle;
}