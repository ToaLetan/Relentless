using UnityEngine;
using System.Collections;
using GameJolt;

public class GameManager : MonoBehaviour 
{
    private const int SCOREBOARD_ID = 83337;

    public enum GameState { Splash, Running, Paused, Over };
    private GameState currentGameState = GameState.Splash;

    private InputManager gameInput = null;
    private EnemySpawnManager enemySpawner = null;

    private GameObject splashScreen = null;

    private string playerName = "Player";

    private int waveReached = 0;

    public GameState CurrentGameState
    { get { return currentGameState; } }

	// Use this for initialization
	void Start () 
    {
        gameInput = InputManager.Instance;

        enemySpawner = gameObject.GetComponent<EnemySpawnManager>();

        splashScreen = GameObject.Find("Main Camera").transform.FindChild("SplashScreen").gameObject;

        GameJolt.UI.Manager.Instance.ShowSignIn((bool success) => 
        {
            if (success)
            {
                playerName = GameJolt.API.Manager.Instance.CurrentUser.Name;
                Debug.Log("The user: " + playerName + " signed in!");

                StartGame();
            }
            else
            {
                Debug.Log("The user failed to signed in or closed the window :(");

                StartGame();
            }
            });
	    }
	
	// Update is called once per frame
	void Update () 
    {
        if (gameInput != null)
            gameInput.Update();
	}

    public void GameOver()
    {
        currentGameState = GameState.Over;

        //If the user has signed in, submit the high score.
        waveReached = enemySpawner.CurrentWave;

        string scoreboardText = "Wave reached: " + waveReached;

        GameJolt.API.Scores.Add(waveReached, scoreboardText, SCOREBOARD_ID, "", (bool success) => 
        {
            Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
        });
    }

    public void StartGame()
    {
        //Hide the splash screen, enable objects movement.
        splashScreen.GetComponent<SpriteRenderer>().enabled = false;

        Cursor.visible = false; //Hide the mouse cursor

        currentGameState = GameState.Running;
    }
}
