using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class waveManager : MonoBehaviour
{
    public static waveManager Instance;
    public static event Action<int> OnWaveChanged;

    public List<Wave> waves;
    public List<Transform> spawnPoints;
    
    [SerializeField] private float baseSpawnDelay = 1f;
    [SerializeField] private float minSpawnDelay = 0.2f;

    private bool forceKillWave = false;
    private int activeEnemies = 0;
    public int roundNumber = 1;

    private void Start()
    {
        Instance = this;
        OnWaveChanged?.Invoke(roundNumber); // eerste ronde
        StartCoroutine(SpawnWaveCoroutine());
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
        Debug.Log($"RegisterEnemy aangeroepen, activeEnemies = {activeEnemies}");
    }

    public void UnregisterEnemy()
    {
        activeEnemies--;
        if (activeEnemies < 0) activeEnemies = 0; // voorkomt negatieve values
        Debug.Log($"Enemy verslagen. Nog over: {activeEnemies}");
    }

    bool AllEnemiesDefeated()
    {
        return activeEnemies <= 0;
    }

    /*IEnumerator RunWaves()
    {
        while (roundNumber - 1 < waves.Count)
        {
            forceKillWave = false;
            Wave currentWave = waves[roundNumber - 1];
            float spawnDelay = Mathf.Max(baseSpawnDelay - (roundNumber * 0.05f), minSpawnDelay);

            foreach (EnemySpawnData enemyData in currentWave.enemiesToSpawn)
            {
                for (int j = 0; j < enemyData.amount; j++)
                {
                    if (forceKillWave) 
                        break;

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
            Debug.Log($"Wachten op einde wave {roundNumber}");
            while (!AllEnemiesDefeated() && !forceKillWave)
            {
                yield return null;
            }

            roundNumber++;
            OnWaveChanged?.Invoke(roundNumber); 
            yield return new WaitForSeconds(5f);
        }
    }*/

    IEnumerator SpawnWaveCoroutine()
    {
        if (roundNumber - 1 >= waves.Count)
        {
            Debug.Log("Alle waves voltooid.");
            yield break;
        }

        forceKillWave = false;
        Wave currentWave = waves[roundNumber - 1];
        float spawnDelay = Mathf.Max(baseSpawnDelay - (roundNumber * 0.05f), minSpawnDelay);

        Debug.Log($"Wave {roundNumber} begint!");

        foreach (EnemySpawnData enemyData in currentWave.enemiesToSpawn)
        {
            for (int j = 0; j < enemyData.amount; j++)
            {
                if (forceKillWave)
                {
                    yield break;
                }

                Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
                GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    RegisterEnemy();
                    enemyScript.OnDeath += UnregisterEnemy;

                    float damageMultiplier = 1f + (roundNumber * 0.1f);
                    float speedMultiplier = 1f + (roundNumber * 0.05f);

                    enemyScript.damage *= damageMultiplier;
                    enemyScript.health *= damageMultiplier;

                    RegisterEnemy();
                    enemyScript.OnDeath += UnregisterEnemy;
                    Debug.Log($"Enemy {enemyScript.name} gespawned en event gelinkt.");
                }

                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed *= 1f + (roundNumber * 0.05f);
                }

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        Debug.Log("Wachten tot alle enemies dood zijn...");
        yield return new WaitUntil(() => AllEnemiesDefeated() || forceKillWave);
        Debug.Log("Alle enemies dood of wave geforceerd afgebroken.");

        roundNumber++;
        OnWaveChanged?.Invoke(roundNumber);
        yield return new WaitForSeconds(5f);

        StartCoroutine(SpawnWaveCoroutine());
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