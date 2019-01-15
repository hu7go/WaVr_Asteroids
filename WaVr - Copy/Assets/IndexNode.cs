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
    public int index;

    private void Start()
    {
        switch (indexEnum)
        {
            case Index.one:
                index = 1;
                break;
            case Index.two:
                index = 2;
                break;
            case Index.three:
                index = 3;
                break;
            case Index.four:
                index = 4;
                break;
        }
    }
}
