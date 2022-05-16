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
    //ͨ�������������괫�����
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTfm.transform.localPosition);
    }

    //�ж�����
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpd;
    }

    //��һ�ξ�����ʼ
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        //Debug.Log("������ʼ");
    }

    //����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpd;
        continuousActions[1] = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpd;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter��"+other.name);
        if (other.name.Equals("target"))
        { 
            AddReward(+1f); //����
            EndEpisode();   //��������
            plane.material.color = Color.green;
            //Debug.Log("����");
        }
        else if (other.name.Equals("wall"))
        {
            AddReward(-1f); //�ͷ�
            EndEpisode();   //����
            plane.material.color = Color.grey;
            //Debug.Log("�ͷ�");
        }
    }
   
}
