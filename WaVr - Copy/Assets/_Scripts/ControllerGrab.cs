using UnityEngine;
using VRTK;

public class ControllerGrab : MonoBehaviour
{
    private bool isContact = false;
    private bool isGrabed = false;
    private GameObject objGrabbed;

    public VRTK_InteractUse use;
    public Transform gunPos;

    private void Update()
    {
        if (GvrControllerInput.AppButton)
            if (isContact)
            {
                if (!isGrabed)
                {
                    isGrabed = true;
                    objGrabbed.transform.position = gunPos.position;
                    objGrabbed.transform.rotation = gunPos.rotation;
                    objGrabbed.transform.parent = gameObject.transform;
                    objGrabbed.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    //isGrabed = false;
                    //objGrabbed.transform.parent = null;
                    //objGrabbed.GetComponent<Rigidbody>().isKinematic = false;
                }
            }

        if (GvrControllerInput.AppButton)
            if (isGrabed)
                objGrabbed.GetComponent<VRTK_InteractableObject>().StartUsing();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "pickUp")
        {
            isContact = true;
            objGrabbed = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "pickUp")
        {
            isContact = false;
            objGrabbed = null;
        }
    }
}