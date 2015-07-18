using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlickerScript : MonoBehaviour 
{
    private const float FLICKER_TIME = 0.075f;
    private const int MAX_NUM_FLICKERS = 4;

    public Color flickerOutline = new Color(0.8f, 0.8f, 0.8f, 1);
    public Color flickerFill = new Color(0.8f, 0, 0.8f, 1);

    private GameManager gameManager = null;

    private List<Material> spriteMaterials = new List<Material>();

    private Color noColourModifier = new Color(0, 0, 0, 0);
    private Color currentColour;

    private Timer flickerTimer = new Timer(FLICKER_TIME);

    private int currentNumFlickers = 0;

    public List<Material> SpriteMaterials
    {
        get { return spriteMaterials; }
        set { spriteMaterials = value; }
    }

    public Timer FlickerTimer
    { get { return flickerTimer; } }

    public int CurrentNumFlickers
    { get { return currentNumFlickers; } }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        flickerTimer.OnTimerComplete += FlickerSprite;

        currentColour = noColourModifier;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            if (flickerTimer != null)
                flickerTimer.Update();
        }
	}

    public void FlickerSprite()
    {
        currentNumFlickers++;

        if (currentColour != flickerFill)
        {
            currentColour = flickerFill;

            for (int i = 0; i < spriteMaterials.Count; i++)
            {
                spriteMaterials[i].SetColor("_OutlineColour", flickerOutline);
                spriteMaterials[i].SetColor("_FillColour", currentColour);
            }
        }
        else
        {
            currentColour = noColourModifier;

            for (int j = 0; j < spriteMaterials.Count; j++)
            {
                spriteMaterials[j].SetColor("_OutlineColour", flickerFill);
                spriteMaterials[j].SetColor("_FillColour", currentColour);
            }
        }

        if (currentNumFlickers <= MAX_NUM_FLICKERS)
            flickerTimer.ResetTimer(true);
        else
            StopFlickerSprite();
    }

    private void StopFlickerSprite()
    {
        currentNumFlickers = 0;

        for (int k = 0; k < spriteMaterials.Count; k++)
        {
            spriteMaterials[k].SetColor("_OutlineColour", noColourModifier);
            spriteMaterials[k].SetColor("_FillColour", noColourModifier);
        }

        flickerTimer.ResetTimer();
    }
}
