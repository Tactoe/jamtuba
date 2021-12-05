using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheckpointBehaviour : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject sheet;

    private Light pointLight;

    private void Awake()
    {
        pointLight = gameObject.GetComponent<Light>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pointLight.enabled = false;
        GameManager.instance.increaseWinCondition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // check if player is near the light
    // coroutine stops itself and enable light if so

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Got player");
                EnableCheckpoint();
        }
    }

    void EnableCheckpoint()
    {
        GameManager.instance.decreaseWinCondition();
        pointLight.enabled = true;
        if(sheet != null)
        {
            sheet.GetComponent<sheetBehaviour>().launch();
        }
        foreach(GameObject enemy in enemies)
        {
            ZombieBehavior zombieBehavior = enemy.GetComponent<ZombieBehavior>();
            if(zombieBehavior != null)
            {
                zombieBehavior.Die();
            }
        }
    }
}