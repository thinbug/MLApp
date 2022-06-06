using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTF8 说明
public class FoodSpawn : MonoBehaviour
{
    public GameObject prefab;

    public bool hasFood;
    GameObject food;

    

    public void PutInFood()
    {
        hasFood = true;
        food = GameObject.Instantiate(prefab);
        food.name = "food";
        food.SetActive(true);
        food.transform.parent = transform.parent;
        food.transform.localPosition = transform.localPosition;
    }

    public void GetFood()
    {
        hasFood = false;
        GameObject.Destroy(food);
    }
}
