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

    private Function previousFunc;
    private bool textShowing;
    
    public void CoroutineStarter(Function firstFunction, Function secondFunction)
    {
        if(textShowing == true)
            StopAllCoroutines();
        StartCoroutine(TextOnDelayOff(firstFunction, secondFunction));
    }
    public IEnumerator TextOnDelayOff (Function firstFunction, Function secondFunction)
    {
        firstFunction();
        textShowing = true;
        yield return new WaitForSeconds(textDelayTime);
        textShowing = false;
        secondFunction();
    }

    public void NobuildTextStart()
    {
        noBuildText.GetComponent<Text>().text = "You can't build here, asteroid is dead";
        noBuildText.SetActive(true);
    }

    public void NobuildTextStop()
    {
        if (textShowing == false) 
            noBuildText.SetActive(false);
    }


    public void NowHealingTextStart()
    {
        noBuildText.GetComponent<Text>().text = "Now healing this asteroid";
        noBuildText.SetActive(true);
    }

    public void NowHealingTextStop()
    {
        if (textShowing == false) 
           noBuildText.SetActive(false);
    }
}