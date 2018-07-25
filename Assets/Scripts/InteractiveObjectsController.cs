using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectsController : MonoBehaviour {

    public bool isFood;

	// Update is called once per frame
	void FixedUpdate ()
    {
        //checking for collision
        if (GameMaster.snake.transform.position.x > (transform.position.x-transform.localScale.x) &&
            GameMaster.snake.transform.position.x < (transform.position.x + transform.localScale.x) &&
            GameMaster.snake.transform.position.y > (transform.position.y - transform.localScale.x) &&
            GameMaster.snake.transform.position.y < (transform.position.y + transform.localScale.x))
        {
            if (isFood)
            {
                GameMaster.foodObj = gameObject;
            }
            else
            {
                GameMaster.hasCollided = true;
            }
        }

    }
}