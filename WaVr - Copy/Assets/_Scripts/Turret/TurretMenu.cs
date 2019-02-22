using UnityEngine;

public class TurretMenu : MonoBehaviour
{
    public Transform playerEye;
    public Transform playerEyeNoPositionTracking;
    public Transform teleAnywherePos;

    public LayerMask layerMask;

    RaycastHit hit;
    bool check = false;

    TurretInfo tI = new TurretInfo();

    private void Start()
    {
        if (teleAnywherePos == null)
            teleAnywherePos = gameObject.transform.Find("TeleAnywherePos");

        playerEye = Manager.Instance.ReturnPlayerBody().transform;
    }

    public bool CheckWhichSideCanSeePlayer (bool spawnTurret = true)
    {
        Vector3 offset = new Vector3(0, 0, 0);
        //This check is done for if its a veersion where you stand of to the side and need to check which side of a asteroid is closest!
        if (Manager.Instance.enums.teleportVersion == Manager.Enums.TeleVersion.arrowsSide && spawnTurret == false)
            offset = new Vector3(2, 0, 0);

        Vector3 startPoint = transform.position + offset;

        if (Manager.Instance.enums.teleportVersion == Manager.Enums.TeleVersion.anywhere)
        {
            startPoint = teleAnywherePos.position;
            tI.transform = teleAnywherePos;
        }

        Transform pos;
        if (playerEye != null)
            pos = playerEye;
        else
            pos = playerEyeNoPositionTracking;

        if (Physics.Linecast(startPoint, pos.position, out hit, ~layerMask))
        {
            if (hit.collider.CompareTag("Player"))
                check = true;
            else
                check = false;
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

    public SideScript CheckSideScript (bool spawningTurret, bool tmp = false)
    {
        tI.sideScript = GetComponent<SideScript>();

        if (tmp)
            return tI.sideScript;

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