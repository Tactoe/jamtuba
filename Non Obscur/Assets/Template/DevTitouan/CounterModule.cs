using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

//--------------------------------------------------------------------
//Dash module is a movement ability
//--------------------------------------------------------------------
public class CounterModule : GroundedControllerAbilityModule
{
    
    [SerializeField] float m_CounterMaxRange = 0.0f;
    [SerializeField] float m_PerfectCounterMaxRange = 0.0f;
    [SerializeField] float m_CounterMaxActiveTime = 0.0f;
    [SerializeField] private bool m_CounterIsActive;
    

    private ParticleSystem m_PerfectCounterPS;
    private ParticleSystem m_CounterPS;
    private bool m_WaitForFirstInput = true;

    private float m_CounterActiveTime;
    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_CounterIsActive = false;
        m_CounterActiveTime = 0;
    }

    private void Start()
    {
        m_PerfectCounterPS = GameObject.Find("PerfectCounterParticle").GetComponent<ParticleSystem>();
        m_CounterPS = GameObject.Find("CounterParticle").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule perfectCounterMain = m_PerfectCounterPS.main;
        perfectCounterMain.startSize = m_PerfectCounterMaxRange * 2;
        perfectCounterMain.startLifetime = m_CounterMaxActiveTime;
        ParticleSystem.MainModule counterMain = m_CounterPS.main;
        counterMain.startSize = m_CounterMaxRange * 2;
        counterMain.startLifetime = m_CounterMaxActiveTime;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_CounterIsActive = true;
        m_PerfectCounterPS.Play();
        m_CounterPS.Play();
        m_CounterActiveTime = 0;
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        GameObject[] currentProjectiles = GameObject.FindGameObjectsWithTag("projectile");
        foreach (GameObject projectile in currentProjectiles)
        {
            if (Vector3.Distance(projectile.transform.position, m_ControlledCollider.transform.position) <
                m_PerfectCounterMaxRange)
            {
                print("did PERFECT COUNTER");
                Destroy(projectile);
                
            }
            else if (Vector3.Distance(projectile.transform.position, m_ControlledCollider.transform.position) < m_CounterMaxRange)
            {
                print("did COUNTER");
                Destroy(projectile);
            }
        }
        m_CounterActiveTime += Time.deltaTime;
    }

    private void Update()
    {
        Vector3 startPos = m_CharacterController.transform.position;
        Debug.DrawLine(startPos, startPos + Vector3.right * m_PerfectCounterMaxRange, Color.green);
        startPos += Vector3.up * 0.1f; 
        Debug.DrawLine(startPos, startPos + Vector3.right * m_CounterMaxRange, Color.blue);
    }


    //Called whenever this module is inactive and updating (implementation by child modules), useful for cooldown updating etc.
    public override void InactiveUpdateModule()
    {
    }

    public bool CanStartDash()
    {
        return true;
    }
    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable()
    {
        if (!DoesInputExist("Counter"))
        {
            Debug.LogError("Input for module " + GetName() + " not set up");
            return false;
        }
        if (GetButtonInput("Counter").m_WasJustPressed)
        {
            m_WaitForFirstInput = false;
            //Camera.main.DOShakePosition(m_CameraShakeDuration, m_CameraShakeStrength);
            return true;
        }
        if (m_WaitForFirstInput)
            return false;

        return m_CounterActiveTime < m_CounterMaxActiveTime;
    }
}