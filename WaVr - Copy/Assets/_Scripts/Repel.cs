using UnityEngine;

public class Repel : MonoBehaviour
{
    [SerializeField] EnemyAI main;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<EnemyAI>() != null)
        {
            main.tooClose = true;
            main.pushAwayFrom = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<EnemyAI>() != null)
            main.tooClose = false;
    }
}