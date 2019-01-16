using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretAI : MonoBehaviour
{
    [SerializeField]
    private GameObject tMuzzle,bullet,enemyParent;

    private Ray range;
    private List<GameObject> enemiesList;

    private int closest = 0;
    public Turret turretInfo;


    public List<EnemyAI> enemies;
    bool shooting = false;

	void Start ()
    {
        enemies = new List<EnemyAI>();

        //InvokeRepeating("Calculate", 3, turretInfo.attackSpeed);
	}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        enemiesList.Add(other.gameObject);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Enemy") && enemiesList != null)
    //    {
    //        enemiesList.Remove(other.gameObject);
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Enemy") && !enemiesList.Contains(other.gameObject))
    //    {
    //        enemiesList.Add(other.gameObject);
    //    }
    //}

    private void Calculate()
    {
        if (enemiesList == null || enemiesList.Count == 0)
            return;
        if (enemiesList.Count > 1)
        {
            for (int i = 0; i < enemiesList.Count; i++)
            {
                if(Vector3.Distance(enemiesList[i].transform.position, tMuzzle.transform.position) < Vector3.Distance(enemiesList[closest].transform.position, tMuzzle.transform.position))
                    closest = i;
                print(enemiesList[closest] + "is closest");
            }
        }
        Shoot();
    }

    private void Shoot()
    {
        //tMuzzle.transform.LookAt(enemiesList[closest].transform.position);
        Quaternion newRot = Quaternion.LookRotation(enemiesList[closest].transform.position);
        tMuzzle.transform.rotation = Quaternion.Slerp(tMuzzle.transform.rotation, newRot, turretInfo.rotationSpeed);
        //Do shooting things
    }

    public void EnteredRange (EnemyAI newEnemy)
    {
        enemies.Add(newEnemy);
        enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, newEnemy.transform.position)).ToList();

        if (!shooting)
        {
            StartCoroutine(Shooting(newEnemy));
        }
    }

    public void ExitedRange (EnemyAI enemy)
    {
        enemies.Remove(enemy);
    }

    private IEnumerator Shooting (EnemyAI target)
    {
        while (shooting)
        {
            Shoot(target);
            yield return new WaitForSeconds(turretInfo.attackSpeed);
        }
    }

    private void Shoot (EnemyAI target)
    {
        target.TakeDamage(turretInfo.damage);
        if (target.ReturnHealth() <= 0)
        {
            enemies.Remove(target);
        }

        //Spawn laser beam!

        if (enemies.Count <= 0)
        {
            shooting = false;
        }
    }
}