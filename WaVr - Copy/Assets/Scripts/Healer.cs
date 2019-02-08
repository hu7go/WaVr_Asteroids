using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour, ITakeDamage<float>
{
    HealerInfo hI;
    void Start()
    {
        hI.health = 100;
        hI.alive = true;
    }

    void Update()
    {
        if (hI.alive == false)
            Destroy(gameObject);

    }

    public void TakeDamage(float damage)
    {
        hI.health -= damage;

        if (hI.health <= 0)
            hI.alive = false;
    }
}

[System.Serializable]
public struct HealerInfo
{
    public Vector3 postition;
    [Range(0, 100)]
    public float health;
    public float regenSpeed;
    public float regenValue;
    public bool alive;
}