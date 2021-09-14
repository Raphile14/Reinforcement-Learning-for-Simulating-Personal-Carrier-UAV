using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour
{
    public Transform moveTarget;
    public float speed = 2;
    new Rigidbody rigidbody;
    public float rotationSpeed = 1f;

    // Animation
    public Animator animator;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        Vector3 moveTargetDirection = moveTarget.position;
        moveTargetDirection.y = this.transform.position.y;
        Vector3 direction = (moveTargetDirection - this.transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation(direction);
        rigidbody.MovePosition(this.transform.position + direction * speed * Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);

        // Animation
        Vector3 velocity = rigidbody.velocity;
        float speedVelocity = velocity.magnitude;

        if (speedVelocity > 0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

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
