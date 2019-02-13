using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float>
{
    public AsteroidInfo asteroid;
    [HideInInspector] public Healer myHealer;
    private TurretMenuMaster turretMaster;

    float h;
    float s;
    float v;

    private MeshRenderer meshRender;

    private float colorAmount = 0;
    private string colorString = "_HealAmount";

    public void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        turretMaster = GetComponentInParent<TurretMenuMaster>();

        asteroid = new AsteroidInfo(transform.position, Manager.Instance.tAe.asteroidHealth, true, false);

        Color.RGBToHSV(meshRender.material.GetColor("_Color"), out h, out s, out v);
    }

    bool tmp = false;

    public void TakeDamage (float damage)
    {
        colorString = "_Damage";
        colorAmount += 1;

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
        colorString = "_Heal";
        colorAmount += 1;

        Manager.Instance.UpdateHealth(newHealth);
        UpdateColor();
    }

    public void Update()
    {
        colorAmount = Mathf.Lerp(colorAmount, 0, Time.deltaTime);
        meshRender.material.SetFloat(colorString, colorAmount);
    }

    void UpdateColor ()
    {
        s = (asteroid.health / 100) / (Manager.Instance.tAe.asteroidHealth / 100);
        meshRender.material.SetColor("_Color", Color.HSVToRGB(h, s, v)); 
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
