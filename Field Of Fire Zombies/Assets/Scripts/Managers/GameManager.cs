using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public float scoreMultiplier = 1f; // wanneer alles goed werkt serialzefield weghalen.
    [SerializeField] private TMP_Text pointsUiText;
    public int Points;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdatePointsUI();
    }

    public void AddScore(int pointsAmount)
    {
        Points += Mathf.RoundToInt(pointsAmount * scoreMultiplier);
        pointsUiText.SetText($"{Points}");
    }

    public void UpdatePointsUI() 
    {
        pointsUiText.SetText($"{Points}");
    }


    void ShowScoreboard()
    {
        var stats = new List<ScoreboardUI.PlayerStats>
    {
        new ScoreboardUI.PlayerStats { playerName = "jochem2005", kills = 1, points = 670, headshots = 1 }
    };

        FindObjectOfType<ScoreboardUI>().UpdateScoreboard(stats);
    }
}
