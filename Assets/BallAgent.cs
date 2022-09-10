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

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this.rigidBody.angularVelocity = Vector3.zero;
            this.rigidBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        this.Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
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
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
}
