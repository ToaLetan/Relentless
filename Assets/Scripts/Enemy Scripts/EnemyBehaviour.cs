﻿using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
    private const float BASE_MOVE_SPEED = 0.75f;

    private GameManager gameManager = null;

    private Material spriteMaterial = null;

    private FlickerScript enemyFlicker = null;

    private AutoTurret turretTargetingThis = null;

    private GameObject player = null;

    private Vector3 destination = Vector3.zero;

    public Vector2 movementDirection = Vector2.zero;

    private float moveSpeed = BASE_MOVE_SPEED;

    private int health = 3;
    private int damage = 1;
    private int currentNumFlickers = 0;

    private bool isStuck = false;

    public AutoTurret TurretTargetingThis
    {
        get { return turretTargetingThis; }
        set { turretTargetingThis = value; }
    }

    public int Health
    { 
        get { return health; }
        set { health = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        enemyFlicker = gameObject.GetComponent<FlickerScript>();

        spriteMaterial = gameObject.GetComponent<SpriteRenderer>().material;
        enemyFlicker.SpriteMaterials.Add(spriteMaterial);

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
            UpdateMovement();
	}

    public void OnHit(int damageTaken)
    {
        //Apply damage and the flicker effect.
        if(enemyFlicker.CurrentNumFlickers == 0)
            enemyFlicker.FlickerSprite();

        health -= damageTaken;

        if (health <= 0)
            Despawn();
    }

    private void Despawn()
    {
        //Probably play a rad animation before despawning. Give the player money to spend on the vendor.
        if (player.GetComponent<PlayerBehaviour>().Money < PlayerBehaviour.MAX_MONEY - 1)
        player.GetComponent<PlayerBehaviour>().Money += 1;

        if (turretTargetingThis != null)
            turretTargetingThis.CurrentTarget = null;

        GameObject deathAnimation = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DeathAnim"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;

        enemyFlicker.FlickerTimer.OnTimerComplete -= enemyFlicker.FlickerSprite;
        Destroy(gameObject);
    }

    private void UpdateMovement()
    {
        //Move towards the player.
        if (isStuck == false)
            destination = player.transform.position;

        Vector3 vectorToPlayer = Vector3.Normalize(player.transform.position - gameObject.transform.position);

        movementDirection = new Vector2(vectorToPlayer.x, vectorToPlayer.y);

        if (SpeculativeContacts.CheckObjectContact(gameObject, movementDirection, moveSpeed) == false) //If not colliding with the environment, move.
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destination, moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D colliderObj)
    {
        switch(colliderObj.tag)
        {
            case "Player":
                PlayerBehaviour playerScript = colliderObj.GetComponent<PlayerBehaviour>();

                if (playerScript != null && playerScript.InvincibilityTimer.IsTimerRunning == false)
                {
                    playerScript.OnHit(damage, gameObject);
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D colliderObj)
    {
        switch (colliderObj.tag)
        {
            case "Player":
                PlayerBehaviour playerScript = colliderObj.GetComponent<PlayerBehaviour>();

                if (playerScript != null && playerScript.InvincibilityTimer.IsTimerRunning == false)
                {
                    playerScript.OnHit(damage, gameObject);
                }
                break;
            default:
                break;
        }
    }
}
