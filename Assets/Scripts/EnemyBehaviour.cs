using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
    private const float BASE_MOVE_SPEED = 0.25f;

    private GameManager gameManager = null;

    private Material spriteMaterial = null;

    private FlickerScript enemyFlicker = null;

    private GameObject player = null;

    private int health = 3;
    private int damage = 1;
    private int currentNumFlickers = 0;

    private float moveSpeed = BASE_MOVE_SPEED;

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
        if(gameManager.CurrentGameState == GameManager.GameState.Running)
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
        //Probably play a rad animation before despawning.
        GameObject deathAnimation = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DeathAnim"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;

        enemyFlicker.FlickerTimer.OnTimerComplete -= enemyFlicker.FlickerSprite;
        Destroy(gameObject);
    }

    private void UpdateMovement()
    {
        //Move towards the player.
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D colliderObj)
    {
        switch(colliderObj.tag)
        {
            case "Player":
                PlayerBehaviour playerScript = colliderObj.GetComponent<PlayerBehaviour>();

                if (playerScript != null)
                {
                    playerScript.OnHit(damage, gameObject);
                }
                break;
            default:
                break;
        }
    }
}
