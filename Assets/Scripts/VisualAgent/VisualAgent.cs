using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class VisualAgent : Agent
{
    float[] acceptableMovements = new float[] { -2.5f, 0f, 2.5f };
    int lane = 1;
    int layerMask;
    public GameObject obstacleHolder;

    void Start()
    {
        layerMask = LayerMask.GetMask("VAObstacle");
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        this.transform.localPosition = new Vector3(0f, 0.5f, -20f);
        lane = 1;

        foreach (Transform child in obstacleHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        // Observe Lane
        sensor.AddObservation(lane);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Default (no movement)
        discreteActionsOut[0] = 0;

        // ================ LEFT/RIGHT MOVEMENT ================
        // Left
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 1;
        }
        // Right
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        // ================ LEFT/RIGHT MOVEMENT ================
        // Left
        if (actions.DiscreteActions[0] == 1)
        {
            if (lane > 0)
            {
                lane -= 1;
            }
        }
        else if (actions.DiscreteActions[0] == 2)
        {
            if (lane < 2)
            {
                lane += 1;
            }
        }
        this.transform.localPosition = new Vector3(acceptableMovements[lane], 0.5f, -20f);
        AddReward(0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            AddReward(-10);
            EndEpisode();
        }
    }
}
