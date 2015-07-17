using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    InputManager gameInput = null;

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
}
