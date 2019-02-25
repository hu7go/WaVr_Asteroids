using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        //StartCoroutine(TextOnDelayOff(NobuildTextStart, NobuildTextStop));
    }

    public IEnumerator TextOnDelayOff (Function firstFunction, Function secondFunction)
    {
        if(textShowing == true)
        {
            previousFunc();
            previousFunc = secondFunction;
            textShowing = false;
        }
        firstFunction();
        textShowing = true;
        previousFunc = secondFunction;
        yield return new WaitForSeconds(textDelayTime);
        secondFunction();
        textShowing = false;
    }

    public void NobuildTextStart()
    {
        noBuildText.GetComponent<Text>().text = "You can't build here, asteroid is dead";
        noBuildText.SetActive(true);
    }

    public void NobuildTextStop() => noBuildText.SetActive(false);

    public void NowHealingTextStart()
    {
        nowHealingText.GetComponent<Text>().text = "Now healing this asteroid";
        nowHealingText.SetActive(true);
    }

    public void NowHealingTextStop() => nowHealingText.SetActive(false);

    void Update()
    {
        
    }
}