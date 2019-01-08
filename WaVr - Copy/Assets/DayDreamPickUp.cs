using UnityEngine;
using VRTK;

public class DayDreamPickUp : MonoBehaviour
{
    [HideInInspector] public bool canPickUp = false;
    [HideInInspector] public VRTK_InteractableObject obj;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<VRTK_InteractableObject>() != null)
        {
            obj = other.gameObject.GetComponent<VRTK_InteractableObject>();
            canPickUp = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<VRTK_InteractableObject>() != null)
        {
            obj = null;
            canPickUp = false;
        }
    }
}