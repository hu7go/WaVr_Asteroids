using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//TODO exchange all UI from Manager to in here
public class UIMaster : MonoBehaviour
{
    public delegate void Function();
    //public Function myFunction;
    public float textDelayTime = 8;
    [SerializeField]
    private GameObject noBuildText;
    [SerializeField]
    private GameObject nowHealingText;

    void Start()
    {
        StartCoroutine(TextOnDelayOff(NobuildTextStart, NobuildTextStop));
    }

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
        nowHealingText.GetComponent<Text>().text = "Now healing this asteroid";
        nowHealingText.SetActive(true);
    }

    public void NowHealingTextStop() => nowHealingText.SetActive(false);

    void Update()
    {
        
    }
}