using UnityEngine;

public class GunOnOff : MonoBehaviour
{
    [SerializeField] private GameObject vrtk;
    [SerializeField] private bool usingDayDream = false;
    [SerializeField] private Transform gunPos;
    SpaceGun spacegun;

    void Awake ()
    {
        spacegun = GetComponent<SpaceGun>();
        gameObject.SetActive(false);
    }

    public void RefreshGun()
    {
        if (Manager.Instance.nmbrOfPizzas == 5)
        {
            gameObject.SetActive(true);

            if (Manager.Instance.usingDayDream)
                usingDayDream = true;

            spacegun.isGrabbed = true;
            Manager.Instance.HoldingGun();
            if (usingDayDream)
            {
                gameObject.transform.position = gunPos.position;
                return;
            }
            gameObject.transform.position = vrtk.transform.position;
        }
    }
}