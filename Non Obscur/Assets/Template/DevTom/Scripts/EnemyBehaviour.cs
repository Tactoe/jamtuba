using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject projectile;

    [SerializeField] float walking_speed = 2;
    [SerializeField] float patrollingErrorThreshold = 1;
    [SerializeField] float patrolTourSize = 20f;

    [SerializeField] float following_speed = 4;
    [SerializeField] float targeting_radius;
    [SerializeField] float fov_vision = 0.3f;
    [SerializeField] float firerate = 1;

    [SerializeField] float collision_damaging_radius;

    [SerializeField] float stunDuration;

    private TargetBehaviour targetBehaviour;
    private int state = 0; // 0 - idle/patrolling ; 1 - targeting ; 2 - stunned
    private float stunCooldown = 0;
    private Material material;
    private Vector2 selfToTarget;

    //navigation
    private Vector2 initPos;
    private Vector2 lastPatrolPos;
    private Vector2 patrolDirection;

    //movement
    private Vector2 lastPos;
    private Vector2 direction;
    private float speed;

    private float fireCooldDown = 0;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        targetBehaviour = target.GetComponent<TargetBehaviour>();
        material = gameObject.GetComponent<Material>();
    }

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<GroundedCharacterController>().gameObject;
        initPos = toVector2(transform.position);
        lastPos = initPos;
        lastPatrolPos = initPos;
        patrolDirection = (Random.value > 0.5f) ? Vector2.left : Vector2.right;
        //StartCoroutine("UpdateState");
    }

    // Update is called once per frame
    void Update()
    {

        selfToTarget = target.transform.position - transform.position;
        switch (state)
        {
            case 0:
                Patrol();
                if (isTargetSeen())
                {
                    state = 1;
                }
                break;
            case 1:
                if (isTargetSeen())
                {
                    FollowAndDamageTarget();
                }
                else
                {
                    state = 0;
                }
                break;
            case 2:
                stunCooldown -= Time.deltaTime;
                break;
        }
        move();
        lastPos = toVector2(transform.position);
        //yield return new WaitForSeconds(0.1f);
    }

    bool isTargetSeen()
    {
        float distance = selfToTarget.magnitude;
        //Debug.Log(distance);
        if (distance <= targeting_radius)
        {
            return DoSeeTarget();
        }
        return false;
    }

    void FollowAndDamageTarget()
    {
        Debug.Log("following");
        m_Animator.SetTrigger("Attack");
        if(selfToTarget.magnitude <= collision_damaging_radius)
        {
            targetBehaviour.GetDamage(1f);
        }
        else
        {
            //Vector2 movement = selfToTarget;
            //movement.y = 0;
            //movement = movement.normalized * following_speed * Time.deltaTime;
            float dX = 0;
            if (transform.position.x > target.transform.position.x)
            {
                dX = -1;
            }
            else
            {
                dX = 1;
            }
            lastPos = transform.position;
            direction = new Vector2(dX, 0);
            speed = following_speed;
            //transform.position += toVector3(direction);
        }
    }

    void Shoot()
    {
        if(fireCooldDown <= 0)
        {
            Vector2 projectionDirection = direction + Vector2.up * 0.8f;
            projectionDirection.Normalize();
            GameObject p = Instantiate(projectile, transform.position + toVector3(projectionDirection), transform.rotation);
            ProjectileBehaviour pBehaviour = p.GetComponent<ProjectileBehaviour>();
            pBehaviour.SetDirection(toVector3(projectionDirection * 10));
            fireCooldDown = (1 / firerate);
        }
        else
        {
            fireCooldDown -= Time.deltaTime;
        }
    }

    void Patrol()
    {
        Debug.Log("Patrol");
        if(transform.position.x > initPos.x + patrolTourSize)
        {
            patrolDirection = Vector2.left;
        }
        if(transform.position.x < initPos.x - patrolTourSize)
        {
            patrolDirection = Vector2.right;
        }
        direction = patrolDirection;
        speed = walking_speed;
        //transform.position += toVector3(patrolDirection * Time.deltaTime * walking_speed);
    }

    bool DoSeeTarget()
    {
        if(transform.position.x > target.transform.position.x)
        {
            Debug.Log("left");
            return (getFacingDir().x < 0);
        }
        else
        {
            Debug.Log("right");
            return (getFacingDir().x > 0);
        }
        //return (Vector2.Dot(getFacingDir().normalized, selfToTarget.normalized) >= 1 - fov_vision);
    }

    void move()
    {
        lastPos = transform.position;
        transform.position += toVector3(direction.normalized * speed * Time.deltaTime);
    }

    public void StunEnemy()
    {
        state = 2;
        stunCooldown = stunDuration;
    }

    public void die()
    {
        StopCoroutine("UpdateState");
        // VFX + SFX
    }

    Vector2 toVector2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    Vector3 toVector3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    Vector2 getFacingDir()
    {
        return direction;
        //return (toVector2(transform.position) - lastPos);
    }

    private void OnDrawGizmosSelected()
    {
        GameManager.DrawCircle(transform.position, targeting_radius);
    }
}