using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerObservable : MonoBehaviour
{
    [SerializeField] GameObject[] listeners;

    private VideoPlayer player;

    private void Awake()
    {
        player = gameObject.GetComponent<VideoPlayer>();
        if(player == null)
        {
            Debug.Log("No video player to listen, self-destroying the script");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("WaitVideoLength");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addListener(GameObject listener)
    {
        listeners.SetValue(listener, listeners.Length);
    }

    IEnumerator WaitVideoLength()
    {
        if(player.clip != null)
        {
            yield return new WaitForSeconds((float)player.length);
        }
        Debug.Log("video ended");
        foreach (GameObject listener in listeners)
        {
            VideoPlayerListener listenerBehaviour = listener.GetComponent<VideoPlayerListener>();
            if(listenerBehaviour != null)
            {
                listenerBehaviour.videoEnded(gameObject);
            }
        }
    }
}
