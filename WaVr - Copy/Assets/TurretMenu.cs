using UnityEngine;

public class TurretMenu : MonoBehaviour
{
    public Transform playerEye;
    public Transform playerEyeNoPositionTracking;
    public Transform teleAnywherePos;

    RaycastHit hit;
    bool check = false;

    TurretInfo tI = new TurretInfo();

    private void Start()
    {
        if (teleAnywherePos == null)
            teleAnywherePos = gameObject.transform.Find("TeleAnywherePos");
    }

    public bool CheckWhichSideCanSeePlayer (bool spawnTurret = true)
    {
        Vector3 offset = new Vector3(0, 0, 0);
        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide && spawnTurret == false)
            offset = new Vector3(2, 0, 0);

        Vector3 startPoint = transform.position + offset;

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.anywhere)
        {
            startPoint = teleAnywherePos.position;
            tI.transform = teleAnywherePos;
        }

        Transform pos;
        if (playerEye != null)
            pos = playerEye;
        else
            pos = playerEyeNoPositionTracking;

        Debug.DrawLine(startPoint, pos.position, Color.red, 10);

        if (Physics.Linecast(startPoint, pos.position, out hit))
        {
            if (hit.collider.tag == "Player")
            {
                check = true;
            }
            else
            {
                check = false;
            }
        }

        return check;
    }

    public Transform StartCheck ()
    {
        tI.transform = transform;

        if (CheckWhichSideCanSeePlayer())
            return tI.transform;
        else
            return null;
    }

    public SideScript CheckSideScript (bool spawningTurret)
    {
        tI.sideScript = GetComponent<SideScript>();

        if (CheckWhichSideCanSeePlayer(spawningTurret))
            return tI.sideScript;
        else
            return null;
    }
}

[System.Serializable]
public struct TurretInfo
{
    public Transform transform;
    public SideScript sideScript;
}