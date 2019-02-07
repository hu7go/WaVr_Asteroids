using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;

    float h;
    float s;
    float v;

    public void Start()
    {
        asteroid.postition = transform;
        asteroid.health = 100;
        asteroid.alive = true;

        rend = GetComponent<MeshRenderer>();
    }

    public void TakeDamage (int damage)
    {
        asteroid.health -= damage;
        if (asteroid.health <= 0)
            asteroid.alive = false;
        UpdateColor();
    }

    void UpdateColor ()
    {
        Color.RGBToHSV(rend.material.color, out h, out s, out v);
        s = asteroid.health;
    }

    public AsteroidInfo GetInfo ()
    {
        return asteroid;
    }
}

[System.Serializable]
public struct AsteroidInfo
{
    public Transform postition;
    public int health;
    public bool alive;
}
