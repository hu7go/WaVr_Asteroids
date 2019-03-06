﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    public float explosionRadius = 20;
    public float damage = 100;
    public LayerMask layerMask;

    private float timer = 0;
    private float maxTimeAlive = 10f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTimeAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Yaaas" + other.name);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            hitColliders[i].GetComponent<EnemyAI>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
