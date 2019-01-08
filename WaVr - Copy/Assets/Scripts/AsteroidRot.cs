using UnityEngine;

public class AsteroidRot : MonoBehaviour
{
    public SideScript[] children;
    private Transform portPos;

    public Transform ReturnCurrentTeleportPos (Sides playerSide)
    {
        switch (playerSide)
        {
            case Sides.up:
                portPos = children[0].TeleportPosition();
                break;
            case Sides.down:
                portPos = children[1].TeleportPosition();
                break;
            case Sides.front:
                portPos = children[2].TeleportPosition();
                break;
            case Sides.back:
                portPos = children[3].TeleportPosition();
                break;
            case Sides.left:
                portPos = children[4].TeleportPosition();
                break;
            case Sides.right:
                portPos = children[5].TeleportPosition();
                break;
            default:
                break;
        }
        return portPos;
    }
}