using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTF8 说明
public class FoodSpawn : MonoBehaviour
{
    public GameObject prefab;

    public bool hasFood;
    GameObject food;

  

    public Vector3 GetFoodAt()
    {
        if (food == null)
            return Vector3.zero;
        return food.transform.localPosition;
    }
    //投食
    public void PutInFood()
    {
        hasFood = true;
        food = GameObject.Instantiate(prefab);
        food.name = "food";
        food.SetActive(true);
        food.transform.parent = transform.parent;
        food.transform.localPosition = transform.localPosition;
    }

    //获取食物（被吃掉）
    public void GetFood()
    {
        hasFood = false;
        if (food != null)
            GameObject.Destroy(food);
        food = null;
    }
}
