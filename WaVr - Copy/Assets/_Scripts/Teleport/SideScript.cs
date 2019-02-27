using UnityEngine;

public class SideScript : MonoBehaviour
{
    public Sides sides;

    private Asteroid parent;
    [SerializeField] private Transform teleportPos;
    public Transform rotator;
    public Vector3 sideOffset;
    public bool highlightBool = false;
    public MeshRenderer highlight;
    private Transform player;

    private float distance;
    private float maxDistance;

    private void Start()
    {
        parent = GetComponentInParent<Asteroid>();
        player = Manager.Instance.ReturnPlayer().transform;
        maxDistance = Manager.Instance.tpMaster.GetMaxLenght();
    }

    void Update()
    {
        if (highlightBool)
        {
            //The + 3 is the radius of the sphere collider!
            distance = (transform.position - player.position).magnitude - 3;

            //This might be not good at all!
            //Probably(maybe) is better to do once when you teleport!
            if (distance <= maxDistance)
                highlight.enabled = true;
            else
                highlight.enabled = false;
        }
    }

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