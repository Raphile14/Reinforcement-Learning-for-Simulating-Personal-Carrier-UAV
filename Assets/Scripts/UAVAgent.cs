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
    public float speed = 10;
    public UserScript userScript;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        // Reset Agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 5, 0);

        // Reset Environment
        userScript.RelocateMoveTarget();
        userScript.ResetUser();
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

        // Fell to the Ground or too low
        // Out of bounds Y Axis
        if (this.transform.localPosition.y > 7f || this.transform.localPosition.y < 3f)
        {
            EndEpisode();
        }

        // Out of bounds X Axis
        if (this.transform.localPosition.x > 49f || this.transform.localPosition.x < -49f)
        {
            EndEpisode();
        }

        // Out of bounds Z Axis
        if (this.transform.localPosition.z > 49f || this.transform.localPosition.z < -49f)
        {
            EndEpisode();
        }
    } 
    
}
