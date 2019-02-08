using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    private UIManager uiManager;

    private void Start()
    {
        uiManager = Manager.Instance.uiManager;
    }

    //! Both these functions will trigger if you look at the obj with the unity editor scene camera!
    //Fix!!!!!
    //private void OnBecameVisible()
    //{
    //    uiManager.Visible();
    //}

    //private void OnBecameInvisible()
    //{
    //    uiManager.NotVisible(transform);
    //}
}
