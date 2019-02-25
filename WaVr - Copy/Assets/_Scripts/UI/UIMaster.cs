using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//TODO exchange all UI from Manager to in here
public class UIMaster : MonoBehaviour
{
    public delegate void Function();
    public float textDelayTime = 8;
    [SerializeField]
    private GameObject noBuildText;
    [SerializeField]
    private GameObject nowHealingText;

    private bool isRunning;

    public IEnumerator TextOnDelayOff (Function firstFunction, Function secondFunction)
    {
        firstFunction();
        isRunning = true;
        yield return new WaitForSeconds(textDelayTime);
        secondFunction();
        isRunning = false;
    }

    public void NobuildTextStart()
    {
        noBuildText.GetComponent<Text>().text = "You can't build here, asteroid is dead";
        if(isRunning == false)
            noBuildText.SetActive(true);
    }

    public void NobuildTextStop() => noBuildText.SetActive(false);

    public void NowHealingTextStart()
    {
        noBuildText.GetComponent<Text>().text = "Now healing this asteroid";
        if(isRunning == false)
            noBuildText.SetActive(true);
    }

    public void NowHealingTextStop() => noBuildText.SetActive(false);
}