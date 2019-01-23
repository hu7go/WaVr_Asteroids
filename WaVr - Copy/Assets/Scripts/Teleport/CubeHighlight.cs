using UnityEngine;

public class CubeHighlight : MonoBehaviour
{
    MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    public void Render ()
    {
        mr.enabled = true;
    }

    public void StopRender ()
    {
        mr.enabled = false;
    }
}