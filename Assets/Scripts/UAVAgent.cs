using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class UAVAgent : Agent
{
    Rigidbody rBody;
    public Transform target;
    public Transform moveTarget;
    public float speed = 5;
    public UserScript userScript;
    public MeshGenerator meshGenerator;
    public GameObject groundMarker;
    public bool ongoing = false;    

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
        groundMarker.SetActive(false);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        // Reset Agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, target.localPosition.y + 3f, 0);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);        

        // Reset Terrain and NavMesh
        meshGenerator.CreateShape();
        meshGenerator.UpdateMesh();
        meshGenerator.UpdateNavMesh();

        // Reset User
        userScript.ResetUser();
        userScript.RelocateMoveTarget();                

        // Set Status to true
        ongoing = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        // Observe User Target and Agent Position as well as Agent Velocity
        // TODO: Add Visual Sensors
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // base.Heuristic(actionsOut);
        // Debug.Log("test");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        // Branches 3
        Vector3 controlSignal = Vector3.zero;

        // Branch0 = dont move, or move forward
        controlSignal.x = actions.DiscreteActions[0];

        // Branch1 = 0 = Look straight, 1 = left, 2 = right
        // Turn Right
        if (actions.DiscreteActions[1] == 2)
        {
            controlSignal.z = 1;
        }
        // Turn Left (-1) or Look straight
        else
        {
            controlSignal.z = -actions.DiscreteActions[1];
        }

        // Branch3 = 0 = don't fly, 1 = fly
        controlSignal.y = actions.DiscreteActions[2];

        // Apply Force
        rBody.AddForce(controlSignal * speed);

        // Follow user
        float distanceToUser = Vector3.Distance(this.transform.localPosition, target.localPosition);
        // Near Target
        if (distanceToUser < 3f)
        {            
            AddReward(0.1f);            
        }
        // Penalty if not in radius
        else
        {
            AddReward(-0.01f);
        }  
        
        // If UAV Clipped very far down
        if (this.transform.localPosition.y < -10f)
        {
            AddReward(-10);
            EndEpisode();
        }

        // Move target and human target
        float distance = target.position.x - moveTarget.position.x;
        if (distance < 1f && distance > -1f)
        {
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Collided with something
        if (ongoing)
        {
            AddReward(-10);
            ongoing = false;
            EndEpisode();            
        }
    }
}
