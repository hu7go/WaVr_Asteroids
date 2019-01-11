using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAI : MonoBehaviour {
    [SerializeField]
    private GameObject tMuzzle,bullet,enemyParent;

    private Ray range;
    private List<float[][]> distance;
    private float[][] distanceID;
    private List<GameObject> enemiesList;

    public Turret turretInfo;
	void Start () {
        InvokeRepeating("Calculate", 3, turretInfo.attackSpeed);
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

    private void Calculate()
    {
        if (enemiesList != null)
        {
            for (int i = 0; i < enemiesList.Count; i++)
            {
                //distance.Add(Vector3.Distance(tMuzzle.transform.position, enemiesList[i].transform.position),(transform.position.x));
                distanceID[distance ,enemiesList[].transform.position.x];
                //distanceID[][].Min(0);
            }
            // distance.Min()
        }
    }

    private void Shoot()
    {


    }
}