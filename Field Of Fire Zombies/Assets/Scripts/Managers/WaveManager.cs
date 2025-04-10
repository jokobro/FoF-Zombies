using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class waveManager : MonoBehaviour
{
    public static waveManager Instance;
    public List<Wave> waves;
    public List<Transform> spawnPoints; // Sleep hier spawn posities in
    [SerializeField] private TextMeshProUGUI roundNumberText;

    [SerializeField] private float baseSpawnDelay = 1f;
    [SerializeField] private float minSpawnDelay = 0.2f;
    private bool forceKillWave = false;
    private int roundNumber = 1;

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

    // Start nieuwe wave
    IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            forceKillWave = false;
            Wave currentWave = waves[i];
            float spawnDelay = Mathf.Max(baseSpawnDelay - (roundNumber * 0.05f), minSpawnDelay);

            // Voor elk type enemy in deze wave
            foreach (EnemySpawnData enemyData in currentWave.enemiesToSpawn)
            {
                for (int j = 0; j < enemyData.amount; j++)
                {
                    if (forceKillWave) yield break;

                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                    GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);

                    // Stats scaling
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        float damageMultiplier = 1f + (roundNumber * 0.1f);
                        float speedMultiplier = 1f + (roundNumber * 0.05f);

                        enemyScript.damage *= damageMultiplier;
                        enemyScript.health *= damageMultiplier;

                        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                        if (agent != null)
                        {
                            agent.speed *= speedMultiplier;
                        }
                    }
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            // Wacht tot wave is verslagen
            while (!AllEnemiesDefeated() && !forceKillWave)
            {
                yield return null;
            }

            roundNumber++;
            yield return new WaitForSeconds(5f); // korte pauze
        }
    }

    bool AllEnemiesDefeated()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
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