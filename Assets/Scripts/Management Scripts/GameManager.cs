using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameJolt;

public class GameManager : MonoBehaviour 
{
    private const int SCOREBOARD_ID = 83337; //Get this info from the game's page on GameJolt

    public enum GameState { Splash, Running, Paused, Over };
    private GameState currentGameState = GameState.Splash;

    public delegate void GameEvent();
    public event GameEvent GameOverEvent;

    private InputManager gameInput = null;
    private EnemySpawnManager enemySpawner = null;
    private UIManager uiManager = null;

    private GameObject splashScreen = null;

    private string playerName = "Player";

    private int waveReached = 0;

    private bool isLoggedIn = false;

    public GameState CurrentGameState
    { get { return currentGameState; } }

	// Use this for initialization
	void Start () 
    {
        gameInput = InputManager.Instance;

        enemySpawner = gameObject.GetComponent<EnemySpawnManager>();
        uiManager = gameObject.GetComponent<UIManager>();

        splashScreen = GameObject.Find("Main Camera").transform.FindChild("SplashScreen").gameObject;

        bool isSignedIn = GameJolt.API.Manager.Instance.CurrentUser != null;

        if (isSignedIn == false)
        {
            GameJolt.UI.Manager.Instance.ShowSignIn((bool success) =>
            {
                if (success)
                {
                    playerName = GameJolt.API.Manager.Instance.CurrentUser.Name;
                    isLoggedIn = true;
                    StartGame();
                }
                else
                {
                    isLoggedIn = false;
                    StartGame();
                }
            });
        }
        else
        {
            playerName = GameJolt.API.Manager.Instance.CurrentUser.Name;
            isLoggedIn = true;
            StartGame();
        }
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

        if (GameOverEvent != null)
            GameOverEvent();

        uiManager.UpdateHUD(); //Update the player's HUD one last time.

        uiManager.ShowHideGameOver(true);
        uiManager.ShowGameOverScore();
        uiManager.ShowSubmissionStatus(isLoggedIn);

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

        //Subscribe to the input manager's key press event.
        gameInput.Key_Pressed += ProcessInput;
    }

    public void RestartGame()
    {
        gameInput.Key_Pressed -= ProcessInput;

        Application.LoadLevel("Main");
    }

    private void ProcessInput(List<string> keysPressed)
    {
        if (currentGameState == GameState.Over)
        {
            if (keysPressed.Contains(gameInput.PlayerKeybinds.Key_Interact.ToString()))
            {
                RestartGame();
            }
        }
    }

    public void ShowShopInventory()
    {
        if(enemySpawner.IsBetweenWaves == true)
            uiManager.ShowShopInventory();
    }
}
