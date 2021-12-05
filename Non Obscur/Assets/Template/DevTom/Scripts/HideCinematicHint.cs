using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideCinematicHint : MonoBehaviour
{
    [SerializeField] float hintDisplayDuration = 0.5f;
    [SerializeField] float pressTimeRequired = 0.5f;
    [SerializeField] GameObject hint;

    private float hintCooldown;
    private float skipCooldown = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            hintCooldown = hintDisplayDuration;
            hint.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            skipCooldown += Time.deltaTime;
            if(skipCooldown >= pressTimeRequired)
            {
                SceneManager.LoadScene("HomeMenu");
            }
        }
        else
        {
            skipCooldown = 0;
        }
        if(hintCooldown <= 0)
        {
            hint.SetActive(false);
        }
        else
        {
            hintCooldown -= Time.deltaTime;
        }
    }
}
