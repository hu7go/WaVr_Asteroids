using System.Collections;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    public enum Shooter
    {
        turret,
        enemy
    }

    public Shooter shooter = Shooter.turret;

    [Space(20)]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform muzzle;
    [Tooltip("Damage dealt per bullet hit.")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool animationsOn = true;
    [SerializeField] private bool spawnEffect = true;
    [SerializeField] private bool spread = false;
    [SerializeField] public bool isGrabbed;
    //[SerializeField] private bool automaticFire = false;
    private bool canFire = true;
    private Animator animController;
    private Quaternion muzzleOriginalRot;

    RaycastHit target;
    int layerMask = 1 << 11;

    private Vector3 fireDirection;

    private void Start()
    {
        layerMask = ~layerMask;

        muzzleOriginalRot = muzzle.rotation;

        animController = GetComponent<Animator>();
    }

    public void Shoot ()
    {
        if (!canFire)
            return;

        canFire = false;

        StartCoroutine(FireRate());

        if (animationsOn)
            animController.SetTrigger("Fire");

        if (spread)
            fireDirection = muzzle.forward + (Random.insideUnitSphere * .3f) * 50;
        else
            fireDirection = muzzle.forward;

        GameObject tmpBullet = Instantiate(bullet, muzzle.position, muzzle.rotation * Quaternion.Euler(fireDirection));

        //Debug.DrawRay(muzzle.position, tmpBullet.transform.forward * 150, Color.cyan, 1f);

        if (Physics.Raycast(muzzle.position, tmpBullet.transform.forward, out target, 150, layerMask))
        {
            if (spawnEffect)
            {
                GameObject newEffect = Instantiate(hitEffect, target.point, target.collider.transform.rotation);
                Destroy(newEffect, 1f);
            }

            if (target.collider.tag == "Objective")
            {
                target.collider.GetComponent<ObjectiveHP>().TakeDamage(damage);
                return;
            }


            if (shooter == Shooter.turret)
                if (target.collider.tag == "Enemy")
                    target.collider.GetComponent<EnemyAI>().TakeDamage(damage);
        }
    }

    private IEnumerator FireRate ()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public float RetunrFireRate ()
    {
        return fireRate;
    }

    public Transform ReturnMuzzle ()
    {
        return muzzle;
    }
}