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
    private bool on = true;
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
        OnOff();
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

    public void OnOff()
    {
        if (on)
            sCollider.enabled = false;
        else
            sCollider.enabled = true;

        on = !on;
    }
}
