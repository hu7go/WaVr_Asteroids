using UnityEngine;
using UnityEngine.Events;

public class ChangeSide : MonoBehaviour
{
    public GameObject rotator;
    public TeleportMaster tpm;
    public direction myDirection;

    private Sides previousDirection;

    public enum direction
    {
        forward,
        backward,
        left,
        right,
    }

    public void DoFunction ()
    {
        Rotate();
    }

    public void Rotate ()
    {
        switch (myDirection)
        {
                //The direction im clicking!
            case direction.forward:
                rotator.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                break;
                //The direction im clicking!
            case direction.backward:
                rotator.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                break;
                //The direction im clicking!
            case direction.left:
                rotator.transform.localRotation *= Quaternion.Euler(0, 0, 90);
                break;
                //The direction im clicking!
            case direction.right:
                rotator.transform.localRotation *= Quaternion.Euler(0, 0, -90);
                break;
            default:
                break;
        }
        tpm.ChechWhichSideIsClosest();
    }
}