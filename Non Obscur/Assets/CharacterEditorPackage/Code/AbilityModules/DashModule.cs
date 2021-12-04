using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

//--------------------------------------------------------------------
//Dash module is a movement ability
//--------------------------------------------------------------------
public class DashModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_DashStrength = 0.0f;
    [SerializeField] float m_DashCooldown = 0.0f;
    [SerializeField] float m_DashMaxDuration = 0.0f;
    [SerializeField] float m_CameraShakeDuration = 0.0f;
    [SerializeField] float m_CameraShakeStrength = 0.0f;
    [SerializeField] float m_BloomMaxStrength = 0.0f;
    [SerializeField] float m_BloomFadeOutTime = 0.0f;
    [SerializeField] float m_DistortionMaxStrength = 0.0f;

    [SerializeField] bool m_ResetDashsAfterTouchingWall = false;
    [SerializeField] bool m_ResetDashsAfterTouchingEdge = false;
    [SerializeField] bool m_OverridePreviousSpeed = false;


    private Volume m_DashVolume;
    private ParticleSystem m_DashParticle;
    float m_LastDashTime;
    bool m_HasDashedAndNotTouchedGroundYet;
    float m_DashDuration;

    private Vector2 m_VelocityBeforeDash;
    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_LastDashTime = Time.fixedTime - m_DashCooldown;
        m_DashDuration = 0;
        m_HasDashedAndNotTouchedGroundYet = false;
    }

    private void Start()
    {
        m_DashVolume = GameObject.Find("DashVolume").GetComponent<Volume>();
        m_DashParticle = GameObject.Find("DashParticle").GetComponent<ParticleSystem>();
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_LastDashTime = Time.fixedTime;
        m_DashDuration = 0;
        m_HasDashedAndNotTouchedGroundYet = true;
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        Vector2 direction = GetDirInput("Aim").m_ClampedInput.normalized;
        direction.x = direction.x > 0 ? 1 : -1;
        direction.y = 0;

        m_DashParticle.Play();
        Volume ok = FindObjectOfType<Volume>();
        Bloom bloo;
        LensDistortion distortion;
        ok.profile.TryGet(out bloo);
        ok.profile.TryGet(out distortion);
        DOTween.To(x => bloo.intensity.value = x, 0f, m_BloomMaxStrength, 0.05f).OnComplete(() => DOTween.To(x => bloo.intensity.value = x, m_BloomMaxStrength, 0f, m_BloomFadeOutTime));
        DOTween.To(x => distortion.intensity.value = x, 0f, m_DistortionMaxStrength, 0.05f).OnComplete(() => DOTween.To(x => distortion.intensity.value = x, m_DistortionMaxStrength, 0f, m_BloomFadeOutTime));
        //DOTween.To(x => bloo.intensity.value = x, 0f, m_BloomMaxStrength, 0.05f).SetLoops(1, LoopType.Yoyo);
        

        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        StartCoroutine(ResetDash());
        if (m_OverridePreviousSpeed)
        {
            currentVel = Vector2.zero;
        }
        Vector2 jumpVelocity = direction * m_DashStrength;

        m_VelocityBeforeDash = currentVel;
        currentVel += jumpVelocity;

        m_ControlledCollider.UpdateWithVelocity(currentVel);
    }

    IEnumerator ResetDash()
    {
        while (m_DashDuration < m_DashMaxDuration)
        {
            m_DashDuration += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        m_DashParticle.Stop();

        m_DashDuration = 0;
        print("Done dashing");
        m_ControlledCollider.UpdateWithVelocity(m_VelocityBeforeDash);
        
    }

    //Called whenever this module is inactive and updating (implementation by child modules), useful for cooldown updating etc.
    public override void InactiveUpdateModule()
    {
        if (m_ControlledCollider.IsGrounded() ||
           (m_ControlledCollider.IsPartiallyTouchingWall() && m_ResetDashsAfterTouchingWall) ||
           (m_ControlledCollider.IsTouchingEdge() && m_ResetDashsAfterTouchingEdge))
        {
            m_HasDashedAndNotTouchedGroundYet = false;
        }
    }

    public bool CanStartDash()
    {
        if (Time.fixedTime - m_LastDashTime < m_DashCooldown || m_HasDashedAndNotTouchedGroundYet || !GetDirInput("Aim").HasSurpassedThreshold())
        {
            return false;
        }
        return true;
    }
    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable()
    {
        if (Time.fixedTime - m_LastDashTime < m_DashCooldown || m_HasDashedAndNotTouchedGroundYet)
        {
            return false;
        }

        if (!DoesInputExist("Aim") || !DoesInputExist("Dash"))
        {
            Debug.LogError("Input for module " + GetName() + " not set up");
            return false;
        }
        if (GetDirInput("Aim").HasSurpassedThreshold() && GetButtonInput("Dash").m_WasJustPressed)
        {
            //Camera.main.DOShakePosition(m_CameraShakeDuration, m_CameraShakeStrength);
            return true;
        }
        
        return false;
    }
}