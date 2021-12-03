using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public Transform Target;
    // Start is called before the first frame update

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
    }
}