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


    public IEnumerator TextOnDelayOff (Function firstFunction, Function secondFunction)
    {
        firstFunction();
        yield return new WaitForSeconds(textDelayTime);
        secondFunction();
    }

    public void NobuildTextStart()
    {
        noBuildText.GetComponent<Text>().text = "You can't build here, asteroid is dead";
        noBuildText.SetActive(true);
    }

    public void NobuildTextStop() => noBuildText.SetActive(false);

    public void NowHealingTextStart()
    {
        noBuildText.GetComponent<Text>().text = "Now healing this asteroid";
        noBuildText.SetActive(true);
    }

    public void NowHealingTextStop() => noBuildText.SetActive(false);
}