using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private Rigidbody rb;
    private bool hasBeenLaunched = false;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hasBeenLaunched && direction != null && direction.magnitude != 0)
        {
            hasBeenLaunched = true;
            rb.AddForce(direction, ForceMode.Impulse);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Zombie") && !other.CompareTag("Waypoint"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<EnergyManager>().GetHit();
            }

            Destroy(gameObject);
        }
    }

}