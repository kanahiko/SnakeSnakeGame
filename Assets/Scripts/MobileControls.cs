using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MobileControls : MonoBehaviour
{

    public JoystickController joystick;

    //direction of joystick controll
    public static Vector2 direction = new Vector2(0, 1);

    ///value for SmoothDamp for changing direction
    [Range(0, 1)]
    public float turnTime = 0.5f;

    //value for SmoothDamp for moving snake
    [Range(0, 1)]
    public float smoothTime = 0.35f;


    //flag and time double escape for app closing
    bool escapePress = false;
    float escapeTime = 0;

    //flag for pressing AccelerationButton
    public static bool isAccelerated = false;

    //for checking if in main menu or not
    bool isInGame = true;

    //UI elements
    public Canvas pauseMenu;
    public Text CurrentText;
    public Text RecordText;
    public Text CongratulationText;
    public Text fpsCounter;
    public Image AccButtonImage;

    void Start()
    {
        //if in main menu load record
        isInGame = SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu");
        RecordText.text = "Record: " + PlayerPrefs.GetInt("Record score", 0);
    }

    // Update is called once per frame
    void Update()
    {
        //if not in game then check for double escape
        //else show pause menu or exit pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isInGame)
            {
                if (!escapePress)
                {
                    escapePress = true;
                    escapeTime = Time.time;
                }
                else
                {
                    if (Time.time - escapeTime < 2)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        escapePress = false;
                        escapeTime = 0;
                    }
                }
                return;
            }
            else
            {
                pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
                Time.timeScale = pauseMenu.gameObject.activeSelf ? 0 : 1;
            }
        }

        if (isInGame && !GameMaster.hasCollided && Time.timeScale != 0)
        {
            if (direction != joystick.GetJoysticDirection())
            {
                // if angle between direction and joystic is too big
                //split it into two stages
                if (Vector2.Angle(direction, joystick.GetJoysticDirection()) > 96)
                {
                    direction = Vector2.Lerp(direction, new Vector2(joystick.GetJoysticDirection().y, -joystick.GetJoysticDirection().x), turnTime);
                }
                else
                {
                    direction = Vector2.Lerp(direction, joystick.GetJoysticDirection(), turnTime);
                }
            }



            //making camera not go over field
            Vector3 camPosition = new Vector3(0, 0, -10);
            float[] fieldSize = new float[2];
            fieldSize[0] = GameMaster.fieldCoords[1].x / 2;
            fieldSize[1] = GameMaster.fieldCoords[1].y / 2;

            //if snake position is over field border - middle of a screen
            if (GameMaster.snake.transform.position.x < GameMaster.fieldCoords[0].x + (fieldSize[0]))
            {
                camPosition.x = GameMaster.fieldCoords[0].x + fieldSize[0];
            }
            else if (GameMaster.snake.transform.position.x > fieldSize[0])
            {
                camPosition.x = fieldSize[0];
            }
            else
            {
                camPosition.x = GameMaster.snake.transform.position.x;
            }


            //same for y
            if (GameMaster.snake.transform.position.y < GameMaster.fieldCoords[0].y + fieldSize[1])
            {
                camPosition.y = GameMaster.fieldCoords[0].y + fieldSize[1];
            }
            else if (GameMaster.snake.transform.position.y > fieldSize[1])
            {
                camPosition.y = fieldSize[1];
            }
            else
            {
                camPosition.y = GameMaster.snake.transform.position.y;
            }

            Camera.main.transform.position = camPosition;

            //fps display
            fpsCounter.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        }
    }

    void FixedUpdate()
    {
        //comaning snake to move or game over
        if (isInGame)
        {
            if (!GameMaster.hasCollided)
            {
                GameMaster.snakeController.MoveSnake(direction, isAccelerated, smoothTime);
            }
            else
            {
                DeathScreen();
            }
        }
    }


    public void AcceleartionButtonPress(bool value)
    {
        isAccelerated = value;
        if (value)
        {
            AccButtonImage.color = new Color32(240, 52, 51, 127);
        }
        else
        {
            AccButtonImage.color = new Color32(255, 255, 255, 127);
        }
    }

    public void PauseMenuClick(bool isPaused)
    {
        //if paused stop time
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ChangeScore(int score)
    {
        CurrentText.text = "Score: " + score;
    }


    public void ChangeRecordScore(int score)
    {
        RecordText.text = "Record: " + score;
    }

    public void ExitButtonClick()
    {
        if (isInGame) SaveScore();
        Application.Quit();
    }

    public void StartButtonClick()
    {
        direction = new Vector2(0, 1);
        escapePress = false;
        escapeTime = 0;
        isAccelerated = false;
        if (isInGame)
        {
            SaveScore();
        }
        SceneManager.LoadScene("Game");
    }

    public void MainMenuButtonClick()
    {
        Time.timeScale = 1;
        SaveScore();
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowCongratulationText()
    {
        CongratulationText.color = Color.white;
        IEnumerator corouting = AnimateCongratulationText(15);
        StartCoroutine(corouting);
    }

    IEnumerator AnimateCongratulationText(int seconds)
    {
        yield return new WaitForSeconds(seconds / 5f);

        for (int i = 0; i < seconds; i++)
        {
            CongratulationText.color = new Color(1, 1, 1, CongratulationText.color.a - (1f / seconds));
            yield return null;
        }
        CongratulationText.color = new Color(1, 1, 1, 0);
    }

    public void DeathScreen()
    {
        Time.timeScale = 0;
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.transform.GetChild(1).GetComponent<Text>().text = "GAME OVER";
        pauseMenu.transform.GetChild(2).gameObject.SetActive(false);
        pauseMenu.transform.GetChild(3).GetComponent<Text>().text = "Your Score: " + GameMaster.foodEaten;
        pauseMenu.transform.GetChild(3).gameObject.SetActive(true);
    }

    void SaveScore()
    {
        if (GameMaster.foodEaten >= GameMaster.recordScore)
        {
            PlayerPrefs.SetInt("Record score", GameMaster.recordScore);
        }
    }
}
