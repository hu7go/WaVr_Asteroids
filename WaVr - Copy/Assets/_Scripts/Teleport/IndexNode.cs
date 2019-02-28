using UnityEngine;

public class IndexNode : MonoBehaviour
{
    public enum Index
    {
        one,
        two,
        three,
        four
    }

    [SerializeField] private Index indexEnum;
    private SphereCollider sCollider;
    private int myIndex;
    public int index
    {
        get
        {
            return myIndex;
        }
    }

    private void Start()
    {
        sCollider = GetComponent<SphereCollider>();
        if (Manager.Instance.enums.pointerState != Manager.Enums.PointerState.Teleport)
            Manager.Instance.SetPointerState(Manager.Enums.PointerState.Teleport);
        Off();
        switch (indexEnum)
        {
            case Index.one:
                myIndex = 1;
                break;
            case Index.two:
                myIndex = 2;
                break;
            case Index.three:
                myIndex = 3;
                break;
            case Index.four:
                myIndex = 4;
                break;
        }
    }

    public void On() => sCollider.enabled = true;

    public void Off () => sCollider.enabled = false;
}