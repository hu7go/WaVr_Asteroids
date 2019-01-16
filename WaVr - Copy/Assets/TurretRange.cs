using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRange : MonoBehaviour
{
    TurretAI turret;

    private void Awake()
    {
        turret = GetComponentInParent<TurretAI>();
    }

    private void Start()
    {
        float range = turret.turretInfo.rangeRadius;
        transform.localScale = new Vector3(range, range, range);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("I entered range!");
            turret.EnteredRange(other.GetComponent<EnemyAI>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("I exited range!");
            turret.ExitedRange(other.GetComponent<EnemyAI>());
        }
    }
}
