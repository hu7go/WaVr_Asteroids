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

    EnemyAI currentTarget;

	void Start ()
    {
        enemies = new List<EnemyAI>();
	}

    public void EnteredRange (EnemyAI newEnemy)
    {
        enemies.Add(newEnemy);
        if (enemies.Count > 1)
            enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, newEnemy.transform.position)).ToList();

        if (!shooting)
        {
            shooting = true;
            currentTarget = enemies[0];
            StartCoroutine(Shooting());
        }
    }

    private IEnumerator Shooting ()
    {
        while (shooting)
        {
            yield return new WaitForSeconds(turretInfo.attackSpeed);
            Shoot();
        }
    }

    private void Shoot ()
    {
        Debug.Log(currentTarget.name, currentTarget);
        currentTarget.TakeDamage(turretInfo.damage);
        if (currentTarget.ReturnHealth() <= 0)
        {
            enemies.Remove(currentTarget);
            Destroy(currentTarget.gameObject);
            if (enemies.Count > 0)
                currentTarget = enemies[0];
        }

        //Spawn laser beam!
        
        if (enemies.Count <= 0)
        {
            shooting = false;
        }
    }

    public void ExitedRange(EnemyAI enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count > 1)
            enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, enemy.transform.position)).ToList();
    }
}