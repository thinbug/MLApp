using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FoodAgent : Agent
{
    [SerializeField] FoodSwitch foodSwitch;
    [SerializeField] FoodSpawn foodSpawn;
    [SerializeField] Renderer plane;
    [SerializeField] TextMesh text;
    float moveSpd = 3f;

    Rigidbody rig;

    float btime;
    float greentime = 1000f;   //��ǰ��������
    float usetime;//������ʱ
    const float greenrange = 5f;    //�����ֵ����Ҳ����ɫ

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    //��һ�ξ�����ʼ
    public override void OnEpisodeBegin()
    {
        btime = Time.time;
        transform.localPosition = new Vector3(Random.Range(-9f, 0f), 0f, Random.Range(-4f, 4f));
        foodSwitch.transform.localPosition = new Vector3(Random.Range(3f, 9f), 0f, Random.Range(-4f, 4f));

        foodSpawn.transform.localPosition = new Vector3(Random.Range(-9f, 0f), 5f, Random.Range(-4f, 4f));

        foodSwitch.Clear();
        Debug.Log("������ʼ");
    }

    //ͨ�������������괫�����
    public override void CollectObservations(VectorSensor sensor)
    {
        
        sensor.AddObservation(foodSwitch.isOn ? 1 : 0);
        Vector3 dir = (foodSwitch.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dir.x);
        sensor.AddObservation(dir.z);

        sensor.AddObservation(foodSpawn.hasFood ? 1 : 0);

        if (foodSpawn.hasFood)
        {
            Vector3 dirfood = (foodSpawn.GetFoodAt() - transform.localPosition).normalized;
            sensor.AddObservation(dirfood.x);
            sensor.AddObservation(dirfood.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        //Debug.Log("Observations : " );
    }

    //�ж�����
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.DiscreteActions[0];
        float moveZ = actions.DiscreteActions[1];
        bool switchon = actions.DiscreteActions[2] == 1;
        //Debug.Log("OnAction : " + moveX+","+ moveZ + ","+ switchon);
        Vector3 addforce = Vector3.zero;
        switch (moveX)
        {
            case 0:
                addforce.x = 0f; break;
            case 1:
                addforce.x = -1f; break;
            case 2:
                addforce.x = 1f; break;
        }
        switch (moveZ)
        {
            case 0:
                addforce.z = 0f; break;
            case 1:
                addforce.z = -1f; break;
            case 2:
                addforce.z = 1f; break;
        }

        rig.velocity = addforce * moveSpd + new Vector3(0f, rig.velocity.y, 0f);
        //transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpd;
        //Debug.Log(rig.velocity+","+ addforce);
        if (switchon)
        {
            Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].name.Equals("switch"))
                {
                    if (!foodSwitch.isOn)
                    {
                        foodSwitch.Switch(true);
                        AddReward(1f);

                        Debug.Log("AddReward - switch");
                    }
                }
                
            }
        }
        AddReward(-1f / MaxStep);
    }

    

    //����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1:
                actions[0] = 1; break;
            case 0:
                actions[0] = 0; break;
            case 1:
                actions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1:
                actions[1] = 1; break;
            case 0:
                actions[1] = 0; break;
            case 1:
                actions[1] = 2; break;
        }
        actions[2] = Input.GetKey(KeyCode.E) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Equals("food"))
        {
            AddReward(1f);
            Debug.Log("AddReward - food");
            Destroy(collision.collider.gameObject);

            foodSwitch.Switch(false);

            //������ʱ
            float nowtime = Time.time;
            usetime = nowtime - btime;
            if (usetime < greentime)
            {
                greentime = usetime;
                
            }
            if (usetime - greenrange < greentime)
            {
                isgreen = true;
                //��������ʱ���һ����ֵ������������ɫ�ġ�
                plane.material.color = Color.green;
            }

            EndEpisode();

        }
    }

    bool isgreen;
    private void Update()
    {
        usetime = Time.time - btime;
        text.text = ((int)usetime).ToString()+"/" +((int)greentime).ToString(); 
        if (isgreen)
        {
            //�������ɫ�ģ���ⳬʱ�͸�Ϊ��ɫ
            
            if (usetime - greenrange > greentime)
            {
                //��������ʱ���һ����ֵ������������ɫ�ġ�
                plane.material.color = Color.gray;
                isgreen = false;
            }
        }
    }


}
