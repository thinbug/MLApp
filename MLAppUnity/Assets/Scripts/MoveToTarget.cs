using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTarget : Agent
{
    [SerializeField] Transform targetTfm;
    [SerializeField] Renderer plane;
    float moveSpd = 30f;
    //通过传感器把坐标传入监视
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTfm.transform.localPosition);
    }

    //行动接收
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpd;
    }

    //当一段经历开始
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        //Debug.Log("经历开始");
    }

    //启发
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpd;
        continuousActions[1] = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpd;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter："+other.name);
        if (other.name.Equals("target"))
        { 
            AddReward(+1f); //奖励
            EndEpisode();   //结束经历
            plane.material.color = Color.green;
            //Debug.Log("奖励");
        }
        else if (other.name.Equals("wall"))
        {
            AddReward(-1f); //惩罚
            EndEpisode();   //结束
            plane.material.color = Color.grey;
            //Debug.Log("惩罚");
        }
    }
   
}
