using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private Rigidbody rb;
    private bool hasBeenLaunched = false;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hasBeenLaunched && direction != null && direction.magnitude != 0)
        {
            hasBeenLaunched = true;
            rb.AddForce(direction, ForceMode.Impulse);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }
}
