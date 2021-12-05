using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject resumeMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winBlackBackground;
    [SerializeField] private Image fadeout;
    [SerializeField] GameObject winLogo;
    [SerializeField] Slider soundVolume;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource winSoundPlayer;
    [SerializeField] AudioSource deathSoundPlayer;
    [SerializeField] bool showMenuOnStart;

    private int mainMenuIndex = 1;
    private bool isPlaying = false;
    private bool isPaused = false;
    private bool escapeBufferRead = false;
    private int winCondition = 0;
    private Image winBlackBackgroundImg;

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

        winBlackBackgroundImg = winBlackBackground.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!showMenuOnStart)
        {
            startMenu.SetActive(false);
            canvas.SetActive(false);
        }
        else
        {
            musicPlayer.Play();
        }
        StartCoroutine("CheckForWin");
        resumeMenu.SetActive(false);
        setSoundVolume();
        winScreen.SetActive(false);
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
                win();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Transition()
    {
        fadeout.color = new Color(0, 0, 0, 0);
        fadeout.DOFade(1, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneManager.LoadScene("Scenes/Level_hall");
            fadeout.DOFade(0, 0.4f);
        });
    }

    public void win()
    {
        isPlaying = false;
        FindObjectOfType<PlayerInput>().enabled = false;
        canvas.SetActive(true);
        winScreen.SetActive(true);
        resumeMenu.SetActive(false);
        musicPlayer.Pause();
        winSoundPlayer.Play();
        winBlackBackgroundImg.color = new Color(0, 0, 0, 0);
        winBlackBackgroundImg.DOFade(1, 9.5f).SetEase(Ease.Linear).OnComplete(DisplayWinLogo);
    }

    void DisplayWinLogo()
    {
        winLogo.SetActive(true);
        winLogo.transform.DOScale(0.8f, 9f).OnComplete(() =>
            {
                winLogo.SetActive(false);
                winScreen.SetActive(false);
                mainMenu();
            }
        );
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
        isPlaying = true;
        canvas.SetActive(false);
        startMenu.SetActive(false);
        resumeMenu.SetActive(true);
        Time.timeScale = 1;
        SceneManager.LoadScene("Level");
        if (!musicPlayer.isPlaying)
        {
            musicPlayer.Play();
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

    public void DeathReload()
    {
        winBlackBackgroundImg.color = new Color(0, 0, 0, 0);
        deathSoundPlayer.Play();
        
        winBlackBackgroundImg.DOFade(1, 4f).SetEase(Ease.Linear).OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
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