using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float>
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;
    [HideInInspector] public Healer myHealer;
    private TurretMenuMaster turretMaster;
    Color colorStart;
    Color colorEnd;
    Color switcher;
    Color red;
    float h;
    float s;
    float v;
    float rate;
    float i;
    bool healing;
    bool takingDamage;

    public void Start()
    {
        //red = new Color(360, 100, 50);
        red = Color.red;

        turretMaster = GetComponentInParent<TurretMenuMaster>();

        asteroid = new AsteroidInfo(transform.position, Manager.Instance.tAe.asteroidHealth, true, false);

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);

        colorStart = rend.material.GetColor("_Color");
        colorEnd = rend.material.GetColor("_Color");
    }

    bool tmp = false;

    public void TakeDamage(float damage)
    {
        if (asteroid.beingHealed == true)
        {
            myHealer.TakeDamage(damage);
            return;
        }

        StartCoroutine(DamageVisual(.1f));

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

    public IEnumerator HealingVisual(float newTime)
    {
        float t = 0;

        Color tmpColor = rend.material.GetColor("_Color");

        while (t < 1)
        {
            t += Time.deltaTime / (newTime / 2);

            rend.material.color = Color.Lerp(rend.material.GetColor("_Color"), Color.green, t);
            yield return null;
        }

        while (t < 1)
        {
            t += Time.deltaTime / (newTime / 2);

            i += Time.deltaTime * rate;
            rend.material.color = Color.Lerp(rend.material.GetColor("_Color"), tmpColor, t);
            yield return null;
        }
    }

    public IEnumerator DamageVisual(float newTime)
    {
        float t = 0;

        Color tmpColor = rend.material.GetColor("_Color");

        while (t < 1)
        {
            t += Time.deltaTime / (newTime / 2);

            rend.material.color = Color.Lerp(rend.material.GetColor("_Color"), Color.red, t);
            yield return null;
        }

        while (t < 1)
        {
            t += Time.deltaTime / (newTime / 2);

            rend.material.color = Color.Lerp(rend.material.GetColor("_Color"), tmpColor, t);
            yield return null;
        }
    }

    public void Heal(float newHealth)
    {
        Manager.Instance.UpdateHealth(newHealth);
        StartCoroutine(HealingVisual(.1f));
        UpdateColor();
    }

    void UpdateColor()
    {
        s = (asteroid.health / 100) / (Manager.Instance.tAe.asteroidHealth / 100);
        rend.material.SetColor("_Color", Color.HSVToRGB(h, s, v));
    }

    public AsteroidInfo GetInfo()
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