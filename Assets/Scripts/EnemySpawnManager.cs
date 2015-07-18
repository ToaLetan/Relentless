using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour 
{
    private const float BREAK_TIME = 5.0f;
    private const float SPAWN_DELAY = 0.5f;

    private const int MAX_NUM_ENEMIES = 100;
    private const int BASE_NUM_ENEMIES = 5;

    private GameManager gameManager = null;

    private Timer spawnTimer = null; //The length ot fime between individual enemy spawns.
    private Timer waveBreakTimer = null; //The length of time between waves (use if implementing vendor)

    private List<GameObject> spawnPoints = new List<GameObject>();

    private int numEnemiesToSpawn = 0;
    private int currentWave = 0;
    private int numEnemiesInScene = 0;
    private int previousnumEnemiesInScene = 0;

    private bool isSpawningWave = false;

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

	// Use this for initialization
	void Start () 
    {
        gameManager = gameObject.transform.GetComponent<GameManager>();

        waveBreakTimer = new Timer(BREAK_TIME, true);
        waveBreakTimer.OnTimerComplete += BeginWave;

        spawnTimer = new Timer(SPAWN_DELAY);
        spawnTimer.OnTimerComplete += SpawnNewEnemy;

        SetSpawnPoints();
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
        currentWave++;

        isSpawningWave = true;

        numEnemiesToSpawn = BASE_NUM_ENEMIES * currentWave;

        spawnTimer.StartTimer();

        waveBreakTimer.ResetTimer();
    }

    private void SpawnNewEnemy()
    {
        if (spawnPoints.Count > 0)
        {
            bool resetTimerAfterSpawn = false;

            numEnemiesToSpawn--;

            GameObject newEnemy = GameObject.Instantiate(Resources.Load("Prefabs/Enemy")) as GameObject;

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

        if (numEnemiesInScene == 0 && isSpawningWave == false)
        {
            waveBreakTimer.StartTimer(); //Start the break between waves.
        }
    }
}
