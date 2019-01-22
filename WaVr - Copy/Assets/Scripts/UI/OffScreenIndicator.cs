using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public UIManager uiManager;

    //! Both these functions will trigger if you look at the obj with the unity editor scene camera!
    private void OnBecameVisible()
    {
        Debug.Log("<color=green>I am visible!</color>");
        uiManager.Visible();
    }

    private void OnBecameInvisible()
    {
        Debug.Log("<color=red>I am not currently visible!</color>");
        uiManager.NotVisible(transform);
    }
}
