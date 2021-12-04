using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float targeting_radius;
    [SerializeField] float damaging_radius;
    [SerializeField] float stunDuration;
    [SerializeField] float following_speed;

    private TargetBehaviour targetBehaviour;
    private int state = 0; // 0 - idle ; 1 - targeting ; 2 - stunned
    private float stunCooldown = 0;
    private Material material;

    private void Awake()
    {
        targetBehaviour = target.GetComponent<TargetBehaviour>();
        material = gameObject.GetComponent<Material>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                CheckDistanceWithTarget();
                break;
            case 1:
                FollowAndDamageTarget();
                break;
            case 2:
                stunCooldown -= Time.deltaTime;
                break;
        }
    }

    void CheckDistanceWithTarget()
    {
        if ((target.transform.position - transform.position).magnitude <= targeting_radius)
        {
            state = 1;
        }
    }

    void FollowAndDamageTarget()
    {
        Vector3 selfToTarget = target.transform.position - transform.position;
        if(selfToTarget.magnitude <= damaging_radius)
        {
            targetBehaviour.GetDamage(1f);
        }
        else
        {
            transform.position += selfToTarget.normalized * following_speed * Time.deltaTime;
        }

    }

    public void StunEnemy()
    {
        state = 2;
        stunCooldown = stunDuration;
    }
}
