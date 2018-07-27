using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    
    public GameObject blockPrefab;

    public static Vector3[] fieldCoords= new Vector3[2];

    //current score of a game
    public static int foodEaten = 0;

    public FoodPool pool;

    public static SnakeController snakeController;
    public static GameObject snakeHead;
    MobileControls mobileControls;

    public static bool hasCollided = false;

    public Material bgMat;

    GameObject background;

    public static int recordScore;

    //empty gameObject for all blocks
    GameObject blockContainer;

    //new record text will be show only once per game
    bool isCongradulated = false;

	// Use this for initialization
	void Start ()
    {
        Application.targetFrameRate = 60;
        recordScore = PlayerPrefs.GetInt("Record score");
        StartGame();
    }
	

    public void StartGame()
    {
        Time.timeScale = 1;
        hasCollided = false;
        isCongradulated = false;
        foodEaten = 0;
        snakeController = transform.GetComponent<SnakeController>();

        snakeController.DestroySnake();
        snakeController.CreateSnake();

        snakeHead = transform.GetChild(0).gameObject;
        mobileControls = GetComponent<MobileControls>();

         //pool
        CreatePlayField();
        if (pool)
        {
            pool.Reset();
        }
        pool.Create();

        Destroy(blockContainer);

        blockContainer = new GameObject("BlockContainer");
        
        GenerateFood();
    }

    void FixedUpdate()
    {
        //comaning snake to move or game over
        if (!hasCollided)
        {
            snakeController.MoveSnake(mobileControls.direction, mobileControls.isAccelerated, mobileControls.smoothTime);

            //if snake head outside field boundaries game over
            if (snakeHead.transform.position.x < fieldCoords[0].x + 0.50f || snakeHead.transform.position.x > fieldCoords[1].x - 0.50f ||
                snakeHead.transform.position.y < fieldCoords[0].y + 0.50f || snakeHead.transform.position.y > fieldCoords[1].y - 0.50f)
            {
                hasCollided = true;
                mobileControls.DeathScreen();
                return;
            }

            Collider[] hit = Physics.OverlapBox(snakeHead.transform.position, snakeHead.transform.localScale / 2);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.tag=="food")
                {
                    if (hit[i].gameObject.activeSelf)
                    {
                        EatFood(hit[i].gameObject);
                    }
                }
                else
                {
                    hasCollided = true;
                    mobileControls.DeathScreen();
                    return;

                }
            }

        }
        else
        {
            mobileControls.DeathScreen();
        }
    }


    void CreatePlayField()
    {
        //if already generated then return
        if (background) return;

        int[] fieldSize = new int[2];
        fieldSize[0] = (int) Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x *2;
        fieldSize[1] = (int) Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y *2;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-fieldSize[0]-1f, -fieldSize[1]-1f, 1);
        vertices[1] = new Vector3(fieldSize[0]+1f, -fieldSize[1]-1f, 1);
        vertices[2] = new Vector3(-fieldSize[0]-1f, fieldSize[1]+1f, 1);
        vertices[3] = new Vector3(fieldSize[0]+1f, fieldSize[1]+1f, 1);


        mesh.vertices = vertices;

        //playing field constrains
        fieldCoords[0] = vertices[0];
        fieldCoords[1] = vertices[3];

        mesh.triangles =new int[]{ 0,3,1,0,2,3};
        mesh.uv = new Vector2[] { Vector2.zero, Vector2.up, Vector2.right, Vector2.one };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        background = new GameObject("Background");
        background.AddComponent<MeshFilter>().mesh = mesh;
        background.AddComponent<MeshRenderer>().material = bgMat;
        background.transform.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(fieldCoords[1].y, fieldCoords[1].x ));
    }

    void GenerateFood()
    {
        switch (foodEaten)
        {
            case 11:
                PositionFood();
                PositionFood();
                break;
            case 21:
                PositionFood();
                PositionFood();
                break;
            case 31:
                PositionFood();
                PositionFood();
                PositionFood();
                break;
            default:
                PositionFood();
                break;
        }
    }

    void PositionFood()
    {
        GameObject food = pool.GetFood();
        if (food)
        {            
            food.transform.position = GenerateCoords("food");
            food.SetActive(true);
        }
    }

    public void PositionBlock()
    {
        GameObject block = Instantiate(blockPrefab,blockContainer.transform);
        block.transform.position = GenerateCoords("block");
    }    

    public void EatFood(GameObject food)
    {
        //add to current score
        //change scror UI
        //increase snake speed and tail
        //return food to pool
        //generate new food and block
        foodEaten++;

        mobileControls.ChangeScore(foodEaten);

        if (foodEaten > recordScore)
        {
            recordScore = foodEaten;
            mobileControls.ChangeRecordScore(recordScore);
            if (!isCongradulated)
            {
                isCongradulated = true;
                mobileControls.ShowCongratulationText();
            }
        }


        snakeController.IncreaseSpeed();
        snakeController.AddTailPart();

        pool.ReturnFood(food);

        GenerateFood();
        PositionBlock();
    }

    public Vector3 GenerateCoords(string tag)
    {
        Vector2 screenCoords = new Vector2();
        float[] fieldSize = new float[2];
        fieldSize[0] = fieldCoords[1].x / 6f;
        fieldSize[1] = fieldCoords[1].y/6f;

        Vector2 snakeCoords = snakeController.transform.GetChild(0).position;

        //generating random x
        screenCoords.x = Random.Range(fieldCoords[0].x+ 1.5f, fieldCoords[1].x- 1.5f);
        List<Collider> hit = new List<Collider>();
        int tries = 0;
            //if that x is not under or below snake head then generate random y
            if (screenCoords.x < snakeCoords.x - fieldSize[0] ||
                screenCoords.x > snakeCoords.x + fieldSize[0])
            {
                do
                {
                    screenCoords.y = Random.Range(fieldCoords[0].y + 1.5f, fieldCoords[1].y - 1.5f);
                    hit.AddRange(Physics.OverlapBox(screenCoords, snakeHead.transform.localScale / 2));
                    hit.RemoveAll(x => x.transform.tag == tag);
                    tries++;
                } while (hit.Count != 0 && tries < 100);

            }
            else
            {
                //else if lower boundary of snake head is lower than lower boundary of field
                //or if upper boundary of snake head below upper boundary of field and rando, number greater than 50
                //then generate y above snake head
                //otherwise below
                if ((fieldCoords[0].y > snakeCoords.y - fieldSize[1]) ||
                   !(fieldCoords[1].y < snakeCoords.y + fieldSize[1]) && (Random.Range(0, 100) > 50))
                {
                    do
                    {
                        screenCoords.y = Random.Range(snakeCoords.y + fieldSize[1] + 1.5f, fieldCoords[1].y - 1.5f);
                        hit.RemoveAll(x => x.transform.tag == tag);
                    tries++;
                } while (hit.Count != 0 && tries < 100);
            }
                else
                {
                    do
                    {
                        screenCoords.y = Random.Range(fieldCoords[0].y + 1.5f, snakeCoords.y - fieldSize[0] - 1.5f);
                    hit.RemoveAll(x => x.transform.tag == tag);
                    tries++;
                } while (hit.Count != 0 && tries < 100);
            }
        }
        return screenCoords;
    }
    

}
