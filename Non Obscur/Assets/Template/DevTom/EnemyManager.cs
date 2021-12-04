using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{ 
    public GameObject[] Enemies;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckForEnemies()
    {
        bool atLeastOneEnemyAlive = false;
        while (!atLeastOneEnemyAlive)
        {
            foreach (GameObject enemy in Enemies)
            {
                if (enemy != null)
                {
                    atLeastOneEnemyAlive = true;
                    break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}