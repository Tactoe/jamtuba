using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

//--------------------------------------------------------------------
//Dash module is a movement ability
//--------------------------------------------------------------------
public class DashModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_DashStrength = 0.0f;
    [SerializeField] float m_DashCooldown = 0.0f;
    [SerializeField] float m_DashMaxDuration = 0.0f;

    [SerializeField] bool m_ResetDashsAfterTouchingWall = false;
    [SerializeField] bool m_ResetDashsAfterTouchingEdge = false;
    [SerializeField] bool m_OverridePreviousSpeed = false;

    float m_LastDashTime;
    bool m_HasDashedAndNotTouchedGroundYet;
    float m_DashDuration;

    private Vector2 m_VelocityBeforeDash;
    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_LastDashTime = Time.fixedTime - m_DashCooldown;
        m_DashDuration = 0;
        print("Doing restet");
        m_HasDashedAndNotTouchedGroundYet = false;
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

        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        GameObject.Find("DashParticle").GetComponent<ParticleSystem>().Play();
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
        GameObject.Find("DashParticle").GetComponent<ParticleSystem>().Stop();

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

        if (m_DashDuration >= m_DashMaxDuration)
        {
            print("OVERDOSE");
            return false;
        }
        if (!DoesInputExist("Aim") || !DoesInputExist("Dash"))
        {
            Debug.LogError("Input for module " + GetName() + " not set up");
            return false;
        }
        if (GetDirInput("Aim").HasSurpassedThreshold() && GetButtonInput("Dash").m_WasJustPressed)
        {
            return true;
        }
        
        return false;
    }
}