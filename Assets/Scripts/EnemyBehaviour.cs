using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
    private const float BASE_MOVE_SPEED = 2.0f;

    private GameManager gameManager = null;

    private Material spriteMaterial = null;

    private FlickerScript enemyFlicker = null;

    private int health = 3;
    private int currentNumFlickers = 0;

    private float moveSpeed = BASE_MOVE_SPEED;

    public int Health
    { 
        get { return health; }
        set { health = value; }
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

        enemyFlicker = gameObject.GetComponent<FlickerScript>();

        spriteMaterial = gameObject.GetComponent<SpriteRenderer>().material;
        enemyFlicker.SpriteMaterials.Add(spriteMaterial);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
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
        enemyFlicker.FlickerTimer.OnTimerComplete -= enemyFlicker.FlickerSprite;
        Destroy(gameObject);
    }
}
