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
    public float rotationSpeed = 0.5f;
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

        // Reset Agent rigidbody
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;        

        // Reset Terrain, Trees, and NavMesh
        meshGenerator.CreateShape();
        meshGenerator.UpdateMesh();
        meshGenerator.GenerateTrees();
        meshGenerator.UpdateNavMesh();

        // Reset User
        userScript.ResetUser();
        userScript.RelocateMoveTarget();               

        // Reset Agent Location
        this.transform.localPosition = new Vector3(0, target.localPosition.y + 3f, 0);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

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

        Vector3 controlSignal = Vector3.zero;

        // ================ FORWARD MOVEMENT ================
        // Branch0 (2) = dont move, or move forward
        // controlSignal.x = actions.DiscreteActions[0];
        if (actions.DiscreteActions[0] == 1)
        {
            controlSignal += this.transform.forward;
        }

        // ================ UP/DOWN MOVEMENT ================
        // Branch1 (2) = 0 = don't fly, 1 = fly
        // controlSignal.y = actions.DiscreteActions[2];
        if (actions.DiscreteActions[1] == 1)
        {
            controlSignal += this.transform.up;
        }

        // ================ ROLL ROTATION ================
        // Branch2 = 0 = Dont Roll, 1 = left, 2 = right
        // Turn Left (-1) or Look straight        
        float roll = -actions.DiscreteActions[2] * rotationSpeed;
        // Turn Right
        if (actions.DiscreteActions[2] == 2)
        {
            roll = 1 * rotationSpeed;
        }

        // ================ YAW ROTATION ================
        // Branch3 = 0 = Dont Rotate on Yaw, 1 = left, 2 = right
        // Turn Left (-1) or Look straight        
        float yaw = -actions.DiscreteActions[3] * rotationSpeed;        
        // Turn Right
        if (actions.DiscreteActions[3] == 2)
        {
            yaw = 1 * rotationSpeed;            
        }

        // ================ PITCH ROTATION ================
        // Branch4 = 0 = Dont Rotate on Pitch, 1 = left, 2 = right
        // Turn Left (-1) or Look straight        
        float pitch = -actions.DiscreteActions[4] * rotationSpeed;
        // Turn Right
        if (actions.DiscreteActions[4] == 2)
        {
            pitch = 1 * rotationSpeed;
        }

        // Applying rotation
        this.transform.Rotate(roll, yaw, pitch);        

        // Apply Force
        rBody.AddRelativeForce(controlSignal * speed);

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
        
        // If UAV Clipped very far down or very far up
        if (this.transform.localPosition.y < -10f || this.transform.localPosition.y > 30f)
        {
            AddReward(-10);
            EndEpisode();
        }

        // Move target and human target
        // End episode if human is near target
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
