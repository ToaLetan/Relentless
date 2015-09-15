using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour 
{
    private const float CLOSE_DELAY_TIME = 0.75f;

    private InputManager inputManager = null;
    private GameManager gameManager = null;
    private UIManager uiManager = null;
    private EnemySpawnManager spawnManager = null;
    private PlayerBehaviour playerInfo = null;

    private List<GameObject> buttonList = new List<GameObject>();

    private GameObject healthButton = null;
    private GameObject pierceButton = null;
    private GameObject damageButton = null;
    private GameObject turretButton = null;
    private GameObject closeButton = null;

    private GameObject[] healthPriceDisplay = new GameObject[3];
    private GameObject[] piercePriceDisplay = new GameObject[3];
    private GameObject[] damagePriceDisplay = new GameObject[3];
    private GameObject[] turretPriceDisplay = new GameObject[3];

    private Sprite[] numberSprites = new Sprite[10];

    private Timer closeDelayTimer = null;

    private int previousHealthPrice = 0;
    private int previousPiercePrice = 0;
    private int previousDamagePrice = 0;
    private int previousTurretPrice = 0;

    private int healthPrice = 25;
    private int piercePrice = 20;
    private int damagePrice = 10;
    private int turretPrice = 30;

    private bool givePlayerTurret = false;

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiManager = gameManager.GetComponent<UIManager>();
        spawnManager = gameManager.GetComponent<EnemySpawnManager>();

        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        healthButton = gameObject.transform.FindChild("Button_Health").gameObject;
        pierceButton = gameObject.transform.FindChild("Button_Pierce").gameObject;
        damageButton = gameObject.transform.FindChild("Button_Damage").gameObject;
        turretButton = gameObject.transform.FindChild("Button_Turret").gameObject;
        closeButton = gameObject.transform.FindChild("Button_Close").gameObject;

        buttonList.Add(healthButton);
        buttonList.Add(pierceButton);
        buttonList.Add(damageButton);
        buttonList.Add(turretButton);
        buttonList.Add(closeButton);

        healthPriceDisplay[0] = healthButton.transform.FindChild("Price_Ones").gameObject;
        healthPriceDisplay[1] = healthButton.transform.FindChild("Price_Tens").gameObject;
        healthPriceDisplay[2] = healthButton.transform.FindChild("Price_Hundreds").gameObject;

        piercePriceDisplay[0] = pierceButton.transform.FindChild("Price_Ones").gameObject;
        piercePriceDisplay[1] = pierceButton.transform.FindChild("Price_Tens").gameObject;
        piercePriceDisplay[2] = pierceButton.transform.FindChild("Price_Hundreds").gameObject;

        damagePriceDisplay[0] = damageButton.transform.FindChild("Price_Ones").gameObject;
        damagePriceDisplay[1] = damageButton.transform.FindChild("Price_Tens").gameObject;
        damagePriceDisplay[2] = damageButton.transform.FindChild("Price_Hundreds").gameObject;

        turretPriceDisplay[0] = turretButton.transform.FindChild("Price_Ones").gameObject;
        turretPriceDisplay[1] = turretButton.transform.FindChild("Price_Tens").gameObject;
        turretPriceDisplay[2] = turretButton.transform.FindChild("Price_Hundreds").gameObject;

        closeDelayTimer = new Timer(CLOSE_DELAY_TIME);

        inputManager.Key_Pressed += ProcessMouseClick;
        spawnManager.WaveStart += OnWaveStart;
        closeDelayTimer.OnTimerComplete += SetClosed;
        gameManager.GameOverEvent += OnGameOver;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            if (spawnManager.IsBetweenWaves == true)
            {
                UpdatePrices();
                if(closeDelayTimer != null)
                    closeDelayTimer.Update();
            }
        }
	}

    private void UpdatePrices()
    {
        if (previousHealthPrice != healthPrice)
        {
            uiManager.NumericalDisplay(healthPriceDisplay, healthPrice);
            previousHealthPrice = healthPrice;
        }

        if (previousPiercePrice != piercePrice)
        {
            uiManager.NumericalDisplay(piercePriceDisplay, piercePrice);
            previousPiercePrice = piercePrice;
        }

        if (previousDamagePrice != damagePrice)
        {
            uiManager.NumericalDisplay(damagePriceDisplay, damagePrice);
            previousDamagePrice = damagePrice;
        }

        if (previousTurretPrice != turretPrice)
        {
            uiManager.NumericalDisplay(turretPriceDisplay, turretPrice);
            previousTurretPrice = turretPrice;
        }
    }

    public void ShowHideShop(bool showShop, bool setInMenu)
    {
        //Loop through all objects and child objects and enable/disable their sprite renderers.
        gameObject.GetComponent<SpriteRenderer>().enabled = showShop;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
            {
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = showShop;

                if (gameObject.transform.GetChild(i).GetComponent<BoxCollider2D>() != null)
                    gameObject.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = showShop;

                if (gameObject.transform.GetChild(i).GetComponent<ButtonScript>() != null)
                    gameObject.transform.GetChild(i).GetComponent<ButtonScript>().IsMouseOver = false;

                if (gameObject.transform.GetChild(i).childCount > 0)
                {
                    for (int j = 0; j < gameObject.transform.GetChild(i).childCount; j++)
                    {
                        if (gameObject.transform.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>() != null)
                            gameObject.transform.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>().enabled = showShop;
                    }
                }
            }
        }

        if(playerInfo != null)
        {
            if (showShop == false && playerInfo.IsInMenu == true && setInMenu == true)
                playerInfo.IsInMenu = false;
        }
    }

    private void ProcessMouseClick(List<string> keysPressed)
    {
        if (keysPressed.Contains(inputManager.PlayerKeybinds.LeftMouse.ToString()) == true)
        {
            //Check if any buttons are being selected.
            for (int i = 0; i < buttonList.Count; i++)
            {
                ButtonScript buttonInfo = buttonList[i].GetComponent<ButtonScript>();

                if (buttonInfo.IsMouseOver == true)
                {
                    ProcessButton(buttonList[i]);
                }
            }
        }
    }

    private void ProcessButton(GameObject buttonObj)
    {
        switch(buttonObj.name)
        {
            case "Button_Health":
                if (playerInfo.Money >= healthPrice && playerInfo.Health < 10)
                {
                    playerInfo.Health = 10;
                    playerInfo.Money -= healthPrice;
                }
                break;
            case "Button_Pierce":
                if (playerInfo.Money >= piercePrice && playerInfo.PierceValue < 999)
                {
                    playerInfo.PierceValue++;
                    playerInfo.Money -= piercePrice;
                    piercePrice += 40;
                } 
                break;
            case "Button_Damage":
                if (playerInfo.Money >= damagePrice && playerInfo.WeaponDamage < 999)
                {
                    playerInfo.WeaponDamage++;
                    playerInfo.Money -= damagePrice;
                    damagePrice += 20;
                }
                break;
            case "Button_Turret":
                if (playerInfo.Money >= turretPrice && playerInfo.IsHoldingTurret == false && givePlayerTurret == false)
                {
                    givePlayerTurret = true;
                    playerInfo.Money -= turretPrice;
                    turretPrice += 100;
                }   
                break;
            case "Button_Close":
                ShowHideShop(false, false);
                closeDelayTimer.StartTimer();
                if (givePlayerTurret == true)
                {
                    playerInfo.AddTurret();
                    givePlayerTurret = false;
                }
                break;
            default:
                break;    
        }
    }

    private void OnWaveStart()
    {
        ShowHideShop(false, true);
    }

    private void SetClosed()
    {
        playerInfo.IsInMenu = false;
        closeDelayTimer.ResetTimer();
    }

    private void OnGameOver()
    {
        inputManager.Key_Pressed -= ProcessMouseClick;
        spawnManager.WaveStart -= OnWaveStart;
        closeDelayTimer.OnTimerComplete -= SetClosed;
        gameManager.GameOverEvent -= OnGameOver;
    }
}
