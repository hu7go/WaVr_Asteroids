using UnityEngine;
using VRTK;

public class Highlight : MonoBehaviour
{
    DestinationMarkerEventArgs pointer;

    public void HighlightObj (object hit, DestinationMarkerEventArgs point)
    {
        point.target.GetComponentInChildren<MeshRenderer>().enabled = true;
    }
}