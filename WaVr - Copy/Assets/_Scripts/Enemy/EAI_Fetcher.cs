using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Fetcher : EnemyAI
{
    [SerializeField] private GameObject mesh;

    public override void Movement()
    {
        if (seekAndDestroy == true)
        {
            objective = objectiveOrder[nextTargetIndex].transform;

            if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
            {
                nextTargetIndex++;
            }

            distance = Vector3.Distance(transform.position, objective.position);

            if (distance > range)
                gun.shoot = false;
            else if (distance <= range)
                gun.shoot = true;

            if (distance <= range)
            {
                mesh.transform.localRotation = Quaternion.Lerp(mesh.transform.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime * 1);
            }

            //Stops a certain distance away from the target!
            if (distance > stopDistance)
            {
                onTheWay = true;
                if (distance < range)
                {
                    tmpSpeed = privateSpeed;

                    Quaternion XLookRotation = Quaternion.LookRotation(objective.position, transform.up) * Quaternion.Euler(new Vector3(0, 0, 90));

                    //transform.LookAt(objective, Manager.Instance.GetWorldAxis());
                }
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
            }
            if (distance <= stopDistance)
            {
                onTheWay = false;
            }
        }
        else
        {
            //If seekAndDestroy is false they go back to there home portal!
            distance = Vector3.Distance(transform.position, objective.position);

            if (distance < 2)
            {
                //Should deposite resources instead of killing it self!
                Kill();
                home.Over();
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, objective.position, tmpSpeed * Time.deltaTime);
        }
    }
}
