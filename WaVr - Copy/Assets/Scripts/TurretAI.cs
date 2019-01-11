using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAI : MonoBehaviour {
    [SerializeField]
    private GameObject tMuzzle,bullet,enemyParent;

    private Ray range;
    private List<float> distance;
    private List<GameObject> enemiesList;

    public Turret turretInfo;
	void Start () {
        InvokeRepeating("Shoot", 3, turretInfo.attackSpeed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && enemiesList != null)
        {
            enemiesList.Remove(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesList.Add(other.gameObject);
        }
    }

    private void Shoot()
    {
        if (enemiesList != null)
            for (int i = 0; i < enemiesList.Count; i++)
            {
                float[] nums = new float[];

                distance.Add(Vector3.Distance(tMuzzle.transform.position, enemiesList[i].transform.position));
            }
        // distance.Min()
    }
}