using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EaterAgent : Agent
{

    [SerializeField] Renderer plane;
    [SerializeField] TextMesh textmesh;
    [SerializeField] TextMesh text;
    [SerializeField] float layDis = 20f;
    [SerializeField] float bornRange = 5f;
    float moveSpd = 3f;
    float rotSpd = 6f;
    public int lv = 0;
    int eatedfood = 0;
    Rigidbody rig;

    bool isgreen;
    float btime;
    float greentime = 1000f;   //当前最快结束的
    float usetime;//本次用时
    const float greenrange = 5f;    //比最快值长，也是绿色

    int layerWall, layerFood, layerFish;
    Camera myCamera;

    GameObject targetFish;
    GameObject targetFood;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();

        layerWall = LayerMask.NameToLayer("wall");
        layerFood = LayerMask.NameToLayer("food");
        layerFish = LayerMask.NameToLayer("fish");

        myCamera = Camera.main;
    }

    //当一段经历开始
    public override void OnEpisodeBegin()
    {
        btime = Time.time;
        targetFish = null;
        targetFood = null;
        if (lv == 0)
        {
            lv = Random.Range(1, 5);
            textmesh.text = lv.ToString();
            transform.localPosition = new Vector3(Random.Range(-bornRange, bornRange), 0f, Random.Range(-bornRange, bornRange));
        }
        //Debug.Log("经历开始");
    }

    //通过传感器把坐标传入监视
    public override void CollectObservations(VectorSensor sensor)
    {
        //首先玩家发射检测扇形，检测前方物体
        Vector3 dir ;
        int layer = 1 << layerFood | 1 << layerFish | 1 << layerWall;
        bool eyeFish = false;
        bool eyeFood = false;
        Vector3 dirFish = Vector3.zero;
        Vector3 dirFood = Vector3.zero;
        int fishLV=0;
        float fishdis = 0f;
        float fooddis = 0f;
        int maxline = 25;
        //targetFish = targetFood = null;
        float maxdisfood = float.MaxValue;
        float maxdisfish = float.MaxValue;
        float dis1 = float.MaxValue;
        float dis2 = float.MaxValue;
        GameObject tfood = null, tfish = null;
        for (int i = 0; i < maxline; i++)
        {
            Quaternion q = Quaternion.AngleAxis(5f* (i - maxline/2) , Vector3.up);
            dir = (q * transform.forward).normalized;

            //Debug.DrawRay(transform.localPosition, dir,Color.green,0.03f);
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            bool hited = Physics.SphereCast(ray, 0.5f, out hit, layDis, layer);
            Debug.DrawRay(ray.origin, ray.direction * layDis, Color.white);
            
            if (hited)
            {
                if (hit.collider.gameObject.layer == layerFood)
                {
                    dis1 = Vector3.Distance(hit.collider.transform.position, transform.position);
                    if (dis1 < maxdisfood)
                    {
                        maxdisfood = dis1;
                        //Debug.Log("eyeFood");

                        tfood = hit.collider.gameObject;

                        //dirFood = (hit.collider.transform.position - transform.position).normalized;

                    }

                }
                else if (hit.collider.gameObject.layer == layerFish)
                {
                    dis2 = Vector3.Distance(hit.collider.transform.position, transform.position);
                    if (dis2 < maxdisfish)
                    {
                        maxdisfish = dis2;
                        //dirFish = (hit.collider.transform.position - transform.position).normalized;

                        //Debug.DrawRay(ray.origin, dirFood * dis, Color.blue);
                        tfish = hit.collider.gameObject;
                    }
                
                }
                //else if (hit.collider.gameObject.layer == layerDefault)
                //{
                //    //eyeFish = true;
                //    //dirFish = (hit.collider.transform.position - transform.position).normalized;
                //    //Debug.Log("eyeWall");
                //}
            }
        }

        //如果找到一个目标
        if (tfood != null)
        {
            //如果当前目标不是空
            if (targetFood != null && targetFood != tfood)
            {
                //计算那个距离近，更新目标
                float d = Vector3.Distance(targetFood.transform.position, transform.position);
                if (dis1 < d)
                {
                    targetFood = tfood;
                }
            }
            else
            {
                targetFood = tfood;
            }
        }
        if (targetFood != null)
        {
            fooddis = Vector3.Distance(targetFood.transform.position, transform.position);
            if (fooddis > layDis)
            {
                //超出距离了
                fooddis = 0f;
                targetFood = null;
            }
            else
            {
                eyeFood = true;
                dirFood = (targetFood.transform.position - transform.position).normalized;
                Debug.DrawRay(transform.position, dirFood * fooddis, Color.blue, Time.deltaTime);
            }
        }
        sensor.AddObservation(eyeFood ? 1 : 0);
        sensor.AddObservation(dirFood.x);
        sensor.AddObservation(dirFood.z);
        sensor.AddObservation(fooddis);

        if (tfish != null)
        {
            //如果当前目标不是空
            if (targetFish != null && targetFish != tfish)
            {
                //计算那个距离近，更新目标
                float d = Vector3.Distance(targetFish.transform.position, transform.position);
                if (dis2 < d)
                {
                    targetFish = tfish;
                }
            }
            else
            {
                targetFish = tfish;
            }
        }
        if (targetFish != null)
        {
            fishdis = Vector3.Distance(targetFish.transform.position, transform.position);
            if (fishdis > layDis)
            {
                fishdis = 0f;
                targetFish = null;
            }
            else
            {
                eyeFish = true;
                dirFish = (targetFish.transform.position - transform.position).normalized;
                fishLV = targetFish.GetComponent<EaterAgent>().lv;
                Debug.DrawRay(transform.position, dirFish * fishdis, Color.red , Time.deltaTime);
            }
        }
        sensor.AddObservation(eyeFish ? 1 : 0);
        sensor.AddObservation(dirFish.x);
        sensor.AddObservation(dirFish.z);
        sensor.AddObservation(fishdis);
        sensor.AddObservation(lv - fishLV);

    }

    //行动接收
    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveForward = actions.DiscreteActions[0];
        int rotForward = actions.DiscreteActions[1] ;
        
        float addforce = 0f;
        switch (moveForward)
        {
            case 0:
                addforce = 0f; break;
            case 1:
                addforce = 1f; break;
            case 2:
                addforce = -1f; break;

        }
        rig.velocity = transform.forward * addforce * moveSpd + new Vector3(0f, rig.velocity.y, 0f);

        Vector3 rotVel = Vector3.zero;
        switch (rotForward)
        {
            case 0:
                rotVel.y = 0f; break;
            case 1:
                rotVel.y = 1f; break;
            case 2:
                rotVel.y = -1f; break;

        }
        rig.angularVelocity = rotVel * rotSpd;
        

        

        //transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpd;
        //Debug.Log(rig.velocity+","+ addforce);
        //if (switchon)
        //{
        //    Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
        //    for (int i = 0; i < colls.Length; i++)
        //    {
        //        if (colls[i].name.Equals("switch"))
        //        {
        //            if (!foodSwitch.isOn)
        //            {
        //                foodSwitch.Switch(true);
        //                AddReward(1f);

        //                Debug.Log("AddReward - switch");
        //            }
        //        }
                
        //    }
        //}
        AddReward(-1f / MaxStep);
    }

    

    //启发
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case 0:
                actions[0] = 0; break;
            case 1:
                actions[0] = 1; break;
            case -1:
                actions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1:
                actions[1] = 2; break;
            case 0:
                actions[1] = 0; break;
            case 1:
                actions[1] = 1; break;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == layerFood)
        {
            //Debug.Log(collision.collider.gameObject.name +" - "+ targetFood.name);
            if (targetFood == collision.collider.gameObject)
            {
                //Debug.Log("AddReward - food");

                AddReward(1f);
                eatedfood += 1;
                if (eatedfood >= 1)
                {
                    lvup(1);
                    eatedfood = 0;
                }
                textmesh.text = lv.ToString();
                FoodCreator.inst.DestoryFood(collision.collider.gameObject);
                EndEpisode();
            }
        }
        if (collision.collider.gameObject.layer == layerFish)
        {
            if (targetFish == collision.collider.gameObject)
            {
                //Debug.Log("AddReward - food");
                EaterAgent target = collision.gameObject.GetComponent<EaterAgent>();
                if (lv > target.lv)
                {
                    lvup(1);
                    AddReward(target.lv/10f);

                    target.AddReward(-0.5f);
                    target.dead();
                    target.EndEpisode();
                }
                else
                {
                    dead();
                    AddReward(-0.1f);
                }
                EndEpisode();
            }
        }
        //if (collision.collider.gameObject.layer == layerWall)
        //{
        //    AddReward(-0.01f);
        //}
    }

    public void dead()
    {
        lv = 0;
        eatedfood = 0;
        textmesh.text = lv.ToString();
    }

    void lvup(int num)
    {
        lv+=num;
        textmesh.text = lv.ToString();
    }

    private void Update()
    {
        textmesh.transform.forward = myCamera.transform.forward;

        
    }


}
