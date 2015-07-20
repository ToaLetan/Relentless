﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour 
{
    private const float MOVE_SPEED = 1.0f;
    private const float KNOCKBACK = 5.0f;
    private const float ARM_POSITION_X_DEFAULT = 0.0601f;
    private const float ARM_POSITION_X_LEFT = 0.0292f;
    private const float ARM_POSITION_X_RIGHT = -0.0006f;
    private const float INVINCIBLE_TIME = 0.5f;
    private const float INTERACTION_RANGE = 0.25f;

    public enum PlayerState { Idle, Dead }

    private InputManager playerInput = null;
    private GameManager gameManager = null;

    private GlobalConstants.Direction_Indices currentDirection = GlobalConstants.Direction_Indices.DOWN;
    private GlobalConstants.Direction_Indices prevDirectionPressed;

    private List<int> prevPressedKeyIDs = new List<int>();

    private List<Material> spriteMaterials = new List<Material>();

    private GameObject playerShoulder = null;
    private GameObject playerArm = null;
    private GameObject playerWeapon = null;
    private GameObject crosshair = null;

    private FlickerScript playerFlicker = null;
    private AnimationScript playerAnimScript = null;
    private AnimationScript shoulderAnimScript = null;

    private Timer invincibilityTimer = null;

    private PlayerState currentState = PlayerState.Idle;

    private Vector2 currDirectionVector = Vector2.zero;

    private int health = 10;
    private int pierceValue = 0;
    private int weaponDamage = 1;
    private int money = 300;

    public Timer InvincibilityTimer
    { get { return invincibilityTimer; } }

    public PlayerState CurrentState
    { get { return currentState; } }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int PierceValue
    {
        get { return pierceValue; }
        set { pierceValue = value; }
    }

    public int WeaponDamage
    {
        get { return weaponDamage; }
        set { weaponDamage = value; }
    }

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

	// Use this for initialization
	void Start () 
    {
        playerInput = InputManager.Instance;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //Get all player child objects for easy access later.
        playerShoulder = gameObject.transform.FindChild("Player_Shoulder").gameObject;
        playerArm = gameObject.transform.FindChild("Player_Arm").gameObject;
        playerWeapon = playerArm.transform.FindChild("Weapon").gameObject;

        playerFlicker = gameObject.GetComponent<FlickerScript>();
        playerAnimScript = gameObject.GetComponent<AnimationScript>();
        shoulderAnimScript = playerShoulder.GetComponent<AnimationScript>();

        invincibilityTimer = new Timer(INVINCIBLE_TIME);

        //Subscribe to necessary events.
        playerInput.Key_Held += ProcessMovement;
        playerInput.Key_Released += Idle;
        playerInput.Key_Pressed += ProcessKeyPress;
        invincibilityTimer.OnTimerComplete += EndInvincibilityTime;
        gameManager.GameOverEvent += OnGameOver;

        crosshair = GameObject.Find("Crosshair");

        //Populate the sprite materials with all sub-objects' materials.
        spriteMaterials.Add(gameObject.GetComponent<SpriteRenderer>().material);
        spriteMaterials.Add(playerShoulder.GetComponent<SpriteRenderer>().material);
        spriteMaterials.Add(playerArm.GetComponent<SpriteRenderer>().material);
        spriteMaterials.Add(playerWeapon.GetComponent<SpriteRenderer>().material);

        playerFlicker.SpriteMaterials = spriteMaterials;

        currDirectionVector = GlobalConstants.DOWN_VECTOR;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            invincibilityTimer.Update();
            UpdateCrosshairAim();
        }
	}

    private void ProcessMovement(List<string> keys)
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            Vector2 moveDirection = Vector2.zero;

            if (currentState == PlayerState.Idle)
            {
                if (keys.Contains(playerInput.PlayerKeybinds.Key_Up.ToString()) || keys.Contains(playerInput.PlayerKeybinds.Key_Down.ToString())
                    || keys.Contains(playerInput.PlayerKeybinds.Key_Left.ToString()) || keys.Contains(playerInput.PlayerKeybinds.Key_Right.ToString()))
                {
                    SetDirection(keys);

                    switch (currentDirection)
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
                        prevPressedKeyIDs.Add(playerInput.KeyIDs[keys[i]]);
                    }

                }
            }
        }
    }

    private void ProcessKeyPress(List<string> keysPressed)
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            if (keysPressed.Contains(playerInput.PlayerKeybinds.LeftMouse.ToString()) )
            {
                Shoot();
            }
            if (keysPressed.Contains(playerInput.PlayerKeybinds.Key_Interact.ToString()) )
            {
                GameObject vendor = GameObject.Find("Vendor");

                if (vendor != null)
                {
                    if (SpeculativeContacts.GetDistance(gameObject.transform.position, vendor.transform.position) <= INTERACTION_RANGE)
                        UseVendor();
                }
            }
        }
    }

    private void Idle(List<string> keysReleased)
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            if (keysReleased.Contains(playerInput.PlayerKeybinds.Key_Up.ToString()) && keysReleased.Contains(playerInput.PlayerKeybinds.Key_Down.ToString())
                && keysReleased.Contains(playerInput.PlayerKeybinds.Key_Left.ToString()) && keysReleased.Contains(playerInput.PlayerKeybinds.Key_Right.ToString()) )
            {
                playerAnimScript.PlayAnimation("Player_Idle_" + DirectionName() );
                shoulderAnimScript.PlayAnimation("PlayerShoulder_Idle_" + DirectionName() );  
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

        SetArmPosition();

        playerAnimScript.PlayDirectionalAnimation("Player_Walk_" + DirectionName() );
        shoulderAnimScript.PlayDirectionalAnimation("PlayerShoulder_Walk_" + DirectionName());
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

        if (SpeculativeContacts.CheckObjectContact(gameObject, direction, MOVE_SPEED * Time.deltaTime) == false)
        {
            newPosition.x += direction.x * Time.deltaTime * MOVE_SPEED;
            newPosition.y += direction.y * Time.deltaTime * MOVE_SPEED;
        }

        gameObject.transform.position = newPosition;
    }

    private void UpdateCrosshairAim()
    {
        //Keep the arm rotated towards the Crosshair object.
        float deltaX = crosshair.transform.position.x - playerArm.transform.position.x;
        float deltaY = crosshair.transform.position.y - playerArm.transform.position.y;

        float armAngle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg * -1;

        Quaternion newRotation = Quaternion.AngleAxis(armAngle, -Vector3.forward);

        playerArm.transform.rotation = newRotation;
    }

    private void Shoot()
    {
        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player_Projectile"), playerWeapon.transform.position, playerWeapon.transform.rotation) as GameObject;

        projectile.GetComponent<ProjectileScript>().Damage = weaponDamage;
        projectile.GetComponent<ProjectileScript>().PierceCount = pierceValue;
    }

    public void OnHit(int damageTaken, GameObject enemy)
    {
        //Apply damage, play the flicker effect and push the player back.
        if (invincibilityTimer.IsTimerRunning == false)
        {
            invincibilityTimer.StartTimer();

            health -= damageTaken;

            //Push the player away from the enemy that dealt damage
            Vector3 vectorToKnockback = Vector3.Normalize(enemy.transform.position - gameObject.transform.position);
            Vector2 knockbackDirection = new Vector2(-vectorToKnockback.x, -vectorToKnockback.y);

            int directionModifier = 0;

            if (SpeculativeContacts.CheckObjectContact(gameObject, knockbackDirection, KNOCKBACK * Time.deltaTime) == false)
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, enemy.transform.position, KNOCKBACK * Time.deltaTime * -1);

            if (health <= 0)
            {

                PlayerDeath();
            }
            else
            {
                playerFlicker.FlickerSprite();
            }
        }
    }

    private void PlayerDeath()
    {
        GameObject deathAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DeathAnim"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;

        playerInput.Key_Held -= ProcessMovement;
        playerInput.Key_Pressed -= ProcessKeyPress;
        playerInput.Key_Released -= Idle;
        invincibilityTimer.OnTimerComplete -= EndInvincibilityTime;

        Destroy(gameObject);

        gameManager.GameOver();
    }

    private string DirectionName()
    {
        string directionName = "";

        switch(currentDirection)
        {
            case GlobalConstants.Direction_Indices.UP:
                directionName = "Up";
                break;
            case GlobalConstants.Direction_Indices.DOWN:
                directionName = "Down";
                break;
            case GlobalConstants.Direction_Indices.LEFT:
                directionName = "Left";
                break;
            case GlobalConstants.Direction_Indices.RIGHT:
                directionName = "Right";
                break;
            default:
                break;
        }

        return directionName;
    }

    private void SetArmPosition()
    {
        switch (currentDirection)
        {
            case GlobalConstants.Direction_Indices.UP:
            case GlobalConstants.Direction_Indices.DOWN:
                playerArm.transform.localPosition = new Vector3(ARM_POSITION_X_DEFAULT, playerArm.transform.localPosition.y, playerArm.transform.localPosition.z);
                break;
            case GlobalConstants.Direction_Indices.LEFT:
                playerArm.transform.localPosition = new Vector3(ARM_POSITION_X_LEFT, playerArm.transform.localPosition.y, playerArm.transform.localPosition.z);
                break;
            case GlobalConstants.Direction_Indices.RIGHT:
                playerArm.transform.localPosition = new Vector3(ARM_POSITION_X_RIGHT, playerArm.transform.localPosition.y, playerArm.transform.localPosition.z);
                break;
        }
    }

    private void EndInvincibilityTime()
    {
        invincibilityTimer.ResetTimer();
    }

    private void OnGameOver()
    {
        playerInput.Key_Held -= ProcessMovement;
        playerInput.Key_Pressed -= ProcessKeyPress;
        playerInput.Key_Released -= Idle;
        invincibilityTimer.OnTimerComplete -= EndInvincibilityTime;
    }

    private void UseVendor()
    {
        //Show the vendor interface
        gameManager.ShowShopInventory();
    }
}
