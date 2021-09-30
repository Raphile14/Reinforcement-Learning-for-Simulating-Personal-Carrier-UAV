using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserScript : MonoBehaviour
{
    public Transform moveTarget;
    public Transform spawnTarget;
    new Rigidbody rigidbody;
    NavMeshAgent agent;
    int layerMask;
    Vector3 destination;

    // Animation
    public Animator animator;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        layerMask = LayerMask.GetMask("GroundMesh");
    }
    
    void FixedUpdate()
    {

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
    }

    // Helper Functions
    public void RelocateMoveTarget()
    {
        moveTarget.localPosition = new Vector3(Random.Range(40.0f, 49.0f) * (Random.Range(0, 2) * 2 - 1), 10f, Random.Range(40.0f, 49.0f) * (Random.Range(0, 2) * 2 - 1));

        RaycastHit hit;
        if (Physics.Raycast(moveTarget.localPosition, moveTarget.TransformDirection(Vector3.down), out hit, 20f, layerMask))
        {
            agent.SetDestination(hit.point);
            destination = hit.point;
            // Debug.Log("original hit: " + hit.point);
        }
        else
        {
            Debug.Log("Did Not Hit. Relocating... ");
            RelocateMoveTarget();
        }
        
    }

    public void ResetUser()
    {
        agent.enabled = false;
        RaycastHit hit;
        Physics.Raycast(spawnTarget.localPosition, spawnTarget.TransformDirection(Vector3.down), out hit, 20f, layerMask);
        this.transform.localPosition = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
        // Debug.Log("reset location: " + this.transform.position);
        agent.enabled = true;
    }

    //public void VerifyDestination()
    //{
    //    if (agent.enabled)
    //    {
    //        if (agent.destination.x != moveTarget.position.x && agent.destination.z != moveTarget.position.z)
    //        {
    //            Debug.Log("Destination incorrect, setting destination correctly");
    //            // moveTarget.position = new Vector3(agent.destination.x, 10f, agent.destination.z);
    //            agent.SetDestination(destination);
    //        }
    //    }
    //}
}
