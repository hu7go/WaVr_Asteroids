using System.Collections;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float, EnemySpawnPoint>
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;
    [HideInInspector] public Healer myHealer;
    private TurretMenuMaster turretMaster;
    Color red;
    float h;
    float s;
    float v;

    Color currentColor;
    SphereCollider sphereCollider;

    public void Awake()
    {
        //red = new Color(360, 100, 50);
        red = Color.red;

        turretMaster = GetComponentInParent<TurretMenuMaster>();

        asteroid = new AsteroidInfo(transform.position, Manager.Instance.tAe.asteroidHealth, true, false);

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);

        sphereCollider = GetComponentInParent<SphereCollider>();
    }

    [HideInInspector] public bool tmp = false;
    bool damageBool = false;

    public void SetStartHealth (float damage)
    {
        asteroid.health -= damage;
        if (asteroid.health <= 0)
        {
            asteroid.alive = false;
            Manager.Instance.masterCurrentHealth -= damage;
            UpdateColor();
        }
    }

    public void TakeDamage(float damage, EnemySpawnPoint enemyOrigin)
    {
        if (asteroid.beingHealed == true)
        {
            myHealer.TakeDamage(damage);
            return;
        }

        if (damageBool == false)
        {
            damageBool = true;
            StartCoroutine(DamageVisual(.05f));
        }

        asteroid.health -= damage;
        enemyOrigin.HealthTracker(damage);
        if (asteroid.health <= 0)
        {
            turretMaster.AsteroidDied();
            asteroid.alive = false;
            if (tmp == false)
            {
                //! Un comment this to get it to work as it used to!
                //Manager.Instance.UpdatePath(transform.position, enemyOrigin);
                tmp = true;
            }
        }

        if (asteroid.health < 0)
        {
            enemyOrigin.HealthTracker(-asteroid.health);
            Manager.Instance.UpdateHealth(-asteroid.health);
        }

        Manager.Instance.UpdateHealth(-damage);

        UpdateColor();
    }

    public IEnumerator HealingVisual(float newTime)
    {
        rend.material.color = Color.green;
        yield return new WaitForSeconds(newTime);
        rend.material.color = currentColor;

        healingBool = false;
    }

    public IEnumerator DamageVisual(float newTime)
    {
        rend.material.color = Color.white;
        yield return new WaitForSeconds(newTime);
        rend.material.color = currentColor;

        damageBool = false;
    }

    bool healingBool = false;

    public void Heal(float newHealth)
    {
        Manager.Instance.UpdateHealth(newHealth);
        if (healingBool == false)
        {
            healingBool = true;
            StartCoroutine(HealingVisual(.1f));
        }
        UpdateColor();
    }

    public void UpdateColor()
    {
        s = (asteroid.health / 100) / (Manager.Instance.tAe.asteroidHealth / 100);
        if (damageBool  == false && healingBool == false)
            rend.material.SetColor("_Color", Color.HSVToRGB(h, s, v));
        currentColor = Color.HSVToRGB(h, s, v);
    }

    public AsteroidInfo GetInfo() => asteroid;

    public void Revive ()
    {
        tmp = false;
        asteroid.alive = true;
    }

    public void ColliderOn () => sphereCollider.enabled = true;

    public void ColliderOff () => sphereCollider.enabled = false;
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