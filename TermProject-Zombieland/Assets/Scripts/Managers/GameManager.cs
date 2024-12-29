using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Wave Settings")]
    public int startingWave = 1;
    public int enemiesPerWave = 5;
    public float waveInterval = 10f;
    public float spawnInterval = 1f;
    public float enemyIncreaseRate = 1.2f;
    public int initialPoolSize = 10;
    public bool isAutomatic = true;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform enemyPoolParent;
    public bool usePooling = true;

    [Header("UI Settings")]
    public TextMeshProUGUI activeEnemyCountText;
    public TextMeshProUGUI waveInfoText;

    private int currentWave;
    private int enemiesToSpawn;

    private List<GameObject> enemyPool;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentWave = startingWave;
        enemiesToSpawn = enemiesPerWave;

        waveInfoText.text = $"Wave: {currentWave}";
        enemyPool = new List<GameObject>();
        if (usePooling)
        {
            InitializePool();
        }

        StartCoroutine(SpawnWave());            // Start the wave spawning process
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemyPoolParent);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    GameObject GetPooledEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        // Extend the pool if no inactive enemies are found
        GameObject newEnemy = Instantiate(enemyPrefab, enemyPoolParent);
        newEnemy.SetActive(false);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    IEnumerator SpawnWave()
    {
        while (isAutomatic)
        {
            waveInfoText.text = $"Wave: {currentWave}";

            Debug.Log($"Wave {currentWave} starting with {enemiesToSpawn} enemies");

            // Spawn enemies in this wave
            yield return StartCoroutine(SpawnEnemies(enemiesToSpawn));

            Debug.Log($"Wave {currentWave} complete. Next wave starts in {waveInterval} seconds.");

            // Wait for the interval before starting the next wave
            yield return new WaitForSeconds(waveInterval);

            // Increment wave and enemies for the next wave
            currentWave++;
            enemiesToSpawn = Mathf.RoundToInt(enemiesToSpawn * enemyIncreaseRate); // Scale up enemies
        }
    }

    IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (usePooling)
            {
                SpawnPooledEnemy();
            }
            else
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval); // Delay between spawns
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, enemyPoolParent);
        UpdateActiveEnemyCount();
    }

    void SpawnPooledEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Get an enemy from the pool
        GameObject enemy = GetPooledEnemy();
        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;
        enemy.SetActive(true);
        UpdateActiveEnemyCount();
    }

    public void KillAllAndSpawn()
    {
        UpdateActiveEnemyCount();
        StopAllCoroutines();
        foreach (Transform enemy in enemyPoolParent)
        {
            if (usePooling)
            {
                enemyPool.Add(enemy.gameObject);

                if (!enemy.gameObject.activeInHierarchy) continue;

                enemy.gameObject.SetActive(false);
            }
            else
            {
                Destroy(enemy.gameObject);
                enemyPool.Clear();
            }
        }
        StartCoroutine(SpawnEnemies(enemiesToSpawn));
    }

    void UpdateActiveEnemyCount()
    {
        int activeEnemies = 0;
        foreach (Transform enemy in enemyPoolParent)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                activeEnemies++;
            }
        }

        // Update the UI text
        if (activeEnemyCountText != null)
        {
            activeEnemyCountText.text = $"Active Enemies: {activeEnemies}";
        }
    }
}
