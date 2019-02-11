using UnityEngine;

public class Probe : MonoBehaviour
{
    public float speed = 3;
    private Vector3 target;
    private Vector3 secondaryTarget;
    private float distance;

    private bool probe = true;

    public void Instantiate (Vector3 newPos, Vector3 secondary)
    {
        target = newPos;
        secondaryTarget = secondary;
    }

    private void Update()
    {
        transform.LookAt(target);
        distance = Vector3.Distance(transform.position, target);

        if (probe)
        {

            if (distance > 4)
                transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            else
            {
                Vector3 tmp = target;
                target = secondaryTarget;
                secondaryTarget = tmp;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            if (distance < 2)
            {
                speed *= 2;
                if (distance < 1)
                    Destroy(gameObject);
            }
        }
    }

    public void Return(Vector3 pos)
    {
        probe = false;
        target = pos;
    }
}