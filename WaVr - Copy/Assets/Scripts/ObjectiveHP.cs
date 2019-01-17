using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHP : MonoBehaviour {
    [SerializeField] private float Hp = 100;
    [SerializeField] private LayerMask bulletLayerMask;

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
        if (other.CompareTag("bullet") && other.gameObject.layer == bulletLayerMask)
        {
            Hp -= 5f;
            Manager.Instance.slider.value = Hp;
            CheckHP();
        }

        if (!other.CompareTag("bullet"))
        {
            return;
        }
    }
}