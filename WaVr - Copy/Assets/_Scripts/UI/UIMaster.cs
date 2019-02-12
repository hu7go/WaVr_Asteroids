using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//TODO exchange all UI from Manager to in here
public class UIMaster : MonoBehaviour
{
    public delegate void Function();
    //public Function myFunction;

    [SerializeField]
    private Text noBuildText;
    [SerializeField]
    private Text nowHealingText;

    void Start()
    {
        Fun(Fun2, Fun3);
    }

    public void Fun (Function newFuntion, Function myFunkytion)
    {
        newFuntion();
        myFunkytion();
    }

    private void Fun2 ()
    {

    }

    private void Fun3()
    {

    }

    void Update()
    {
        
    }
}