using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPool : MonoBehaviour {

    public GameObject food;
    Queue<GameObject> foodPool;
    public int poolSize = 5;


    // Use this for initialization
    public void Create () {
        foodPool = new Queue<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            foodPool.Enqueue(Instantiate(food,transform));
        }
	}
	
    public GameObject GetFood()
    {
        if (foodPool.Count > 0)
        {
            return foodPool.Dequeue();
        }
        return null;
    }

    public void ReturnFood(GameObject food)
    {
        food.SetActive(false);
        foodPool.Enqueue(food);
    }    

    public void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            foodPool.Enqueue(transform.GetChild(0).gameObject);
        }
    }
}
