using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour
{
    public float speed = 3;
    private List<AsteroidHealth> targets;
    private Vector3 currentTarget;
    private Vector3 secondaryTarget;
    private float distance;
    private float number;

    private float startSpeed;

    private bool probe = true;
    private bool forward = true;

    int index = 0;

    public void Instantiate (List<AsteroidHealth> newPosOrder, Vector3 secondary, float newNumber)
    {
        startSpeed = speed;
        targets = newPosOrder;
        currentTarget = targets[0].asteroid.postition;
        secondaryTarget = secondary;
        number = newNumber;
    }

    private void Update()
    {
        transform.LookAt(currentTarget);
        distance = Vector3.Distance(transform.position, currentTarget);

        if (distance < 6)
            speed = startSpeed * .75f;
        if (distance < 5)
            speed = startSpeed * .5f;
        if (distance > 5)
            speed = startSpeed * .75f;
        if (distance > 6)
            speed = startSpeed;

        if (probe)
        {
            if (distance > 4)
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            else
            {
                if (forward)
                    index++;
                else
                    index--;

                if (index >= number - 1)
                    forward = false;
                else if (index <= 0)
                    forward = true;

                currentTarget = targets[index].asteroid.postition;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
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
        currentTarget = pos;
    }
}