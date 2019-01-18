using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHP : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    [SerializeField] private LayerMask bulletLayerMask;

    public void TakeDamage (int damage)
    {
        hp -= damage;
        Manager.Instance.uISettings.slider.value = hp;
        CheckHP();
    }

    private void CheckHP()
    {
        //make a color gradient change or something to display health, maybe a hp bar when hovering over the Objective with rayvast.
        if(hp <= 0)
        {
            Manager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}