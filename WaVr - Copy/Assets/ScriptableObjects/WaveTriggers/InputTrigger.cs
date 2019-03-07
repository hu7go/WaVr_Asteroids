using UnityEngine;

[CreateAssetMenu(fileName = "InputTrigger", menuName = "ScriptableObject/Wave/InputTrigger", order = 5)]
public class InputTrigger : TriggerManager
{
    public override bool Trigger()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Testsetset input");
            return true;
        }
        else
        {
            return false;
        }
    }
}