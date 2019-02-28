using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretReloader : MonoBehaviour
{   
    [SerializeField]
    private Image reloadBar;
    [SerializeField]
    private Text turretsText;

    //add something for healer later on?

    private bool full = true;
    private bool fullHealer = true;
    private bool reloading = false;
    private bool reloadingHealer = false;

    [SerializeField] private int maxNumberOfTurrets = 5;
    [SerializeField] private int maxNumberOfHealers = 3;
    [HideInInspector] public int numberOfTurretsLeft = 5;
    [HideInInspector] public int numberOfHealersLeft = 3;
    public float reloadTime = 2;
    public float reloadForHealer = 8;
    public float startValue = 0.6f;

    private float currentTime;
    [HideInInspector]
    public float currentTimeForHealer;

    private void Start() => turretsText.text = maxNumberOfTurrets.ToString();

    public void HealerUsed()
    {
        numberOfHealersLeft--;
        if (reloadingHealer == false)
            StartCoroutine(ReloadingHealer());
    }
    private IEnumerator ReloadingHealer()
    {
        reloadingHealer = true;
        while (numberOfHealersLeft < maxNumberOfHealers)
        {
            currentTimeForHealer += Time.deltaTime;

            if(currentTimeForHealer > reloadForHealer)
            {
                currentTimeForHealer = 0;
                numberOfHealersLeft++;
            }
            yield return null;
        }
        if (numberOfHealersLeft == maxNumberOfHealers)
            reloadingHealer = false;
        currentTimeForHealer = 0;
    }

    public void Reload()
    {
        numberOfTurretsLeft--;

        UpdateText();

        if (reloading == false)
            StartCoroutine(Reloading());
    }

    private IEnumerator Reloading ()
    {
        float percent;
        reloading = true;

        while (numberOfTurretsLeft < maxNumberOfTurrets)
        {
            currentTime += Time.deltaTime;

            percent = (currentTime / reloadTime) % reloadTime;

            reloadBar.fillAmount = percent * startValue;

            if (currentTime > reloadTime)
            {
                currentTime = 0;
                numberOfTurretsLeft++;
                UpdateText();
            }

            yield return null;
        }

        if (numberOfTurretsLeft == maxNumberOfTurrets)
            reloading = false;

        currentTime = 0;
    }

    private void UpdateText () => turretsText.text = numberOfTurretsLeft.ToString();
}