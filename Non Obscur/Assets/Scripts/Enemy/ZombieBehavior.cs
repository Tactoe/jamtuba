using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieBehavior : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_SightRadius;
    [SerializeField] private float m_LoseSightRadius;
    [SerializeField] private float m_AttackRadius;
    [SerializeField] private int m_CurrentDirection;
    [SerializeField] private GameObject m_Projectile;
    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private float m_ProjectileJumpForce;
    [SerializeField] private float m_stunDuration;
    
    private Rigidbody m_RB;
    private Animator m_Animator;

    private Transform m_Target;
    private enum ZombieState {Patrol, Chasing, Attacking, Stunned};

    [SerializeField] private ZombieState m_CurrentState;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RB = GetComponent<Rigidbody>();
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateState(ZombieState.Patrol);
    }
    
    void Update()
    {
        if (m_Target == null)
        {
            m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (m_CurrentState == ZombieState.Attacking || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                UpdateState(ZombieState.Chasing);
            }
        }
        else if (m_CurrentState == ZombieState.Chasing)
        {
            int previousDirection = m_CurrentDirection;
            m_CurrentDirection = m_Target.transform.position.x > transform.position.x ? 1 : -1;
            if (previousDirection != m_CurrentDirection)
                transform.DORotate(transform.rotation.eulerAngles * Vector2.up * -1, 0.2f);
            transform.position += Vector3.right * Time.deltaTime * m_CurrentDirection * m_MoveSpeed;
        }
        else if (m_CurrentState == ZombieState.Patrol)
        {
            transform.position += Vector3.right * Time.deltaTime * m_CurrentDirection * m_MoveSpeed;
        }
    }

    void UpdateState(ZombieState newState)
    {
        m_CurrentState = newState;
        switch (newState)
        {
            case ZombieState.Patrol:
                StartCoroutine(CheckForHero());
                break;
            case ZombieState.Chasing:
                StartCoroutine(ChaseHero());
                break;
            case ZombieState.Stunned:
                StartCoroutine(Stunned());
                break;
        }
    }

    IEnumerator CheckForHero()
    {
        bool hasFoundHero = false;
        while (!hasFoundHero)
        {
            if (Vector3.Distance(transform.position, m_Target.transform.position) < m_SightRadius)
            {
                hasFoundHero = true;
                UpdateState(ZombieState.Chasing);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ChaseHero()
    {
        bool heroGotAway = false;
        while (!heroGotAway)
        {
            if (Vector3.Distance(transform.position, m_Target.transform.position) < m_AttackRadius && m_CurrentState != ZombieState.Stunned)
            {
                m_Animator.SetTrigger("Attack");
                UpdateState(ZombieState.Attacking);
            }

            if (Vector3.Distance(transform.position, m_Target.transform.position) > m_LoseSightRadius)
            {
                heroGotAway = true;
                UpdateState(ZombieState.Patrol);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Stunned()
    {
        Debug.Log("stunned");
        
        yield return new WaitForSeconds(m_stunDuration);
        UpdateState(ZombieState.Patrol);
    }
    
    public void Shoot()
    {
        GetComponent<AudioSource>().Play();
        GameObject projectile = Instantiate(m_Projectile, transform.position, Quaternion.identity);
        //projectile.GetComponent<Rigidbody>().DOJump(m_Target.position, 2, 1, m_ProjectileSpeed);
        float heightOffset = Random.Range(-2, 2);
        float speedOffset = Random.Range(-2, 2);
        Vector2 projectionDirection = new Vector2(m_CurrentDirection * (m_ProjectileSpeed + speedOffset), m_Target.position.y + m_ProjectileJumpForce + heightOffset);
        projectile.GetComponent<Rigidbody>().AddForce(projectionDirection, ForceMode.Impulse);
    }
    // Update is called once per frame

    public void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void stun()
    {
        StopAllCoroutines();
        UpdateState(ZombieState.Stunned);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            if (m_CurrentState == ZombieState.Patrol)
            {
                print("Going back");
                m_CurrentDirection *= -1;
                transform.DORotate(transform.rotation.eulerAngles * Vector2.up * -1, 0.4f);
            }
            else
            {
                
            }
        }
    }
}