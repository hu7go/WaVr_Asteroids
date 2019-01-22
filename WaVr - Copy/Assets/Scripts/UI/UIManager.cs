using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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

    public void Visible ()
    {
        pointer.SetActive(false);
    }

    public void NotVisible (Transform newTarget)
    {
        currentTarget = newTarget;
        pointer.SetActive(true);
    }
}
