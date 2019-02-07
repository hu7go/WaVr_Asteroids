using System.Collections;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform muzzle;
    [Tooltip("Damage dealt per bullet hit.")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool spawnEffect = true;
    private bool canFire = true;
    private Quaternion muzzleOriginalRot;

    RaycastHit target;
    public LayerMask layerMask;

    private Vector3 fireDirection;

    private void Start()
    {
        muzzleOriginalRot = muzzle.rotation;
    }

    public void Shoot ()
    {
        if (!canFire)
            return;

        canFire = false;

        StartCoroutine(FireRate());

        fireDirection = muzzle.forward;

        //GameObject tmpBullet = Instantiate(bullet, muzzle.position, muzzle.rotation * Quaternion.Euler(fireDirection));

        //Debug.DrawRay(muzzle.position, tmpBullet.transform.forward * 150, Color.cyan, 1f);


        if (Physics.Raycast(muzzle.position, muzzle.transform.forward, out target, 150, layerMask))
        {
            if (spawnEffect)
            {
                GameObject newEffect = Instantiate(hitEffect, target.point, target.collider.transform.rotation);
                Destroy(newEffect, 1f);
            }

            Debug.Log("Test hit!");
            target.collider.GetComponent<AsteroidHealth>().TakeDamage(damage);
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