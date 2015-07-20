using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour 
{
    private const float BREAK_TIME = 30.0f;
    private const float SPAWN_DELAY = 0.5f;
    private const float MAX_MOVESPEED = 0.75f;

    private const int MAX_WAVE_NUM = 999;
    private const int MAX_NUM_ENEMIES = 300;
    private const int BASE_NUM_ENEMIES = 5;
    private const int MAX_DAMAGE = 10;
    private const int MAX_HEALTH = 50;

    public delegate void SpawnEvent();
    public event SpawnEvent WaveStart;

    private GameManager gameManager = null;
    private UIManager uiManager = null;

    private Timer spawnTimer = null; //The length ot fime between individual enemy spawns.
    private Timer waveBreakTimer = null; //The length of time between waves (use if implementing vendor)

    private List<GameObject> spawnPoints = new List<GameObject>();

    private GameObject vendor = null;

    private Vector3 vendorPosition = new Vector3(0, 0.61f, 0);

    private float currentEnemySpeed = 0.25f;

    private int numEnemiesToSpawn = 0;
    private int currentWave = 0;
    private int numEnemiesInScene = 0;
    private int previousnumEnemiesInScene = 0;
    private int difficultyWaveIncrementation = 0;
    private int currentEnemyStrength = 1;
    private int currentEnemyHealth = 3;
    
    private bool isSpawningWave = false;
    private bool isBetweenWaves = true;

    public Timer WaveBreakTimer
    {
        get { return waveBreakTimer; }
    }

    public int NumEnemiesToSpawn
    {
        get { return numEnemiesToSpawn; }
    }

    public int CurrentWave
    {
        get { return currentWave; }
    }

    public int NumEnemiesInScene
    {
        get { return numEnemiesInScene; }
    }

    public bool IsSpawningWave
    {
        get { return isSpawningWave; }
    }

    public bool IsBetweenWaves
    {
        get { return isBetweenWaves; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = gameObject.transform.GetComponent<GameManager>();
        uiManager = gameObject.transform.GetComponent<UIManager>();

        waveBreakTimer = new Timer(BREAK_TIME, true);
        waveBreakTimer.OnTimerComplete += BeginWave;

        spawnTimer = new Timer(SPAWN_DELAY);
        spawnTimer.OnTimerComplete += SpawnNewEnemy;

        SetSpawnPoints();

        SpawnDespawnVendor(true);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            CheckEnemyCount();

            waveBreakTimer.Update();
            spawnTimer.Update();
        }
	}

    private void SetSpawnPoints()
    {
        if (spawnPoints.Count > 0)
            spawnPoints.Clear();

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("EnemySpawnPoint").Length; i++)
        {
            spawnPoints.Add(GameObject.FindGameObjectsWithTag("EnemySpawnPoint")[i]);
        }
    }

    private void BeginWave()
    {
        if (WaveStart != null)
            WaveStart();

        if(currentWave < MAX_WAVE_NUM - 1)
            currentWave++;
        difficultyWaveIncrementation++;

        if (difficultyWaveIncrementation == 2)
        {
            difficultyWaveIncrementation = 0; //Reset the incrementation so every 2 waves = difficulty increase

            if(currentEnemyStrength + 1 < MAX_DAMAGE)
                currentEnemyStrength++;

            if (currentEnemySpeed + 0.05f < MAX_MOVESPEED)
                currentEnemySpeed += 0.05f;

            if(currentEnemyHealth + 1 < MAX_HEALTH)
                currentEnemyHealth ++;
        }

        isSpawningWave = true;
        isBetweenWaves = false;

        numEnemiesToSpawn = BASE_NUM_ENEMIES + (BASE_NUM_ENEMIES * currentWave);

        spawnTimer.StartTimer();

        waveBreakTimer.ResetTimer();

        uiManager.ShowHideWaveTime(false);

        SpawnDespawnVendor(false);
    }

    private void SpawnNewEnemy()
    {
        if (spawnPoints.Count > 0)
        {
            bool resetTimerAfterSpawn = false;

            numEnemiesToSpawn--;

            GameObject newEnemy = GameObject.Instantiate(Resources.Load("Prefabs/Enemy")) as GameObject;

            //Set enemy stats based on overall wave progression.
            newEnemy.GetComponent<EnemyBehaviour>().Damage = currentEnemyStrength;
            newEnemy.GetComponent<EnemyBehaviour>().MoveSpeed = currentEnemySpeed;
            newEnemy.GetComponent<EnemyBehaviour>().Health = currentEnemyHealth;

            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            newEnemy.transform.position = spawnPoint.transform.position;

            if (numEnemiesToSpawn > 0)
                resetTimerAfterSpawn = true;
            else
                isSpawningWave = false;

            spawnTimer.ResetTimer(resetTimerAfterSpawn);
        }
    }

    private void CheckEnemyCount()
    {
        numEnemiesInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if(previousnumEnemiesInScene != numEnemiesInScene)
            previousnumEnemiesInScene = numEnemiesInScene;

        if (numEnemiesInScene == 0 && isSpawningWave == false && isBetweenWaves == false)
        {
            waveBreakTimer.StartTimer(); //Start the break between waves.
            isBetweenWaves = true;
            uiManager.ShowHideWaveTime(true);
            SpawnDespawnVendor(true);
        }
    }

    private void SpawnDespawnVendor(bool spawn)
    {
        if (spawn == true)
        {
            GameObject spawnAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DeathAnim"), vendorPosition, Quaternion.identity) as GameObject;
            vendor = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Vendor"), vendorPosition, Quaternion.identity) as GameObject;
        }
        else
        {
            if (vendor != null)
            {
                GameObject despawnAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DeathAnim"), vendorPosition, Quaternion.identity) as GameObject;
                Destroy(vendor);
            }
        }
    }
}
