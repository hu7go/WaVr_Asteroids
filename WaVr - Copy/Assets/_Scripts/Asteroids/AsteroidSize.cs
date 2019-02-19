using UnityEngine;

public class AsteroidSize : MonoBehaviour
{
    private float scaleSize = 5;
    private float minRot = - 22;
    private float maxRot = 22;

    [SerializeField] private bool controlledRotation;
    [SerializeField] private bool reversedRotation;

	void Start ()
    {
        scaleSize = Manager.Instance.RetAsteroidSize();
        gameObject.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);

        if (controlledRotation)
            transform.rotation = Quaternion.Euler(RandomNumber(), RandomNumber(), RandomNumber());
        if(reversedRotation)
            transform.rotation = Quaternion.Euler(RandomNumber(), 180 +RandomNumber(),RandomNumber());
    }

    private float RandomNumber () => Random.Range(minRot, maxRot);
}