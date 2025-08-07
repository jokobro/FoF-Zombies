using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class waveManager : MonoBehaviour
{
    public static waveManager Instance;
    public static event Action<int> OnWaveChanged;

    [Header("Wave Settings")]
    public List<Wave> waves;
    public List<Transform> spawnPoints;

    [Header("Spawn Timing")]
    [SerializeField] private float baseSpawnDelay = 1f;
    [SerializeField] private float minSpawnDelay = 0.2f;

    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool waveInProgress = false;
    private bool forceKill = false;
    
    public int CurrentWave => currentWave + 1; // +1 zodat het vanaf 1 telt

    [SerializeField] private AudioClip roundChangeSound;
    private float powerupAudioVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        FindSpawnPoints();
        StartCoroutine(WaveRoutine());
    }

    // Wordt aangeroepen door Enemy.cs bij spawn
    private void RegisterEnemy()
    {
        activeEnemies++;
    }

    private void UnregisterEnemy()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }

    public void KillAllEnemies() // Nuke effect
    {
        forceKill = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null)
            {
                e.OnDeath -= UnregisterEnemy; // voorkom dubbele aanroep
                e.enemyDead(); // deze roept normaal OnDeath aan
                UnregisterEnemy(); // handmatig afmelden
            }
        }
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(2f); // Kleine start delay

        while (currentWave < waves.Count)
        {
            waveInProgress = true;
            forceKill = false;
         
            currentWave++; // wave 1 begint bij index 1 (voor UI consistentie)
            
            OnWaveChanged?.Invoke(currentWave);
            
            yield return StartCoroutine(SpawnWave(waves[currentWave - 1]));

            yield return new WaitUntil(() => activeEnemies <= 0 || forceKill);

            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        PlayUISound(roundChangeSound, powerupAudioVolume);
        foreach (EnemySpawnData enemyData in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyData.amount; i++)
            {
                if (forceKill) yield break;

                Transform spawnPoint = GetValidSpawnPoint();
                GameObject newEnemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);

                Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // Fixed: Only register enemy once
                    RegisterEnemy();
                    enemyScript.OnDeath += UnregisterEnemy;

                    // Scale enemy stats based on wave
                    float healthMultiplier = 1f + (currentWave - 1) * 0.1f;
                    float damageMultiplier = 1f + (currentWave - 1) * 0.1f;

                    enemyScript.health *= healthMultiplier;
                    enemyScript.damage *= damageMultiplier;

                    /*RegisterEnemy();
                    enemyScript.OnDeath += UnregisterEnemy;*/
                }

                NavMeshAgent agent = newEnemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed *= 1f + (currentWave - 1) * 0.05f;
                }

                float spawnDelay = Mathf.Max(baseSpawnDelay - (currentWave * 0.05f), minSpawnDelay);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private void PlayUISound(AudioClip clip,float volume = 5)
    {
        GameObject audioObject = new GameObject("TempAudio");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0;
        audioSource.Play();

        Destroy(audioObject,clip.length + 0.1f);
    }

    private Transform GetValidSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            FindSpawnPoints();
        }

        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        }

        return transform; // Simple fallback
    }

    private void FindSpawnPoints()
    {
        spawnPoints = new List<Transform>();

        // Zoek alle GameObjects met "SpawnPoint" in de naam
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None); // true = include inactive

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("SpawnPoint"))
            {
                spawnPoints.Add(obj.transform);
            }
        }
    }
}

[Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int amount;
}

[Serializable]
public class Wave
{
    public List<EnemySpawnData> enemiesToSpawn;
}