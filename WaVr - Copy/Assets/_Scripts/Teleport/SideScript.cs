using UnityEngine;

public class SideScript : MonoBehaviour
{
    public Sides sides;

    private Asteroid parent;
    [SerializeField] private Transform teleportPos;
    public Transform rotator;
    public Vector3 sideOffset;

    private void Start() => parent = GetComponentInParent<Asteroid>();

    public void Reached () => parent.Reached();

    public Transform TeleportPosition () => teleportPos;
}

public enum Sides
{
    up,
    down,
    front,
    back,
    left,
    right,
}