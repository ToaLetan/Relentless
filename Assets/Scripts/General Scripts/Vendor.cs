using UnityEngine;
using System.Collections;

public class Vendor : MonoBehaviour 
{
    private const float INTERACT_RANGE = 0.25f;

    private GameManager gameManager = null;

    private GameObject prompt = null;
    private GameObject player = null;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        player = GameObject.FindGameObjectWithTag("Player").gameObject;

        prompt = gameObject.transform.GetChild(0).gameObject;
        prompt.GetComponent<SpriteRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            if (SpeculativeContacts.GetDistance(gameObject.transform.position, player.transform.position) < INTERACT_RANGE)
                ShowHidePrompt(true);
            else
                ShowHidePrompt(false);
        }
	}

    private void ShowHidePrompt(bool showPrompt)
    {
        prompt.GetComponent<SpriteRenderer>().enabled = showPrompt;
    }
}
