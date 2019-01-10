﻿using UnityEngine;
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
                switch (tpm.arrowIndex)
                {
                    case 1:
                        rotator.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                        break;
                    case 2:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, -90);
                        break;
                    case 3:
                        rotator.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                        break;
                    case 4:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, 90);
                        break;
                    default:
                        break;
                }
                break;
                //The direction im clicking!
            case direction.backward:
                switch (tpm.arrowIndex)
                {
                    case 1:
                        rotator.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                        break;
                    case 2:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, 90);
                        break;
                    case 3:
                        rotator.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                        break;
                    case 4:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, -90);
                        break;
                    default:
                        break;
                }
                break;
                //The direction im clicking!
            case direction.left:
                switch (tpm.arrowIndex)
                {
                    case 1:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, 90);
                        break;
                    case 2:
                        rotator.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                        break;
                    case 3:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, -90);
                        break;
                    case 4:
                        rotator.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                        break;
                    default:
                        break;
                }
                break;
                //The direction im clicking!
            case direction.right:
                switch (tpm.arrowIndex)
                {
                    case 1:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, -90);
                        break;
                    case 2:
                        rotator.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                        break;
                    case 3:
                        rotator.transform.localRotation *= Quaternion.Euler(0, 0, 90);
                        break;
                    case 4:
                        rotator.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        tpm.ChechWhichSideIsClosest();
    }
}