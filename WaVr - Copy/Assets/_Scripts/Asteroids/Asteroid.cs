using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private bool isObjective = false;
    private bool hasBeenReached = false;

    public void SetObjective ()
    {
        isObjective = true;
    }

    public void Reached ()
    {
        if (isObjective && !hasBeenReached)
        {
            //Manager.Instance.ObjectiveReached();
            hasBeenReached = true;
        }
    }
}