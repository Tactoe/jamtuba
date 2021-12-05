using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject resumeMenu;
    [SerializeField] Slider soundVolume;
    [SerializeField] bool showMenuOnStart;

    private int mainMenuIndex = 1;
    private bool isPlaying = false;
    private bool isPaused = false;
    private bool escapeBufferRead = false;
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
        if (!showMenuOnStart)
        {
            startMenu.SetActive(false);
            canvas.SetActive(false);
        }
        StartCoroutine("CheckForWin");
        resumeMenu.SetActive(false);
        setSoundVolume();
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    pause();
                }
                else
                {
                    resume();
                }
            }
        }
    }

    IEnumerator CheckForWin()
    {
        while (true)
        {
            if (isPlaying && winCondition == 0)
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

    public void play()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            canvas.SetActive(false);
            startMenu.SetActive(false);
            resumeMenu.SetActive(true);
            Time.timeScale = 1;
            SceneManager.LoadScene("Level");
        }
    }

    public void pause()
    {
        FindObjectOfType<PlayerInput>().enabled = false;
        canvas.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        // disable inputs
    }

    public void resume()
    {
        FindObjectOfType<PlayerInput>().enabled = true;
        Time.timeScale = 1;
        isPaused = false;
        // re-enable inputs
        canvas.SetActive(false);
    }

    public void mainMenu()
    {
        canvas.SetActive(true);
        startMenu.SetActive(true);
        resumeMenu.SetActive(false);
        isPaused = false;
        isPlaying = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeMenu");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void toggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void setSoundVolume()
    {
        AudioListener.volume = soundVolume.normalizedValue;
    }

    //Helper
    public static void DrawCircle(Vector3 center, float radius)
    {
        Gizmos.color = Color.white;
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos = center + new Vector3(x, y, 0);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = center + new Vector3(x, y, 0);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}