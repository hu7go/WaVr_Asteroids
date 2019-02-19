using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretReloader : MonoBehaviour
{   
    [SerializeField]
    private Image reloadBar;
    [SerializeField]
    private Text turretsText;

    private bool full = true;
    private bool reloading = false;

    [SerializeField] private int maxNumberOfTurrets = 5;
    [HideInInspector] public int numberOfTurretsLeft = 5;
    public float reloadTime = 2;
    public float startValue = 0.6f;

    private float currentTime;

    private void Start() => turretsText.text = maxNumberOfTurrets.ToString();

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