using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoorBehaviour : MonoBehaviour
{
    [SerializeField] GameObject finishHint;

    private LightCheckpointBehaviour[] checkpoints;
    private bool canFinish = false;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = FindObjectsOfType<LightCheckpointBehaviour>();
        finishHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canFinish = true;
            foreach (LightCheckpointBehaviour checkpoint in checkpoints)
            {
                if (!checkpoint.isCompleted())
                {
                    canFinish = false;
                    break;
                }
            }
            if (canFinish)
            {
                finishHint.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finishHint.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canFinish && Input.GetKeyDown(KeyCode.N))
            {
                GameManager.instance.win();
            }
        }
    }
}
