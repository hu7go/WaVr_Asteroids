using UnityEngine;

public enum direction
{
    forward,
    backward,
    left,
    right,
}

public class ChangeSide : MonoBehaviour
{
    public GameObject rotator;
    public TeleportMaster tpm;
    public direction myDirection;
    public bool useIndex;

    private Sides previousDirection;

    public void DoFunction () => Rotate();

    public void Rotate ()
    {
        if (useIndex)
        {
            switch (myDirection)
            {
                    //The direction im clicking!
                case direction.forward:
                    switch (tpm.arrowIndex)
                    {
                        case 1:
                            RotatePlayer(90);
                            break;
                        case 2:
                            RotatePlayer(0, -90);
                            break;
                        case 3:
                            RotatePlayer(-90);
                            break;
                        case 4:
                            RotatePlayer(0, 90);
                            break;
                    }
                    break;
                    //The direction im clicking!
                case direction.backward:
                    switch (tpm.arrowIndex)
                    {
                        case 1:
                            RotatePlayer(-90);
                            break;
                        case 2:
                            RotatePlayer(0, 90);
                            break;
                        case 3:
                            RotatePlayer(90);
                            break;
                        case 4:
                            RotatePlayer(0, -90);
                            break;
                    }
                    break;
                    //The direction im clicking!
                case direction.left:
                    switch (tpm.arrowIndex)
                    {
                        case 1:
                            RotatePlayer(0, 90);
                            break;
                        case 2:
                            RotatePlayer(90);
                            break;
                        case 3:
                            RotatePlayer(0, -90);
                            break;
                        case 4:
                            RotatePlayer(-90);
                            break;
                    }
                    break;
                    //The direction im clicking!
                case direction.right:
                    switch (tpm.arrowIndex)
                    {
                        case 1:
                            RotatePlayer(0, -90);
                            break;
                        case 2:
                            RotatePlayer(-90);
                            break;
                        case 3:
                            RotatePlayer(0, 90);
                            break;
                        case 4:
                            RotatePlayer(90);
                            break;
                    }
                    break;
            }
        }
        else
        {
            switch (myDirection)
            {
                case direction.forward:
                    RotatePlayer(90);
                    break;
                case direction.backward:
                    RotatePlayer(-90);
                    break;
                case direction.left:
                    RotatePlayer(0, 90);
                    break;
                case direction.right:
                    RotatePlayer(0, -90);
                    break;
            }
        }
        tpm.ChechWhichSideIsClosest();

        Manager.Instance.SetWorldAxis();
    }

    private void RotatePlayer (float x = 0, float z = 0)
    {
        rotator.transform.localRotation *= Quaternion.Euler(x, 0, z);
    }
}