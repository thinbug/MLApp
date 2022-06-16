using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTF8 说明
public class FoodCreator : MonoBehaviour
{
    public static FoodCreator inst;
    public GameObject prefab;

    [SerializeField] int maxnum = 10;
    [SerializeField] int delaytime = 1;

    public int foodrandrange = 20;  //食物出现位置范围

    float lastcreatetime;
    int no = 0;
    private void Awake()
    {
        inst = this;
    }
    void Update()
    {
        if (Time.time < lastcreatetime)
            return;
        if (transform.childCount > maxnum)
            return;
        lastcreatetime = Time.time + delaytime;

        GameObject food = Instantiate(prefab);
        food.name = "food"+ no;
        no++;
        food.SetActive(true);
        food.transform.parent = transform;
        food.transform.localPosition = new Vector3(Random.Range(-foodrandrange, foodrandrange), 5f, Random.Range(-foodrandrange, foodrandrange));
        
    }

    public void DestoryFood(GameObject food)
    {
        Destroy(food);
    }
}
