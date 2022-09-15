using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{

    public Transform Target;
    Rigidbody rigidBody;
    private float lastDistance;

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this.rigidBody.angularVelocity = Vector3.zero;
            this.rigidBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        this.Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        this.lastDistance = -1;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rigidBody.velocity.x);
        sensor.AddObservation(this.rigidBody.velocity.z);
        sensor.AddObservation(this.Target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionX = actions.ContinuousActions[0];
        var actionZ = actions.ContinuousActions[1];
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionX;
        controlSignal.z = actionZ;
        rigidBody.AddForce(controlSignal * 10);

        var distanceToTarget = Vector3.Distance(this.transform.localPosition, 
            this.Target.localPosition);

        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        } else 
        {
            if (this.lastDistance == -1) {
                this.lastDistance = distanceToTarget;
            } else {
                if (this.lastDistance <= distanceToTarget) {
                    AddReward(-0.01f);
                    EndEpisode();
                } else {
                    AddReward(-0.01f);
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
}
