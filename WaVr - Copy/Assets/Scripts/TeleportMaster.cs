using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class TeleportMaster : MonoBehaviour
{
    public UnityEvent StartTeleport;
    public UnityEvent CanTeleport;

    public GameObject player;
    [Tooltip("The position that s looked at when checkin which side the player is currently standing on")]
    public Transform arrowPositionCheck;
    private Quaternion previousParentRotation;
    public Transform arrowsPos;
    public GameObject[] arrows;
    public ChangeSide[] arrowScripts;
    public bool ghostLine;
    public bool arrowsTeleport;

    [SerializeField] private float teleportMaxLenght = 100f;

    /*
    [SerializeField] public bool dash = false;
    [SerializeField] protected float dashSpeed = 50;
    */
    //[SerializeField] protected bool reverseRotation;

    [Space(20)]
    public Sides currentSide = Sides.up;
    [SerializeField] public bool reverseTeleport = false;
    public bool newWay;
    public bool onlyUp;

    [HideInInspector] public Vector3 newPos;
    [HideInInspector] public Quaternion newRot;
    private float tempX, tempY, tempZ;

    [HideInInspector] public SideScript currentHit;
    [HideInInspector] public SideScript previousHit;
    //The asteroid you are currently standing on!
    [HideInInspector] public SideScript currentAsteroidStandingOn;

    public VRTK_StraightPointerRenderer pointer;
    VRTK_InteractUse use;
    VRTK_InteractGrab grab;

    [Space(20)]
    public float fadeTime = .5f;
    public SideScript firstAsteroid;

    private void Start()
    {
        currentAsteroidStandingOn = firstAsteroid;
        currentHit = firstAsteroid;

        use = pointer.GetComponent<VRTK_InteractUse>();
        grab = pointer.GetComponent<VRTK_InteractGrab>();
        previousHit = firstAsteroid;

    }

    public void Update()
    {
        if (GvrControllerInput.ClickButtonUp)
        {
            StartTeleport.Invoke();
            pointer.Toggle(true, false);
        }
        if (GvrControllerInput.ClickButtonDown)
        {
            CanTeleport.Invoke();
            pointer.Toggle(false, true);
        }
        if (GvrControllerInput.AppButton)
        {
            use.AttemptUse();
            grab.AttemptGrab();
        }
    }

    public void CheckSides(RaycastHit hit, Sides otherSide = Sides.up)
    {
        if (hit.collider.GetComponent<SideScript>() != null)
        {
            if(!ghostLine)
                currentHit = hit.collider.GetComponent<SideScript>();

            Vector3 tempRot = newRot.eulerAngles;
            tempX = tempRot.x;
            tempY = tempRot.y;
            tempZ = tempRot.z;
            if (newWay)
                switch (currentSide)
                {
                    case Sides.up:
                        switch (otherSide)
                        {
                            case Sides.up:
                                //Default
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 90;
                                if (reverseTeleport)
                                    tempX = tempY = tempZ = -90;
                                break;
                            case Sides.right:
                                tempX = 270;
                                if (reverseTeleport)
                                {
                                    tempX = tempY = 90;
                                    tempZ = -90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.down:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = -90;
                                tempY = 180;
                                tempZ = 0;
                                if (reverseTeleport)
                                {
                                    tempX = 90;
                                    tempZ = 180;
                                }
                                break;
                            case Sides.right:
                                tempX = tempY = 90;
                                tempZ = -90;
                                if (reverseTeleport)
                                    tempX = -90;
                                break;

                            default:
                                break;
                        }
                        break;

                    case Sides.front:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 0;
                                tempY = tempZ = 270;
                                if (reverseTeleport)
                                    tempY = tempZ = 90;
                                break;
                            case Sides.right:
                                tempX = 0;
                                tempY = 90;
                                tempZ = 270;
                                if (reverseTeleport)
                                {
                                    tempY = 270;
                                    tempZ = 90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.back:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                break;
                            case Sides.left:
                                tempX = 0;
                                tempY = tempZ = 90;
                                if (reverseTeleport)
                                    tempY = tempZ = 270;
                                break;
                            case Sides.right:
                                tempX = 0;
                                tempY = 270;
                                tempZ = 90;
                                if (reverseTeleport)
                                {
                                    tempY = 90;
                                    tempZ = 270;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.left:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                break;
                            case Sides.right:
                                tempX = 180;
                                tempY = -90;
                                tempZ = 90;
                                if (reverseTeleport)
                                {
                                    tempY = 90;
                                    tempZ = -90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.right:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 90;
                                tempY = tempZ = 0;
                                if (reverseTeleport)
                                {
                                    tempX = 270;
                                    tempZ = 180;
                                }
                                break;
                            case Sides.right:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            tempRot = new Vector3(tempX, tempY, tempZ);
            if (!arrowsTeleport)
                newRot = Quaternion.Euler(tempRot);
            if (arrowsTeleport)
                previousParentRotation = player.transform.parent.rotation;
            Teleport();
        }
        else
            return;
    }

    private void UP()
    {
        tempX = tempY = tempZ = 0;
        if (reverseTeleport)
            tempY = 180;
    }

    private void Front()
    {
        tempX = tempY = 180;
        tempZ = 90;
        if (reverseTeleport)
            tempX = 0;
    }

    private void Down()
    {
        tempX = tempY = 0;
        tempZ = 180;
        if (reverseTeleport)
            tempY = 180;
    }

    private void Back()
    {
        tempX = tempY = 0;
        tempZ = 90;
        if (reverseTeleport)
            tempX = 180;
    }

    protected void Teleport()
    {
        GetComponent<VRTK_HeadsetFade>().Fade(Color.black, fadeTime);
        Invoke("ActualTeleport", fadeTime);
    }

    void ActualTeleport ()
    {
        if(!arrowsTeleport)
            currentSide = currentHit.sides;
        if (onlyUp)
        {
            player.transform.position = currentHit.TeleportPosition().position;
            player.transform.rotation = newRot;
        }
        else
        {
            if(currentHit.GetComponent<AsteroidRot>() != null)
            {
                player.transform.position = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).position + new Vector3(0, -.75f, 0);
                player.transform.rotation = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).rotation;
            }
            else
            {
                player.transform.position = currentHit.TeleportPosition().position;
                player.transform.rotation = newRot;
            }
        }
        //if (arrowsTeleport)
        //{
        //    player.transform.rotation = newRot;
        //}

        //TODO: fix this!!! it should only happen in the version where it is needed
        //if (Manager.Instance.teleportVersion != Manager.TeleVersion.anywhere)
            //player.transform.parent = currentHit.rotator; ///VI FÅR INTE EN NY ROTATOR UTAN VI ÄR KVAR UNDER DEN FÖRSTA.

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrows)
        {
            player.transform.parent = currentHit.rotator;

            player.transform.localPosition = new Vector3(0, 0.75f, 0);
            player.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            player.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTopSide || Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
            player.transform.localPosition += new Vector3(2, 0, 0);

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
        {
            player.transform.parent = currentHit.rotator;

            player.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            player.transform.localRotation = Quaternion.Euler(0, 0, 0);

            player.transform.localPosition = new Vector3(2, .7f, 0);
        }

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTop)
        {
            player.transform.localPosition -= new Vector3(0, .75f, 0);
        }

        GetComponent<VRTK_HeadsetFade>().Unfade(fadeTime);

        ghostLine = false;

        Manager.Instance.teleportOffset = currentHit.transform.position;
        Manager.Instance.UpdateAsteroidLine();
    }

    public void AsteroidToBuildOn ()
    {
        if (currentHit.GetComponent<TurretMenuMaster>() == null)
            currentHit.GetComponentInParent<TurretMenuMaster>().CheckSides(this);
        else
            currentHit.GetComponent<TurretMenuMaster>().CheckSides(this);
    }

    public void RemoveTurretButtonsOnAsteroid ()
    {
        if (currentHit.GetComponent<TurretMenuMaster>() != null)
            currentHit.GetComponent<TurretMenuMaster>().RemoveButtons();
    }

    public void SetHit (RaycastHit newHit)
    {
        if (newHit.collider.GetComponent<SideScript>() != null)
            currentHit = newHit.collider.GetComponent<SideScript>();
    }

    public void IncreaseMaxLenght ()
    {
        teleportMaxLenght += 10;
    }

    public void ReseMaxLenght ()
    {
        teleportMaxLenght = 35;
    }

    public float GetMaxLenght()
    {
        return teleportMaxLenght;
    }

    public void DDTest()
    {
        player.transform.position += new Vector3(1, 0, 1);
    }

    public void ChechWhichSideIsClosest ()
    {
        currentAsteroidStandingOn.GetComponent<TurretMenuMaster>().CheckSides(this, false);
    }
}