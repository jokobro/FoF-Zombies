using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public int CurrentWaveIndex { get; private set; } = 0;
    public static waveManager Instance;
    public List<Wave> waves;  // Lijst van waves
    private int currentGroupIndex = 0;
    private bool waveActive = false;
    public float groupCompletionTime = 1f;


    [SerializeField] private TextMeshProUGUI roundNumberText;
    private int roundNumber = 1;
    /* public float spawnInterval = 30f; // Tussen de groepen in seconden*/

    
    private void Start()
    {   Instance = this;
        StartCoroutine(StartNextWave());
    }

    public void KillCurrentWave()
    {
        // Verwijder alle vijanden in de huidige wave
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy, 2);
        }

        // Update om naar de volgende wave te gaan
        waveActive = false;
        CurrentWaveIndex++;

        if (CurrentWaveIndex < waves.Count)
        {
            StartCoroutine(StartNextWave()); // Start volgende wave
        }
        else
        {
            Debug.Log("Geen waves meer over.");
        }
    }

    // Start nieuwe wave
    IEnumerator StartNextWave()
    {
        while (CurrentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[CurrentWaveIndex];
            currentGroupIndex = 0;
            waveActive = true;

            // Spawn de eerste groep
            StartCoroutine(SpawnNextGroup(currentWave));

            // Wacht tot alle groepen in de wave zijn verslagen
            while (waveActive)
            {
                yield return null;
                if (AllEnemiesDefeated())
                {
                    Debug.Log("all enemy's defeated from this group");
                    CurrentWaveIndex++;
                    waveActive = false;
                    if (CurrentWaveIndex < waves.Count)
                        yield return new WaitForSeconds(5f);  // Wacht even voordat de volgende wave start
                }
            }
        }
    }

    IEnumerator SpawnNextGroup(Wave wave)
    {
        while (currentGroupIndex < wave.groups.Count)
        {
            Group currentGroup = wave.groups[currentGroupIndex];
            foreach (GameObject enemyPrefab in currentGroup.enemies)
            {
                Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.Euler(0f, 0f, 180f));
            }

            /* float spawnTime = 0f; // Tijd voor het volgen van de spawn interval*/
            bool groupDefeated = false;

            // Wacht tot de groep is verslagen of de spawn interval is verstreken
            while (!groupDefeated)
            {
                if (AllEnemiesDefeated())
                {
                    Debug.Log("Alle vijanden van deze groep zijn verslagen.");
                    groupDefeated = true; // Groep is verslagen
                }

                /* // Check of de tijd voor spawn interval is verstreken
                 spawnTime += Time.deltaTime;
                 if (spawnTime >= spawnInterval)
                 {
                     Debug.Log("De groep is niet verslagen binnen de tijd; de volgende groep wordt gestart.");
                     groupDefeated = true; // Forceer het starten van de volgende groep
                 }*/

                yield return null; // Wacht voor de volgende frame
            }
            currentGroupIndex++;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Spawn bovenaan buiten het scherm
        float randomX = Random.Range(-8f, 8f); // Stel je breedte scherm voor
        return new Vector3(randomX, 6f, 0f); // Stel je schermhoogte voor
    }

    bool AllEnemiesDefeated()
    {   // Controleer of er nog vijanden in het spel zijn
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }
}

[System.Serializable]
public class Wave
{
    public List<Group> groups;
}

[System.Serializable]
public class Group
{
    public List<GameObject> enemies;
}