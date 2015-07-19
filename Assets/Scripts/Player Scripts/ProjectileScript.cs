using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour 
{
    private const float LIFE_TIME = 5.0f; //When the projectile has been in the scene for 5 seconds, despawn.
    private const float MOVE_SPEED = 2.0f;

    private GameManager gameManager = null;

    private Timer lifeTimer = null;

    private int damage = 1;
    private int pierceCount = 0;
    private int currentNumPierces = 0;

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int PierceCount
    {
        get { return pierceCount; }
        set { pierceCount = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        lifeTimer = new Timer(LIFE_TIME, true);
        lifeTimer.OnTimerComplete += Despawn;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            lifeTimer.Update();
            UpdateMovement();
        }
	}

    private void UpdateMovement()
    {
        gameObject.transform.position += gameObject.transform.right * MOVE_SPEED * Time.deltaTime;
    }

    private void Despawn()
    {
        lifeTimer.OnTimerComplete -= Despawn;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D colliderObject)
    {
        switch(colliderObject.tag)
        {
            case "Enemy":
                EnemyBehaviour enemyInfo = colliderObject.GetComponent<EnemyBehaviour>();

                if (enemyInfo != null)
                {
                    enemyInfo.OnHit(damage);

                    if (currentNumPierces >= pierceCount)
                        Despawn(); //Make sure to remove the projectile, though keeping it will be useful for a pierce effect
                    else
                        currentNumPierces++;
                }
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D colliderObject)
    {
        switch(colliderObject.gameObject.tag)
        {
            case "Environment":
                Despawn();
                break;
            default:
                break;
        }
    }
}
