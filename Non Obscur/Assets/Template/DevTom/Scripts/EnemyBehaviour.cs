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

    [SerializeField] float damaging_radius;

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

    private void Awake()
    {
        targetBehaviour = target.GetComponent<TargetBehaviour>();
        material = gameObject.GetComponent<Material>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
                CheckDistanceWithTarget();
                break;
            case 1:
                if (DoSeeTarget())
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
        lastPos = toVector2(transform.position);
        //yield return new WaitForSeconds(0.1f);
    }

    void CheckDistanceWithTarget()
    {
        float distance = (target.transform.position - transform.position).magnitude;
        Debug.Log(distance);
        if (distance <= targeting_radius)
        {
            if (DoSeeTarget())
            {
                state = 1;
            }
        }
    }

    void FollowAndDamageTarget()
    {
        Debug.Log("following");
        selfToTarget = target.transform.position - transform.position;
        if(selfToTarget.magnitude <= damaging_radius)
        {
            targetBehaviour.GetDamage(1f);
        }
        else
        {
            Vector2 movement = selfToTarget;
            movement.y = 0;
            movement = movement.normalized * following_speed * Time.deltaTime;
            lastPos = transform.position;
            transform.position += toVector3(movement);
        }
    }

    void Shoot()
    {
        GameObject p = Instantiate(projectile, transform, true);
        ProjectileBehaviour pBehaviour = p.GetComponent<ProjectileBehaviour>();
        pBehaviour.SetDirection(new Vector2(1, 1));
    }

    void Patrol()
    {
        if((lastPatrolPos - toVector2(transform.position)).magnitude > patrollingErrorThreshold)
        {
            // returning back to patrolling
            Debug.Log("back");
            lastPos = transform.position;
            transform.position += toVector3((lastPatrolPos - toVector2(transform.position)).normalized * Time.deltaTime * walking_speed);
        }
        else
        {
            Debug.Log("Patrol");
            if(Mathf.Abs(lastPatrolPos.x - initPos.x) > patrolTourSize)
            {
                patrolDirection = -patrolDirection;
            }
            // continuing patrol
            lastPos = transform.position;
            transform.position += toVector3(patrolDirection * Time.deltaTime * walking_speed);
            lastPatrolPos = toVector2(transform.position);
        }
    }

    public void StunEnemy()
    {
        state = 2;
        stunCooldown = stunDuration;
    }

    bool DoSeeTarget()
    {
        return (Vector2.Dot(getFacingDir().normalized, selfToTarget.normalized) >= 1 - fov_vision);
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
        return (toVector2(transform.position) - lastPos);
    }
}
