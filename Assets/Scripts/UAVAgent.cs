using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class UAVAgent : Agent
{
    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(vectorAction[0]);
        base.OnActionReceived(vectorAction);
    }
}
