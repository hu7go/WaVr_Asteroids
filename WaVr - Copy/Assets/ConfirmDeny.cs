using UnityEngine;
using UnityEngine.UI;

public class ConfirmDeny : MonoBehaviour
{
    public enum ConfimDenyEnum
    {
        confirm,
        deny
    }
    public ConfimDenyEnum confirmDeny;
    public Text text;

    private TurretSpawn tmp;

    private void Start()
    {
        switch (confirmDeny)
        {
            case ConfimDenyEnum.confirm:
                text.text = "Buy Turret";
                break;
            case ConfimDenyEnum.deny:
                text.text = "Cancel";
                break;
            default:
                break;
        }
    }

    public void DoEffect ()
    {
        switch (confirmDeny)
        {
            case ConfimDenyEnum.confirm:
                Confrim();
                break;
            case ConfimDenyEnum.deny:
                Deny();
                break;
            default:
                break;
        }
    }

    public void Confrim ()
    {
        tmp = Manager.Instance.GetCurrentBuildTarget();
        tmp.Confirm();
    }

    public void Deny ()
    {
        tmp = Manager.Instance.GetCurrentBuildTarget();
        tmp.Decline();
    }
}