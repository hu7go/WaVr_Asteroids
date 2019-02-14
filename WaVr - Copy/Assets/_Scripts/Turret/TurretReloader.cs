using UnityEngine;
using UnityEngine.UI;

public class TurretReloader : MonoBehaviour
{   
    [SerializeField]
    private Image reloadBar;
    [SerializeField]
    private Text numberOfTurrets;

    private bool full = true;
    private bool reloading = false;

    [SerializeField]
    private int maxNumberOfTurrets = 5;
    public int numberOfTurretsLeft = 5;
    public float reloadTime = 2;
    public float startValue = 0.6f;

    public void Reload()
    {
        numberOfTurrets.text = numberOfTurretsLeft.ToString();
        if (numberOfTurretsLeft < maxNumberOfTurrets)
            full = false;
        if (!full && !reloading)
        {
            reloading = true;
            full = false;
            while(reloadBar.fillAmount > 0)
                reloadBar.fillAmount -= Time.deltaTime;

            if(reloadBar.fillAmount == 0)
            {
                numberOfTurretsLeft++;
                numberOfTurrets.text = numberOfTurretsLeft.ToString();
                reloading = false;
                reloadBar.fillAmount = startValue;
            }
            if (numberOfTurretsLeft == maxNumberOfTurrets)
            {
                full = true;
                return;
            }

            else if (numberOfTurretsLeft < maxNumberOfTurrets)
                Reload();
        }
    }
}