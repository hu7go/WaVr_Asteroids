using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float>
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;
    [HideInInspector] public Healer myHealer;
    private TurretMenuMaster turretMaster;

    float h;
    float s;
    float v;

    public void Start()
    {
        turretMaster = GetComponentInParent<TurretMenuMaster>();

        asteroid = new AsteroidInfo(transform.position, Manager.Instance.tAe.asteroidHealth, true, false);

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);
    }

    bool tmp = false;

    public void TakeDamage (float damage)
    {
        if (asteroid.beingHealed == true)
        {
            myHealer.TakeDamage(damage);
            return;
        }

        asteroid.health -= damage;
        if (asteroid.health <= 0)
        {
            turretMaster.AsteroidDied();
            asteroid.alive = false;
            if (tmp == false)
            {
                Manager.Instance.UpdatePath(transform.position);
                tmp = true;
            }
        }

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
        s = (asteroid.health / 100) / (Manager.Instance.tAe.asteroidHealth / 100);
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
    public bool beingHealed;

    public AsteroidInfo(Vector3 postition, float health, bool alive, bool beingHealed)
    {
        this.postition = postition;
        this.health = health;
        this.alive = alive;
        this.beingHealed = beingHealed;
    }
}
