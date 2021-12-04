using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheckpointBehaviour : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float distanceCompletionThreshold = 2f;
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
        StartCoroutine("CheckForPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // check if player is near the light
    // coroutine stops itself and enable light if so
    IEnumerator CheckForPlayer()
    {
        while (true)
        {
            if ((player.transform.position - transform.position).magnitude <= distanceCompletionThreshold)
            {
                EnableCheckpoint();
                StopCoroutine("CheckForPlayer");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void EnableCheckpoint()
    {
        GameManager.instance.increaseWinCondition();
        pointLight.enabled = true;
        if(sheet != null)
        {
            sheet.GetComponent<sheetBehaviour>().launch();
        }
        foreach(GameObject enemy in enemies)
        {
            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            if(enemyBehaviour != null)
            {
                enemyBehaviour.die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        GameManager.DrawCircle(transform.position, distanceCompletionThreshold);
    }
}
