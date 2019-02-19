using UnityEngine;

public class TurretRange : MonoBehaviour
{
    public TurretAI turret;

    private void Awake() => turret = GetComponentInParent<TurretAI>();

    private void Start()
    {
        float range = turret.turretInfo.rangeRadius;
        transform.localScale = new Vector3(range, range, range);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            turret.EnteredRange(other.GetComponent<EnemyAI>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            turret.ExitedRange(other.GetComponent<EnemyAI>());
    }
}