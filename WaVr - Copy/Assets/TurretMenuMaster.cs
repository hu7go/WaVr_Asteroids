using System.Collections.Generic;
using UnityEngine;

public class TurretMenuMaster : MonoBehaviour
{
    public GameObject turretButton;

    private TurretMenu[] sideMenus;
    private List<Transform> menuPos = new List<Transform>();
    private List<TurretInfo> turretSides = new List<TurretInfo>();
    private List<GameObject> turretButtons = new List<GameObject>();

    public bool[] turretBuilt;

    private void Start()
    {
        sideMenus = GetComponentsInChildren<TurretMenu>();
        turretBuilt = new bool[6];
    }

    public void CheckSides(TeleportMaster master, bool spawn = true)
    {
        if (!spawn)
        {
            List<SideScript> tmpList = new List<SideScript>();
            SideScript finalSide = new SideScript();
            for (int i = 0; i < sideMenus.Length; i++)
            {
                SideScript tmp = sideMenus[i].CheckSideScript(false);
                tmpList.Add(tmp);
            }

            for (var i = tmpList.Count - 1; i >  -1; i--)
                if (tmpList[i] == null)
                    tmpList.RemoveAt(i);

            float distance = 10;

            foreach (SideScript side in tmpList)
            {
                if (distance > Vector3.Distance(side.transform.position, master.arrowPositionCheck.position))
                {
                    distance = Vector3.Distance(side.transform.position, master.arrowPositionCheck.position);
                    finalSide = side;
                }
            }

            master.currentSide = finalSide.sides;
            return;
        }

        for (int i = 0; i < sideMenus.Length; i++)
        {
            TurretInfo newTI = new TurretInfo();
            newTI.transform = sideMenus[i].StartCheck();
            newTI.sideScript = sideMenus[i].CheckSideScript(true);

            menuPos.Add(sideMenus[i].StartCheck());
            turretSides.Add(newTI);
        }

        for (var i = menuPos.Count - 1; i > -1; i--)
            if (menuPos[i] == null)
                menuPos.RemoveAt(i);

        for (var i = turretSides.Count - 1; i > - 1; i--)
            if (turretSides[i].transform == null)
                turretSides.RemoveAt(i);

        if (spawn)
            SpawnButtons();
    }

    public SideScript ReturnClosestSide (TeleportMaster master)
    {
        List<SideScript> tmpList = new List<SideScript>();
        SideScript finalSide = new SideScript();
        for (int i = 0; i < sideMenus.Length; i++)
        {
            SideScript tmp = sideMenus[i].CheckSideScript(false);
            tmpList.Add(tmp);
        }

        for (var i = tmpList.Count - 1; i > -1; i--)
            if (tmpList[i] == null)
                tmpList.RemoveAt(i);

        float distance = 10;

        foreach (SideScript side in tmpList)
        {
            Debug.DrawLine(side.transform.position, master.previousHit.transform.position, Color.cyan, 5f);
            if (distance > Vector3.Distance(side.transform.position, master.previousHit.transform.position))
            {
                distance = Vector3.Distance(side.transform.position, master.previousHit.transform.position);
                finalSide = side;
            }
        }

        return finalSide;
    }

    void SpawnButtons ()
    {
        for (int i = 0; i < menuPos.Count; i++)
        {
            GameObject newButton = Instantiate(turretButton, menuPos[i]);
            turretButtons.Add(newButton);
            TurretSpawn tmpTurret = newButton.GetComponent<TurretSpawn>();
            tmpTurret.lookAt = Manager.Instance.ReturnPlayer().transform;
            if (Manager.Instance.teleportVersion == Manager.TeleVersion.anywhere)
                tmpTurret.turretSpawnpos = turretSides[i].transform;
            else
                tmpTurret.turretSpawnpos = turretSides[i].sideScript.GetComponentInParent<Transform>();


            tmpTurret.master = this;

            float multiplier = 1.1f;
            float tmp = 10f;

            switch (turretSides[i].sideScript.sides)
            {
                case Sides.up:
                    newButton.transform.position += Vector3.up * multiplier;
                    tmpTurret.rangeOffset = Vector3.up * tmp;

                    tmpTurret.index = 0;
                    if (turretBuilt[0])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                case Sides.down:
                    newButton.transform.position += Vector3.down * multiplier;
                    tmpTurret.rangeOffset = Vector3.down * tmp;

                    tmpTurret.index = 1;
                    if (turretBuilt[1])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                case Sides.front:
                    newButton.transform.position += Vector3.forward * multiplier;
                    tmpTurret.rangeOffset = Vector3.right * tmp;

                    tmpTurret.index = 2;
                    if (turretBuilt[2])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                case Sides.back:
                    newButton.transform.position += Vector3.back * multiplier;
                    tmpTurret.rangeOffset = Vector3.left * tmp;

                    tmpTurret.index = 3;
                    if (turretBuilt[3])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                case Sides.left:
                    newButton.transform.position += Vector3.left * multiplier;
                    tmpTurret.rangeOffset = Vector3.forward * tmp;

                    tmpTurret.index = 4;
                    if (turretBuilt[4])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                case Sides.right:
                    newButton.transform.position += Vector3.right * multiplier;
                    tmpTurret.rangeOffset = Vector3.back * tmp;

                    tmpTurret.index = 5;
                    if (turretBuilt[5])
                    {
                        turretButtons.Remove(newButton);
                        Destroy(newButton);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void SpawnedTurret (int index)
    {
        switch (index)
        {
            case 0:
                turretBuilt[index] = true;
                break;
            case 1:
                turretBuilt[index] = true;
                break;
            case 2:
                turretBuilt[index] = true;
                break;
            case 3:
                turretBuilt[index] = true;
                break;
            case 4:
                turretBuilt[index] = true;
                break;
            case 5:
                turretBuilt[index] = true;
                break;
            default:
                break;
        }
    }

    public void RemoveButtons ()
    {
        foreach (GameObject button in turretButtons)
            Destroy(button);
        turretButtons.Clear();
        turretSides.Clear();
        menuPos.Clear();
    }
}