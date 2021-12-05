using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoPlayerListenerMenu : VideoPlayerListener
{
    public override void videoEnded(GameObject source)
    {
        //GameManager.instance.mainMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
