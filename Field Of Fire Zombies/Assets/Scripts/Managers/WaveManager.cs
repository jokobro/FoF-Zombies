using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class waveManager : MonoBehaviour
{
    public static waveManager Instance;
    public List<Wave> waves;
    public List<Transform> spawnPoints;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private float baseSpawnDelay = 1f;
    [SerializeField] private float minSpawnDelay = 0.2f;

    private bool forceKillWave = false;
    private int activeEnemies = 0;

    public int roundNumber = 1;

    private void Start()
    {
        Instance = this;
        StartCoroutine(RunWaves());
    }

    private void Update()
    {
        roundNumberText.SetText($"{roundNumber}");
    }

    public void KillCurrentWave()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enemyDead();
            }
        }
        forceKillWave = true;
    }

    public void RegisterEnemy()
    {
        activeEnemies++;
    }

    public void UnregisterEnemy()
    {
        activeEnemies--;
    }

    bool AllEnemiesDefeated()
    {
        return activeEnemies <= 0;
    }

    IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            forceKillWave = false;
            Wave currentWave = waves[i];
            float spawnDelay = Mathf.Max(baseSpawnDelay - (roundNumber * 0.05f), minSpawnDelay);

            foreach (EnemySpawnData enemyData in currentWave.enemiesToSpawn)
            {
                for (int j = 0; j < enemyData.amount; j++)
                {
                    if (forceKillWave) yield break;

                    Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
                    GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);

                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        float damageMultiplier = 1f + (roundNumber * 0.1f);
                        float speedMultiplier = 1f + (roundNumber * 0.05f);

                        enemyScript.damage *= damageMultiplier;
                        enemyScript.health *= damageMultiplier;

                        RegisterEnemy();
                        enemyScript.OnDeath += UnregisterEnemy; 
                    }

                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.speed *= 1f + (roundNumber * 0.05f);
                    }

                    yield return new WaitForSeconds(spawnDelay);
                }
            }
            while (!AllEnemiesDefeated() && !forceKillWave)
            {
                yield return null;
            }

            roundNumber++;
            yield return new WaitForSeconds(5f);
        }
    }
}
[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int amount;
}

[System.Serializable]
public class Wave
{
    public List<EnemySpawnData> enemiesToSpawn;
}