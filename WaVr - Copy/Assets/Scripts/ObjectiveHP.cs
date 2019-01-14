using System;
using UnityEngine;

public class ObjectiveHP : MonoBehaviour {
    [SerializeField] private float Hp = 100;

    private void CheckHP()
    {
        //make a color gradient change or something to display health, maybe a hp bar when hovering over the Objective with rayvast.
        if(Hp <= 0)
        {
            Manager.Instance.GameOver();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            Hp -= 5f;
            CheckHP();
        }
    }
}
