using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VideoPlayerListener : MonoBehaviour
{
    public abstract void videoEnded(GameObject source);
}
