using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretAI : MonoBehaviour
{
    [SerializeField]
    private GameObject tMuzzle, bullet, enemyParent, turretSphere;
    private AudioSource audi;
    private int closest = 0;
    public Turret turretInfo;

    public List<EnemyAI> enemies;
    bool shooting = false;
    public bool frozen = false;

    EnemyAI currentTarget;

	void Start ()
    {
        enemies = new List<EnemyAI>();
        audi = gameObject.GetComponent<AudioSource>();
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
            if (currentTarget == null)
                enemies.Remove(currentTarget);
            if (enemies.Count > 0)
                currentTarget = enemies[0];
            Shoot();
        }
    }

    private void Shoot ()
    {
        //This should stop the turret from trying to shot at nothing and instead clear the list and start over!
        if (enemies.Count > 0 && currentTarget == null)
        {
            enemies.Clear();
            return;
        }
        //

        for (int i = enemies.Count -1; i > -1; i--)
            if (enemies[i] == null)
                enemies.RemoveAt(i);

        if (enemies.Count > 0 && frozen == false)
        {
            if (currentTarget == null)
            {
                enemies.RemoveAt(0);
                currentTarget = enemies[0];
            }

            turretSphere.transform.LookAt(currentTarget.transform);
            //The shooting part!
            currentTarget.TakeDamage(turretInfo.damage);
            //
            if(!audi.isPlaying)
                audi.Play();
            if (currentTarget.ReturnHealth() <= 0)
            {
                enemies.Remove(currentTarget);
                if (enemies.Count == 0)
                {
                    shooting = false;
                    return;
                }
            }
            if (currentTarget == null)
                enemies.RemoveAt(0);

            ObjectPooler.Instance.SpawnFromPool("TurretBullet", tMuzzle.transform.position, tMuzzle.transform.rotation);

            if (enemies.Count <= 0)
                shooting = false;
        }
        else
            shooting = false;
    }

    private void MuzzleParticle() => StartCoroutine(StopThatParticle());

    IEnumerator StopThatParticle()
    {
        yield return new WaitForSeconds(1.5f);
        tMuzzle.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ExitedRange(EnemyAI enemy)
    {
        enemies.Remove(enemy);
        if (currentTarget == null)
            enemies.RemoveAt(0);
        if (enemies.Count > 1)
            enemies = enemies.OrderBy(x => Vector3.Distance(tMuzzle.transform.position, enemy.transform.position)).ToList();
    }
}