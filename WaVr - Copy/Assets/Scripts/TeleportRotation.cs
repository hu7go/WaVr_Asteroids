using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleportRotation : MonoBehaviour
{
    public TeleportMaster master;
    [SerializeField] private VRTK_CustomRaycast cr;

    [Space(20)]

    public List<GameObject> hands;
    public GameObject[] parents = new GameObject[2];
    public GameObject teleportorBuildUI;
    private GameObject currentHand;

    public bool canTeleport = false;
    private bool switchedHands;
    RaycastHit hit;
    RaycastHit previousHit;
    RaycastHit asteroidHit;
    RaycastHit prevAsteroidHit;
    RaycastHit buildButtonTarget;
    RaycastHit tempPrevAsteroidHit;
    RaycastHit tempAsteroidHit;
    RaycastHit ghostLineHit;
    RaycastHit ghostLinePrevHit;
    Ray ray;
    public bool highlightCubes = false;

    LineRenderer line;
    Vector3[] linePos;
    public Transform lineEnd;
    public bool renderOwnLine = true;

    RaycastHit tmpPrev = new RaycastHit();

    int startLayerMask = 1 << 14;
    RaycastHit startHit;

    private void Start()
    {
        if (cr == null)
            cr = GetComponent<VRTK_CustomRaycast>();

        UpdateLineRenderer();

        line = GetComponent<LineRenderer>();
        linePos = new Vector3[2];

        Debug.DrawRay(master.firstAsteroid.transform.position + new Vector3(2, 0, 0), master.firstAsteroid.transform.position + new Vector3(-5,0,0));

        if (Physics.Raycast(master.firstAsteroid.transform.position + new Vector3(2, 0, 0), master.firstAsteroid.transform.position + new Vector3(-5, 0, 0), out hit))
            prevAsteroidHit = hit;

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.anywhere)
            highlightCubes = true;
        else
            highlightCubes = false;
    }

    public void Update()
    {
        if (!switchedHands)
            SwtichHands();

        if (renderOwnLine)
        {
            if (line != null)
            {
                linePos[1] = currentHand.transform.position;
                linePos[0] = lineEnd.position;
            }
        }

        //If we hit the start button!
        if (canTeleport)
        {
            if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out startHit, master.GetMaxLenght(), startLayerMask))
                Manager.Instance.StartGame();
        }
        //

        if (canTeleport && Manager.Instance.StartedGame() && !Manager.Instance.holdingGun)
        {
            if (renderOwnLine)
            {
                line.enabled = true;
                line.SetPositions(linePos);
            }

            ray = new Ray(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward));

            Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out hit, master.GetMaxLenght());

            cr.CustomRaycast(ray, out hit, master.GetMaxLenght());

            //Debug.DrawRay(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward) * 20, Color.white, 10);

            if (Manager.Instance.turretHover)
            {
                int layerMask = 1 << 13;
                if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out buildButtonTarget, master.GetMaxLenght(), layerMask))
                {
                    buildButtonTarget.collider.GetComponent<TurretSpawn>().ShowRangeIndicator();
                    tmpPrev = buildButtonTarget;
                }
                else
                    if (tmpPrev.collider != null)
                        tmpPrev.collider.GetComponent<TurretSpawn>().DisableRangeIndicator();
            }

            if (highlightCubes)
            {
                if (hit.collider != null)
                    if (hit.collider.GetComponentInChildren<CubeHighlight>() != null)
                    {
                        if (previousHit.collider != null)
                            if (previousHit.collider != hit.collider)
                                previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();

                        previousHit = hit;
                        previousHit.collider.GetComponentInChildren<CubeHighlight>().Render();
                    }
                if (hit.collider == null)
                    if (previousHit.collider != null)
                        previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();
            }
        }
    }

    private void SwtichHands()
    {
        if (parents[0] == enabled && parents[0].activeInHierarchy)
        {
            currentHand = hands[0];
            switchedHands = true;
            renderOwnLine = false;
            return;
        }
        if (parents[1] == enabled && parents[1].activeInHierarchy)
        {
            currentHand = hands[1];
            switchedHands = true;
            renderOwnLine = false;
            return;
        }
        if (parents[2] == enabled && parents[2].activeInHierarchy)
        {
            if (hands[2] != null)
            {
                Manager.Instance.UsingDayDream();
                renderOwnLine = true;

                currentHand = hands[2];
                switchedHands = true;
                return;
            }
        }
        else
            return;
    }

    public void CanTeleport()
    {
        canTeleport = true;
    }

    public void StartTeleport()
    {
        if (hit.collider)
        {
            line.endColor = Color.green;
            line.startColor = Color.green;
            
            //If we hit the teleport state button!
            if (hit.collider.CompareTag("TeleportState"))
                Manager.Instance.SetPointerState(Manager.PointerState.Teleport);
            //

            //If we hit the build state button!
            if (hit.collider.CompareTag("BuildState"))
                Manager.Instance.SetPointerState(Manager.PointerState.Build);
            //

            //If we hit the rotate state button!
            if (hit.collider.CompareTag("RotateState"))
                Manager.Instance.SetPointerState(Manager.PointerState.Rotate);
            //

            //If we hit a pizza!
            if (hit.collider.CompareTag("Pizza"))
            {
                //Do some pizza shit
                Manager.Instance.UpdatePizzaCounter();
                hit.collider.gameObject.SetActive(false);
                Manager.Instance.ReturnPlayer().GetComponent<AudioSource>().Play();
                Manager.Instance.RefreshGun();
                hit = new RaycastHit();
                return;
            }
            //

            //If we hit the ghost line to the previous asteroid
            if (hit.collider.CompareTag("GhostLine"))
            {
                master.newPos = prevAsteroidHit.point;
                master.newRot = prevAsteroidHit.collider.transform.rotation;
                master.ghostLine = true;

                master.SetHit(prevAsteroidHit);

                RaycastHit temporary;
                temporary = prevAsteroidHit;
                prevAsteroidHit = ghostLineHit;
                ghostLineHit = temporary;
                asteroidHit = ghostLineHit;

                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

                TeleportTrue(true);
            }
            //

            //! If the manager is set to teleport you!
            if (Manager.Instance.pointerState == Manager.PointerState.Teleport || Manager.Instance.pointerState == Manager.PointerState.Rotate)
            {
                if (Manager.Instance.pointerState == Manager.PointerState.Rotate)
                {
                    //remove this return if you should be able to teleport while in rotate mode!
                    return;
                    Manager.Instance.pointerState = Manager.PointerState.Teleport;
                }

                //If the target hit has a highlightscript!
                if (highlightCubes)
                    if (previousHit.collider.GetComponentInChildren<CubeHighlight>() != null)
                        previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();
                //

                //If the target hit is has a sidescript attached to determin where we teleport!
                if (hit.collider.GetComponent<SideScript>() != null)
                {
                    if (hit.collider == asteroidHit.collider)
                        return;

                    if (master.arrowsTeleport)
                    {
                        hit.collider.GetComponent<SideScript>().sides = master.currentSide;
                        master.newRot = hit.collider.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(hit.collider.GetComponent<SideScript>().sides).rotation;
                    }
                    else
                        master.newRot = hit.collider.transform.rotation;

                    master.newPos = hit.point;

                    if (!Manager.Instance.useNewUI)
                        teleportorBuildUI.SetActive(true);

                    master.SetHit(hit);

                    if (asteroidHit.collider == null)
                        asteroidHit = prevAsteroidHit;

                    tempPrevAsteroidHit = prevAsteroidHit;
                    tempAsteroidHit = asteroidHit;

                    prevAsteroidHit = asteroidHit;
                    asteroidHit = hit;

                    ghostLineHit = asteroidHit;

                    //If we use the new State version of the UI we shoudl teleport instantly uppon clicking on a asteroid!
                    if (Manager.Instance.useNewUI)
                        TeleportTrue();
                }
                //

                //If we hit the teleport button
                if (hit.collider.CompareTag("TeleButton"))
                {
                    DisableTeleMenu();

                    TeleportTrue();
                    master.RemoveTurretButtonsOnAsteroid();
                }
                //
            }
            //

            //! If the manager is set to build on the selected cube!
            if (Manager.Instance.pointerState == Manager.PointerState.Build)
            {
                //If we hit a UI element that spawns a turret!
                if (hit.collider.GetComponent<TurretSpawn>() != null)
                {
                    master.ReseMaxLenght();
                    UpdateLineRenderer();

                    TurretSpawn newTurretSpawn = hit.collider.GetComponent<TurretSpawn>();
                    newTurretSpawn.SpawnEm();
                    Manager.Instance.CurrentBuildTarget(newTurretSpawn);
                }
                //

                //If we hit the build button
                if (Manager.Instance.useNewUI)
                {
                    if (hit.collider.GetComponent<SideScript>() != null)
                    {
                        master.currentHit = hit.collider.GetComponent<SideScript>();

                        prevAsteroidHit = tempPrevAsteroidHit;
                        asteroidHit = tempAsteroidHit;
                        master.AsteroidToBuildOn();

                        master.IncreaseMaxLenght();
                        UpdateLineRenderer();
                    }
                }
                else
                {
                    if (hit.collider.CompareTag("BuildButton"))
                    {
                        DisableTeleMenu();

                        prevAsteroidHit = tempPrevAsteroidHit;
                        asteroidHit = tempAsteroidHit;
                        master.AsteroidToBuildOn();

                        master.IncreaseMaxLenght();
                        UpdateLineRenderer();
                    }
                }
                //

                //If we hit a cancelButton
                if (hit.collider.CompareTag("CancelButton"))
                {
                    DisableTeleMenu();

                    RaycastHit tmp;
                    tmp = prevAsteroidHit;
                    asteroidHit = tmp;
                    prevAsteroidHit = ghostLineHit;
                    ghostLineHit = tmp;
                }
                //
            }
            //

            //! If the manager is set to rotate you on the current cube you are standing on!
            if (Manager.Instance.pointerState == Manager.PointerState.Rotate)
            {
                //If its a arrow that determins our rotation!
                if (hit.collider.GetComponent<ChangeSide>() != null)
                    hit.collider.GetComponent<ChangeSide>().DoFunction();
                //
            }

            //Not currently in use!
            //If we hit the confim or deny button!
            if (hit.collider.GetComponent<ConfirmDeny>() != null)
                hit.collider.GetComponent<ConfirmDeny>().DoEffect();
            //

            if (renderOwnLine)
                SetLineEnd(hit);
        }
        else
        {
            line.endColor = Color.red;
            line.startColor = Color.red;
        }
        canTeleport = false;
        if (renderOwnLine)
            line.enabled = false;
    }

    bool tmp = false;

    public void TeleportTrue (bool ghost = false)
    {
        if (ghost)
        {
            if (tmp)
                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

            master.CheckSides(ghostLineHit, ghostLineHit.collider.GetComponent<SideScript>().sides);
            ghostLineHit.collider.GetComponent<SideScript>().Reached();
        }
        else
        {
            if (tmp && !master.ghostLine)
                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

            master.CheckSides(asteroidHit, asteroidHit.collider.GetComponent<SideScript>().sides);
            asteroidHit.collider.GetComponent<SideScript>().Reached();
        }
        tmp = true;
    }

    public void DisableTeleMenu ()
    {
        teleportorBuildUI.SetActive(false);
    }

    public void SetLineEnd (RaycastHit hit)
    {
        linePos[0] = hit.point;
        line.SetPositions(linePos);
    }

    public void ResetLineEnd ()
    {
        linePos[0] = lineEnd.position;
        line.SetPositions(linePos);
    }

    void UpdateLineRenderer ()
    {
        GetComponent<VRTK_StraightPointerRenderer>().maximumLength = master.GetMaxLenght();
    }
}