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
    [HideInInspector] public float minFireRate = 1;
    [HideInInspector] public float maxFireRate = 1;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool spawnEffect = true;
    private bool canFire = true;
    private Quaternion muzzleOriginalRot;

    public LayerMask layerMask;

    private Vector3 fireDirection;
    [HideInInspector] public bool shoot = false;
    [HideInInspector] public bool freeze = false;

    LineRenderer lineRend;
    [Header("This is if the enemy uses beam stuffs!")]
    public Color colorFront;
    public Color colorBack;
    public bool beamTowardsAsteroid = true;
    EnemyAI ai;

    private EAI_Fetcher healthStealer;

    AudioSource audioManager;

    int index;
    EnemySpawnPoint enemyOrigin;
    AsteroidHealth target;

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
        if (bulletType == BulletType.beam || bulletType == BulletType.freeze)
        {
            if (shoot)
            {
                if (beamTowardsAsteroid)
                {
                    lineRend.SetPosition(0, muzzle.position);
                    lineRend.SetPosition(1, ai.objective.position);
                }
                else
                {
                    lineRend.SetPosition(0, ai.objective.position);
                    lineRend.SetPosition(1, muzzle.position);
                }
            }
            else
            {
                lineRend.SetPosition(0, transform.position);
                lineRend.SetPosition(1, transform.position);
            }
        }
    }

    public void StartShooting (int waveIndex, EnemySpawnPoint origin, AsteroidHealth newTarget)
    {
        index = waveIndex;
        enemyOrigin = origin;
        target = newTarget;

        Shoot();
    }

    public void StartShooting(int waveIndex, EnemySpawnPoint origin, AsteroidHealth newTarget, EAI_Fetcher newAi)
    {
        index = waveIndex;
        enemyOrigin = origin;
        target = newTarget;

        healthStealer = newAi;

        Shoot();
    }

    public void Shoot ()
    {
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

        target.TakeDamage(damage, enemyOrigin);
        if (target.asteroid.health <= 0)
        {
            ai.KilledTarget();
        }

        //If the ai is a fetcher!
        if (healthStealer != null)
            healthStealer.stolenHealth += damage;
        //

        StartCoroutine(FireRate());
    }

    public void StopShooting()
    {
        if (audioManager.isPlaying)
            audioManager.Stop();

        StopCoroutine(FireRate());
    }

    private IEnumerator FireRate ()
    {
        yield return new WaitForSeconds(Random.Range(minFireRate, maxFireRate));
        Shoot();
    }

    public float ReturnFireRate () => minFireRate;

    public Transform ReturnMuzzle () => muzzle;
}