using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour 
{
    private const float BOUNDS_X = 1.56f;
    private const float BOUNDS_Y_BOTTOM = -1.36f;
    private const float BOUNDS_Y_TOP = 1.6f;

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
            if (player.transform.position.y <= BOUNDS_Y_TOP && player.transform.position.y >= BOUNDS_Y_BOTTOM)
                newPos.y = player.transform.position.y;

            gameObject.transform.position = newPos;
        }
    }
}
