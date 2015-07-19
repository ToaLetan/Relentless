using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour 
{
    private const float BOUNDS_X = 1.56f;
    private const float BOUNDS_Y = 1.36f;

    private GameManager gameManager = null;

    private GameObject player = null;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(gameManager.CurrentGameState == GameManager.GameState.Running)
            FollowPlayer();
	}

    private void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 newPos = gameObject.transform.position;

            if (player.transform.position.x <= BOUNDS_X && player.transform.position.x >= -BOUNDS_X)
                newPos.x = player.transform.position.x;
            if(player.transform.position.y <= BOUNDS_Y && player.transform.position.y >= -BOUNDS_Y)
                newPos.y = player.transform.position.y;

            gameObject.transform.position = newPos;
        }
    }
}
