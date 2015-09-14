using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour 
{
    private InputManager inputManager = null;
    private GameManager gameManager = null;
    private EnemySpawnManager spawnManager = null;
    private ShopManager shopManager = null;

    private Camera mainCamera = null;

    private List<GameObject> healthBlips = new List<GameObject>();

    private GameObject[] waveNumDisplay = new GameObject[3];
    private GameObject[] enemyNumDisplay = new GameObject[3];
    private GameObject[] moneyNumDisplay = new GameObject[3];
    private GameObject[] gameOverScoreDisplay = new GameObject[3];
    private GameObject[] breakTimeDisplay = new GameObject[2];

    private Sprite[] numberSprites = new Sprite[10];

    private GameObject HUDBar = null;
    private GameObject crosshair = null;
    private GameObject gameOverPanel = null;
    private GameObject scoreSubmissionStatus = null;
    private GameObject shopPanel = null;
    private GameObject breakTimePanel = null;

    private PlayerBehaviour playerInfo = null;

    private Vector2 healthBlipStartPos = new Vector2(-0.65f, 0.04f);

    private float previousBreakTime = 0.0f;

    private int previousPlayerHealth = 0;
    private int previousWaveNum = 0;
    private int previousNumEnemies = 0;
    private int previousPlayerMoney = 0;

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        gameManager = gameObject.GetComponent<GameManager>();
        spawnManager = gameObject.GetComponent<EnemySpawnManager>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        inputManager.Mouse_Moved += ProcessMousePosition;

        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        //Get the HUD, populate player health and game stats
        HUDBar = mainCamera.transform.FindChild("HUDBar").gameObject;

        for (int i = 0; i < playerInfo.Health; i++)
        {
            Vector3 newBlipLocalPosition = new Vector3(healthBlipStartPos.x, healthBlipStartPos.y, 0);

            GameObject newHealthBlip = GameObject.Instantiate(Resources.Load("Prefabs/Health_Blip") ) as GameObject;

            float blipWidth = newHealthBlip.GetComponent<SpriteRenderer>().bounds.extents.x * 2;

            newBlipLocalPosition.x += blipWidth * i;

            newHealthBlip.transform.parent = HUDBar.transform;

            newHealthBlip.transform.localPosition = newBlipLocalPosition;

            healthBlips.Add(newHealthBlip);
        }

        waveNumDisplay[0] = HUDBar.transform.FindChild("Wave_Ones").gameObject;
        waveNumDisplay[1] = HUDBar.transform.FindChild("Wave_Tens").gameObject;
        waveNumDisplay[2] = HUDBar.transform.FindChild("Wave_Hundreds").gameObject;

        enemyNumDisplay[0] = HUDBar.transform.FindChild("Enemies_Ones").gameObject;
        enemyNumDisplay[1] = HUDBar.transform.FindChild("Enemies_Tens").gameObject;
        enemyNumDisplay[2] = HUDBar.transform.FindChild("Enemies_Hundreds").gameObject;

        moneyNumDisplay[0] = HUDBar.transform.FindChild("Money_Ones").gameObject;
        moneyNumDisplay[1] = HUDBar.transform.FindChild("Money_Tens").gameObject;
        moneyNumDisplay[2] = HUDBar.transform.FindChild("Money_Hundreds").gameObject;

        crosshair = GameObject.Find("Crosshair");

        gameOverPanel = HUDBar.transform.FindChild("GameOverPanel").gameObject;
        scoreSubmissionStatus = gameOverPanel.transform.FindChild("SubmissionStatus").gameObject;
        gameOverScoreDisplay[0] = gameOverPanel.transform.FindChild("Score_Ones").gameObject;
        gameOverScoreDisplay[1] = gameOverPanel.transform.FindChild("Score_Tens").gameObject;
        gameOverScoreDisplay[2] = gameOverPanel.transform.FindChild("Score_Hundreds").gameObject;

        numberSprites = Resources.LoadAll<Sprite>("Sprites/UI/UI_Numbers");

        ShowHideGameOver(false);

        shopManager = HUDBar.transform.FindChild("ShopDisplay").GetComponent<ShopManager>();
        shopManager.ShowHideShop(false, false);

        breakTimePanel = HUDBar.transform.FindChild("WaveTimeDisplay").gameObject;
        breakTimeDisplay[0] = breakTimePanel.transform.FindChild("Seconds_Ones").gameObject;
        breakTimeDisplay[1] = breakTimePanel.transform.FindChild("Seconds_Tens").gameObject;

        gameManager.GameOverEvent += OnGameOver;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            UpdateHUD();
        }
	}

    private void ProcessMousePosition(Vector3 mousePosition)
    {
        float crosshairX = 0;
        float crosshairY = 0;

        //float screenPercentX = (mousePosition.x - Screen.width / 2) / Screen.width;
        //float screenPercentY = (mousePosition.y - Screen.height / 2) / Screen.height;

        Vector3 newMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 1) );

        crosshair.transform.position = newMousePos;
    }

    public void UpdateHUD()
    {
        //Update player health if it's changed.
        if (previousPlayerHealth != playerInfo.Health)
        {
            for (int i = 0; i < healthBlips.Count; i++)
            {
                if (i > playerInfo.Health - 1)
                    healthBlips[i].GetComponent<SpriteRenderer>().enabled = false;
                else
                    healthBlips[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            previousPlayerHealth = playerInfo.Health;
        }

        //Update Wave Num
        if (previousWaveNum != spawnManager.CurrentWave)
        {
            NumericalDisplay(waveNumDisplay, spawnManager.CurrentWave);
            previousWaveNum = spawnManager.CurrentWave;
        }

        //Update Num Enemies
        if (previousNumEnemies != spawnManager.NumEnemiesInScene)
        {
            NumericalDisplay(enemyNumDisplay, spawnManager.NumEnemiesInScene);
            previousNumEnemies = spawnManager.NumEnemiesInScene;
        }

        //Update Money
        if (previousPlayerMoney != playerInfo.Money)
        {
            NumericalDisplay(moneyNumDisplay, playerInfo.Money);
            previousPlayerMoney = playerInfo.Money;
        }

        //Update break time between waves
        if (spawnManager.IsBetweenWaves == true)
        {
            if (previousBreakTime != spawnManager.WaveBreakTimer.CurrentTime)
            {
                TimeDisplay(breakTimeDisplay, spawnManager.WaveBreakTimer.TargetTime - spawnManager.WaveBreakTimer.CurrentTime);
                previousBreakTime = spawnManager.WaveBreakTimer.CurrentTime;
            }
        }
    }

    public void NumericalDisplay(GameObject[] displayObjs, int valueToShow)
    {
        int onesValue = (valueToShow / 1) % 10;
        int tensValue = (valueToShow / 10) % 10;
        int hundredsValue = (valueToShow / 100) % 10;

        SpriteRenderer onesText = displayObjs[0].GetComponent<SpriteRenderer>();
        SpriteRenderer tensText = displayObjs[1].GetComponent<SpriteRenderer>();
        SpriteRenderer hundredsText = displayObjs[2].GetComponent<SpriteRenderer>();

        onesText.sprite = numberSprites[onesValue];
        tensText.sprite = numberSprites[tensValue];
        hundredsText.sprite = numberSprites[hundredsValue];
    }

    public void ShowHideGameOver(bool showGameOver)
    {
        gameOverPanel.GetComponent<SpriteRenderer>().enabled = showGameOver;

        for (int i = 0; i < gameOverPanel.transform.childCount; i++)
        {
            if (gameOverPanel.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                gameOverPanel.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = showGameOver;
        }
    }

    public void ShowHideWaveTime(bool showWaveTime)
    {
        breakTimePanel.GetComponent<SpriteRenderer>().enabled = showWaveTime;

        for (int i = 0; i < breakTimePanel.transform.childCount; i++)
        {
            if (breakTimePanel.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                breakTimePanel.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = showWaveTime;
        }
    }

    public void ShowGameOverScore()
    {
        NumericalDisplay(gameOverScoreDisplay, spawnManager.CurrentWave);
    }

    public void ShowSubmissionStatus(bool isLoggedIn)
    {
        if (isLoggedIn == true)
            scoreSubmissionStatus.GetComponent<AnimationScript>().PlayAnimation("Text_Submitted");
        else
            scoreSubmissionStatus.GetComponent<AnimationScript>().PlayAnimation("Text_NotSubmitted");
    }

    private void OnGameOver()
    {
        inputManager.Mouse_Moved -= ProcessMousePosition;
        gameManager.GameOverEvent -= OnGameOver;
    }

    public void ShowShopInventory()
    {
        shopManager.ShowHideShop(true, false);
    }

    private void TimeDisplay(GameObject[] displayObjs, float valueToShow)
    {
        int onesValue = (int)(valueToShow / 1) % 10;
        int tensValue = (int)(valueToShow / 10) % 10;

        SpriteRenderer onesText = displayObjs[0].GetComponent<SpriteRenderer>();
        SpriteRenderer tensText = displayObjs[1].GetComponent<SpriteRenderer>();

        onesText.sprite = numberSprites[onesValue];
        tensText.sprite = numberSprites[tensValue];
    }
}
