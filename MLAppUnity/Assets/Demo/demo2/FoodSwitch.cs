using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTF8 说明
public class FoodSwitch : MonoBehaviour
{
    public FoodSpawn foodSpawn; //触发的食物衍生
    public Renderer renderSwitch;   //改变按钮颜色

    public Material onMat;  //按下的材质
    public Material offMat; //关闭的材质

    public bool isOn;   //按钮是否打开

    public void Clear()
    {
        isOn = false;
        renderSwitch.material = offMat;
        //让按钮还原高度
        renderSwitch.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        //获得食物，并吃掉
        foodSpawn.GetFood();
    }
    public void Switch(bool on)
    {
        if (!isOn && on)
        {
            isOn = true;
            renderSwitch.material = onMat;
         
            //让按钮按下去
            renderSwitch.transform.localPosition = new Vector3(0f, -0.65f, 0f);
            //投食
            foodSpawn.PutInFood();
        }
        else if (isOn && !on)
        {
            isOn = false;
            renderSwitch.material = offMat;
            //让按钮还原高度
            renderSwitch.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            //获得食物，并吃掉
            foodSpawn.GetFood();
        }
    }
}
