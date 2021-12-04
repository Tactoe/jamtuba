using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int winCondition = 0;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("CheckForWin");
    }

    IEnumerator CheckForWin()
    {
        while (true)
        {
            if (winCondition == 0)
            {
                // has won
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void increaseWinCondition()
    {
        winCondition++;
    }

    public void decreaseWinCondition()
    {
        winCondition--;
    }
}
