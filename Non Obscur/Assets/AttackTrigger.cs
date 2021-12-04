using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TriggerShoot()
    {
        GetComponentInParent<ZombieBehavior>().Shoot();
    }
}