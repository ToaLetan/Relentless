using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public enum GameState { Splash, Running, Paused, Over };
    private GameState currentGameState = GameState.Running;

    InputManager gameInput = null;

    public GameState CurrentGameState
    { get { return currentGameState; } }

	// Use this for initialization
	void Start () 
    {
        gameInput = InputManager.Instance;
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
    }
}
