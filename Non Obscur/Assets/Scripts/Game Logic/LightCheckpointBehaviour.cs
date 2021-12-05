using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheckpointBehaviour : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float distanceCompletionThreshold = 2f;
    [SerializeField] GameObject[] enemies;

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
        GameManager.instance.decreaseWinCondition();
        pointLight.enabled = true;
        foreach(GameObject enemy in enemies)
        {
            ZombieBehavior zombieBehavior = enemy.GetComponent<ZombieBehavior>();
            if(zombieBehavior != null)
            {
                zombieBehavior.Die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        GameManager.DrawCircle(transform.position, distanceCompletionThreshold);
    }
}