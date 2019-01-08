using UnityEngine;

public class GhostLine : MonoBehaviour
{
    LineRenderer line;
    CapsuleCollider capsule;
    TeleportMaster tpm;
    public GameObject teleportMaster;

    private SideScript previousSide;

    public float LineWidth;

    void Start()
    {
        tpm = teleportMaster.GetComponent<TeleportMaster>();
        line = GetComponent<LineRenderer>();
        capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = LineWidth * 2;
        capsule.center = Vector3.zero;
        capsule.direction = 2;
        capsule.tag = "GhostLine";
    }

    public void UpdateLine ()
    {
        if (tpm.currentHit == null)
            return;

        if (tpm.currentHit != null)
            line.SetPosition(0, tpm.currentHit.transform.position);

        if(tpm.previousHit != null)
            line.SetPosition(1, tpm.previousHit.transform.position);

        previousSide = tpm.previousHit;
        if (tpm.previousHit != null)
            capsule.transform.position = tpm.currentHit.transform.position + (tpm.previousHit.transform.position - tpm.currentHit.transform.position) / 2;
        capsule.transform.LookAt(tpm.currentHit.transform.position);
        if (tpm.previousHit != null)
            capsule.height = (tpm.previousHit.transform.position - tpm.currentHit.transform.position).magnitude;
    }
}