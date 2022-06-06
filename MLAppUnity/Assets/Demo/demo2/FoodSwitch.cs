using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTF8 说明
public class FoodSwitch : MonoBehaviour
{
    public FoodSpawn foodSpawn;
    public Renderer renderSwitch;

    public Material onMat;
    public Material offMat;

    public bool isOn;

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
