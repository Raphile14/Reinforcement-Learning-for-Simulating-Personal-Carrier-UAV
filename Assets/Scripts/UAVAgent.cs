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
    public float rotationSpeed = 3f;
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
        this.transform.localPosition = new Vector3(0, target.localPosition.y + 3f, -5);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Set Status to true
        ongoing = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        // Observe User Target, Distance, and Agent Position
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        // sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition, target.localPosition));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // base.Heuristic(actionsOut);
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Default
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;
        discreteActionsOut[2] = 0;
        // discreteActionsOut[3] = 0;
        // discreteActionsOut[4] = 0;

        // ================ FORWARD MOVEMENT ================
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }

        // ================ UP/DOWN MOVEMENT ================
        // Up
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[1] = 1;
        }
        // Down
        if (Input.GetKey(KeyCode.C))
        {
            discreteActionsOut[1] = 2;
        }

        // ================ YAW ROTATION ================
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        Vector3 controlSignal = Vector3.zero;

        // ================ FORWARD MOVEMENT ================
        // Branch0 (2) = 0 = dont move, 1 = move forward, 2 = move backward
        // controlSignal.x = actions.DiscreteActions[0];
        if (actions.DiscreteActions[0] == 1)
        {
            controlSignal += transform.forward;
        }
        if (actions.DiscreteActions[0] == 2)
        {
            controlSignal += transform.forward * -1;
        }

        // ================ UP/DOWN MOVEMENT ================
        // Branch1 (2) = 0 = don't fly, 1 = fly, 2 = fly down
        // controlSignal.y = actions.DiscreteActions[2];
        if (actions.DiscreteActions[1] == 1)
        {
            controlSignal += transform.up;
        }
        if (actions.DiscreteActions[1] == 2)
        {
            controlSignal += transform.up * -1;
        }

        //// ================ ROLL ROTATION ================
        //// Branch2 = 0 = Dont Roll, 1 = left, 2 = right
        //// Turn Left (-1) or Look straight        
        //float roll = -actions.DiscreteActions[2] * rotationSpeed;
        //// Turn Right
        //if (actions.DiscreteActions[2] == 2)
        //{
        //    roll = 1 * rotationSpeed;
        //}

        // ================ YAW ROTATION ================
        // Branch3 = 0 = Dont Rotate on Yaw, 1 = left, 2 = right
        // Turn Left (-1) or Look straight
        // 3
        float yaw = -actions.DiscreteActions[2] * rotationSpeed;
        // Turn Right
        if (actions.DiscreteActions[2] == 2)
        {
            yaw = 1 * rotationSpeed;            
        }

        //// ================ PITCH ROTATION ================
        //// Branch4 = 0 = Dont Rotate on Pitch, 1 = left, 2 = right
        //// Turn Left (-1) or Look straight        
        //float pitch = -actions.DiscreteActions[4] * rotationSpeed;
        //// Turn Right
        //if (actions.DiscreteActions[4] == 2)
        //{
        //    pitch = 1 * rotationSpeed;
        //}

        // Applying rotation
        // this.transform.Rotate(roll, yaw, pitch);
        this.transform.Rotate(0, yaw, 0);

        // Apply Force
        rBody.MovePosition(transform.position + (controlSignal * speed * Time.deltaTime));

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
            AddReward(-0.05f);
        }  
        
        // If UAV Clipped very far down or very far up
        if (this.transform.localPosition.y < -10f || this.transform.localPosition.y > 30f)
        {
            AddReward(-10);
            EndEpisode();
        }

        // Move target and human target
        // End episode if human is near target or reached corner
        float distance = target.position.x - moveTarget.position.x;
        if ((distance < 1f && distance > -1f) || target.localPosition.x > 45 || target.localPosition.x < -45 || target.localPosition.z > 45 || target.localPosition.x < 45)
        {
            EndEpisode();
        }

        // Check if human destination is correct
        //userScript.VerifyDestination();
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
