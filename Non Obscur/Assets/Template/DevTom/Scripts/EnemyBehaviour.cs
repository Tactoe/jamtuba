using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float radius;
    [SerializeField] float stunDuration;

    private int state = 0; // 0 - idle ; 1 - targeting ; 2 - stunned
    private float stunCooldown = 0;
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<Material>();
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
        if ((target.transform.position - transform.position).magnitude <= radius)
        {
            state = 1;
        }
    }

    void FollowAndDamageTarget()
    {

    }

    public void StunEnemy()
    {
        state = 2;
        stunCooldown = stunDuration;
    }
}
