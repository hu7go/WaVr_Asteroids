using UnityEngine;

public class UIManager : MonoBehaviour
{
    //TODO: make it so that i dont have to set it in the inspector, it spawns in when it is needed!
    //The pointer points towards a object if its not visible by the camera!
    [SerializeField] private GameObject pointer;

    private Transform currentTarget;

    private void Update()
    {
        if (currentTarget != null)
        {
            var targetPosLocal = transform.InverseTransformPoint(currentTarget.position);
            var targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
            pointer.transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }

    public void Visible() => pointer.SetActive(false);

    public void NotVisible(Transform newTarget)
    {
        currentTarget = newTarget;
        pointer.SetActive(true);
    }
}