using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour
{
    public Transform moveTarget;
    public float speed = 10;
    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveTargetDirection = moveTarget.position;
        moveTargetDirection.y = this.transform.position.y;
        Vector3 direction = (moveTargetDirection - this.transform.position).normalized;        
        rigidbody.MovePosition(this.transform.position + direction * speed * Time.deltaTime);

        float distance = this.transform.position.x - moveTarget.position.x;
        if (distance < 0.5f && distance > -0.5f)
        {
            RelocateMoveTarget();
        }
    }

    // Helper Functions
    public void RelocateMoveTarget()
    {
        moveTarget.localPosition = new Vector3(Random.Range(-49.0f, 49.0f), 10f, Random.Range(-49.0f, 49.0f));
    }

    public void ResetUser()
    {
        this.transform.localPosition = new Vector3(0, 5, 0);
    }
}
