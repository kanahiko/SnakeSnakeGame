﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour {

    public float defaultSpeed = 20f;
    float snakeSpeed;
    public float distance = 1.2f;
    public int startParts = 5;
    float speedInc = 0.01f;

    public Transform snakeHeadPrefab;
    public Transform snakeTailPrefab;

    Transform snakeHead;
    List<Transform> snakeTail;
    
    public void CreateSnake() {
        //creating snake head, 5 tail parts and setting speed
        snakeHead = Instantiate(snakeHeadPrefab, transform, false);
        snakeTail = new List<Transform>();

        snakeTail.Add(Instantiate(snakeTailPrefab, transform, false));
        snakeTail[snakeTail.Count - 1].localPosition = snakeHead.localPosition-new Vector3(0,distance,0);

        for (int i = 1; i < startParts; i++)
        {
            AddTailPart();
        }

        snakeSpeed = defaultSpeed;
    }
    
    public void MoveSnake(Vector2 direction, bool isAccelerated, float smoothTime)
    {
        //rotating snake towards the direction set by joytick
        snakeHead.rotation = Quaternion.AngleAxis(Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg, Vector3.forward);

        //speed  and smooth time for snake based on acceration
        float speed;
        float smoothingTime;
            if (isAccelerated)
        {
            speed = snakeSpeed * 2;
            smoothingTime = smoothTime / 2.0f;
        }
        else
        {
            speed = snakeSpeed;
            smoothingTime = smoothTime;
        }

            //changing snake head position
        snakeHead.position += snakeHead.up * Time.deltaTime * speed;

        //if snake head outside field boundaries game over
        if (snakeHead.position.x< transform.GetComponent<GameMaster>().fieldCoords[0].x +0.50f|| snakeHead.position.x> transform.GetComponent<GameMaster>().fieldCoords[1].x -0.50f||
            snakeHead.position.y < transform.GetComponent<GameMaster>().fieldCoords[0].y +0.50f || snakeHead.position.y > transform.GetComponent<GameMaster>().fieldCoords[1].y-0.50f)
        {
            GameMaster.hasCollided = true;
            return;
        }

        //moving each tail part
        for (int i = snakeTail.Count - 1; i > 0; i--)
        {
            snakeTail[i].GetComponent<SnakeTail>().MovePart(snakeTail[i - 1].position, smoothingTime);
            snakeTail[i].LookAt(snakeHead);
            //if tail part is further than 3 part from head then check for collision
            //checks if snake head is in tail part centered circle
            if (i > 2)
            {
                if (Mathf.Sqrt(Mathf.Pow((snakeHead.position.x - snakeTail[i].position.x), 2) + Mathf.Pow((snakeHead.position.y - snakeTail[i].position.y), 2)) <( transform.localScale.x/2))
                {
                    GameMaster.hasCollided = true;
                    return;
                }
            }
        }

        //moving first tail part
        //not in loop because it's conected to snake head
        snakeTail[0].GetComponent<SnakeTail>().MovePart(snakeHead.position, smoothingTime);
        snakeTail[0].LookAt(snakeHead);

    }
    
    public void AddTailPart()
    {
        //adding tail part in position of pevious tail part
        snakeTail.Add(Instantiate(snakeTailPrefab, transform, false));
        snakeTail[snakeTail.Count - 1].localPosition = snakeTail[snakeTail.Count - 2].localPosition;
    }

    public void IncreaseSpeed()
    {
        //increasing snake speed
        snakeSpeed = snakeSpeed +(snakeSpeed* speedInc);
    }

    public void DestroySnake()
    {
        if (!snakeHead) return;
        Destroy(snakeHead.gameObject);
        for (int i = 0; i < snakeTail.Count; i++)
        {
            Destroy(snakeTail[i].gameObject);
        }
    }

}