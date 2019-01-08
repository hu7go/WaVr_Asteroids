using UnityEngine;

public class TurnOnDepthBuffer : MonoBehaviour
{
	void Start ()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
}