using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sheetBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 1;
    //[SerializeField] float fadeDuration = 1;
    //[SerializeField] GameObject mesh;

    private bool isLaunched = false;
    private float launchedStartTime;
    //private Material material;

    private void Awake()
    {
        //material = mesh.GetComponent<MeshRenderer>().material;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("DelayedLaunchTEST");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            //float alpha = (fadeDuration - (Time.time - launchedStartTime)) / fadeDuration; // normalizing
            //Debug.Log(alpha);
            //if (alpha < 0)
            if(Time.time - launchedStartTime > 1)
            {
                Destroy(gameObject);
            }
            //material.SetColor(0, new Color(1, 1, 1, alpha));
        }
    }

    public void launch()
    {
        //Debug.Log("bouh");
        if (!isLaunched)
        {
            isLaunched = true;
            launchedStartTime = Time.time;
        }
    }

    IEnumerator DelayedLaunchTEST()
    {
        yield return new WaitForSeconds(1);
        launch();
        StopCoroutine("DelayedLaunchTEST");
    }
}
