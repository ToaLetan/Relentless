using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour 
{
    public const float MOVE_SPEED = 1.0f;

    public enum PlayerState { Idle, Dead }

    private InputManager playerInput = null;

    private GlobalConstants.Direction_Indices currentDirection = GlobalConstants.Direction_Indices.DOWN;
    private GlobalConstants.Direction_Indices prevDirectionPressed;

    private List<int> prevPressedKeyIDs = new List<int>();

    private PlayerState currentState = PlayerState.Idle;

    private Vector2 currDirectionVector = Vector2.zero;

    public PlayerState CurrentState
    { get { return currentState; } }

	// Use this for initialization
	void Start () 
    {
        playerInput = InputManager.Instance;

        playerInput.Key_Held += ProcessMovement;

        currDirectionVector = GlobalConstants.DOWN_VECTOR;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessMovement(List<string> keys)
    {
        Vector2 moveDirection = Vector2.zero;

        if (currentState == PlayerState.Idle)
        {
            if (keys.Contains(playerInput.PlayerKeybinds.Key_Up.ToString()) || keys.Contains(playerInput.PlayerKeybinds.Key_Down.ToString())
                || keys.Contains(playerInput.PlayerKeybinds.Key_Left.ToString()) || keys.Contains(playerInput.PlayerKeybinds.Key_Right.ToString()) )
            {
                SetDirection(keys);

                switch(currentDirection)
                {
                    case GlobalConstants.Direction_Indices.UP:
                        moveDirection = GlobalConstants.UP_VECTOR;
                        break;
                    case GlobalConstants.Direction_Indices.DOWN:
                        moveDirection = GlobalConstants.DOWN_VECTOR;
                        break;
                    case GlobalConstants.Direction_Indices.LEFT:
                        moveDirection = GlobalConstants.LEFT_VECTOR;
                        break;
                    case GlobalConstants.Direction_Indices.RIGHT:
                        moveDirection = GlobalConstants.RIGHT_VECTOR;
                        break;
                }

                if (moveDirection != Vector2.zero) //If the player is heading a certain direction, apply movement.
                    Move(moveDirection);

                prevDirectionPressed = currentDirection;

                prevPressedKeyIDs.Clear();
                for (int i = 0; i < keys.Count; i++)
                {
                    prevPressedKeyIDs.Add(playerInput.KeyIDs[keys[i]] );
                }

            }
        }
    }

    private void SetDirection(List<string> keys)
    {
        List<int> currPressedKeyIDS = new List<int>();

        for (int i = 0; i < keys.Count; i++)
            currPressedKeyIDS.Add(playerInput.KeyIDs[keys[i]] );

        string mostRecentKeyPress = GetMostRecentKeyPress(currPressedKeyIDS);

        //Set the player's direction based on the most recent key pressed. If playing different idle anims, play them here.
        if (mostRecentKeyPress == playerInput.PlayerKeybinds.Key_Up.ToString())
            currentDirection = GlobalConstants.Direction_Indices.UP;
        if (mostRecentKeyPress == playerInput.PlayerKeybinds.Key_Down.ToString())
            currentDirection = GlobalConstants.Direction_Indices.DOWN;
        if (mostRecentKeyPress == playerInput.PlayerKeybinds.Key_Left.ToString())
            currentDirection = GlobalConstants.Direction_Indices.LEFT;
        if (mostRecentKeyPress == playerInput.PlayerKeybinds.Key_Right.ToString())
            currentDirection = GlobalConstants.Direction_Indices.RIGHT;
    }

    private string GetMostRecentKeyPress(List<int> currPressKeyIDs)
    {
        int mostRecentKeyID = -1;
        string mostRecentKeyString = "";

        //Get the ID number of the most recent key.
        if (currPressKeyIDs.Count == 1)
            mostRecentKeyID = currPressKeyIDs[0];
        else
        {
            for (int i = 0; i < currPressKeyIDs.Count; i++)
            {
                if (prevPressedKeyIDs.Contains(currPressKeyIDs[i]) == false)
                    mostRecentKeyID = currPressKeyIDs[i];
            }
        }

        foreach (KeyValuePair<string, int> keyIDEntry in playerInput.KeyIDs) //Get the name of the Key
        {
            if (keyIDEntry.Value == mostRecentKeyID)
                mostRecentKeyString = keyIDEntry.Key;
        }

        return mostRecentKeyString;
    }

    private void Move(Vector2 direction)
    {
        Vector3 newPosition = gameObject.transform.position;

        newPosition.x += direction.x * Time.deltaTime * MOVE_SPEED;
        newPosition.y += direction.y * Time.deltaTime * MOVE_SPEED;

        gameObject.transform.position = newPosition;
    }
}
