using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Steps;
    [SerializeField] private AudioClip[] m_Dash;

    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void FootStep()
    {
        int index = Random.Range(0, m_Steps.Length);
        m_AudioSource.PlayOneShot(m_Steps[index]);
    }
    
    public void Dash()
    {
        int index = Random.Range(0, m_Dash.Length);
        m_AudioSource.PlayOneShot(m_Dash[index]);
    }
}