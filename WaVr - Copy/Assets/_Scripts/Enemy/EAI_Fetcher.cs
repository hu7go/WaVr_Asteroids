using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Fetcher : EnemyAI
{
    [SerializeField] private GameObject mesh;
    [SerializeField] private ParticleSystem particles;
    public float totalHealthStealing = 50;
    public float stolenHealth;

    private bool takenEnough = false;

    public override void ShootingBehaviour()
    {
        if (distance <= range && gun.shoot == false)
        {

        }
        else if (distance > range && gun.shoot == true)
        {
            gun.shoot = false;
            gun.StopShooting();
        }
    }

    public override void Movement()
    {
        if (stolenHealth >= totalHealthStealing)
            takenEnough = true;
        else
            takenEnough = false;

        if (seekAndDestroy == true && takenEnough == false)
        {
            if (gun.shoot)
                particles.Play();
            else
                particles.Stop();

            objective = objectiveOrder[nextTargetIndex];

            if (objectiveOrder[nextTargetIndex].asteroid.alive == false)
            {
                nextTargetIndex++;
            }

            distance = Vector3.Distance(transform.position, objective.transform.position);

            if (distance <= range)
            {
                mesh.transform.localRotation = Quaternion.Lerp(mesh.transform.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime * 1);
            }

            if (mesh.transform.localRotation == Quaternion.Lerp(mesh.transform.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime * 1) && gun.shoot == false)
            {
                gun.StartShooting(waveIndex, home, objective.GetComponent<AsteroidHealth>(), this);
                gun.shoot = true;
            }

            //Stops a certain distance away from the target!
            if (distance > stopDistance)
            {
                onTheWay = true;
                if (distance < range)
                {
                    tmpSpeed = privateSpeed;

                    Quaternion XLookRotation = Quaternion.LookRotation(objective.transform.position, transform.up) * Quaternion.Euler(new Vector3(0, 0, 90));

                    //transform.LookAt(objective, Manager.Instance.GetWorldAxis());
                }
                transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
            }
            if (distance <= stopDistance)
            {
                onTheWay = false;
            }
        }
        else if (seekAndDestroy == false && takenEnough == false)
        {
            //If seekAndDestroy is false they go back to there home portal!
            distance = Vector3.Distance(transform.position, objective.transform.position);

            if (distance < 2)
            {
                //Should deposite resources instead of killing it self!
                Kill();
                home.Over();
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
        }
        else if (takenEnough == true)
        {
            gun.StopShooting();
            gun.shoot = false;
            particles.Stop();

            distance = Vector3.Distance(transform.position, objective.transform.position);

            mesh.transform.localRotation = Quaternion.Lerp(mesh.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1);

            if (mesh.transform.localRotation == Quaternion.Lerp(mesh.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1))
            {
                objective = home.GetComponent<AsteroidHealth>();

                if (distance < 2)
                {
                    stolenHealth = 0;
                    objective = objectiveOrder[nextTargetIndex];
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, objective.transform.position, tmpSpeed * Time.deltaTime);
                }
            }


            transform.LookAt(objective.transform);
        }
    }
}
