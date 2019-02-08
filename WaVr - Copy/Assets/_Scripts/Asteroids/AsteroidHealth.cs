using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float>
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;

    float h;
    float s;
    float v;

    public void Start()
    {
        asteroid.postition = transform.position;
        asteroid.health = Manager.Instance.turretsAndEnemies.asteroidHealth;
        asteroid.alive = true;

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);
    }

    public void TakeDamage (float damage)
    {
        asteroid.health -= damage;
        if (asteroid.health <= 0)
            asteroid.alive = false;

        Manager.Instance.UpdateHealth(-damage);

        UpdateColor();
    }

    public void Heal (float newHealth)
    {
        Manager.Instance.UpdateHealth(newHealth);
        UpdateColor();
    }

    void UpdateColor ()
    {
        s = (asteroid.health / 100) / (Manager.Instance.turretsAndEnemies.asteroidHealth / 100);
        rend.material.SetColor("_Color", Color.HSVToRGB(h, s, v)); 
    }

    public AsteroidInfo GetInfo ()
    {
        return asteroid;
    }
}

[System.Serializable]
public struct AsteroidInfo
{
    public Vector3 postition;
    public float health;
    public bool alive;
}
