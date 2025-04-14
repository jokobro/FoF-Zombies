using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject scoreboardRowPrefab;
    [SerializeField] private Transform scoreboardContent;

    public class PlayerStats
    {
        public string playerName;
        public int kills;
        public int points;
        public int headshots;
    }

    public void UpdateScoreboard(List<PlayerStats> playerStatsList)
    {
        foreach (Transform child in scoreboardContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var stats in playerStatsList)
        {
            GameObject row = Instantiate(scoreboardRowPrefab, scoreboardContent);
            TextMeshProUGUI[] columns = row.GetComponentsInChildren<TextMeshProUGUI>();

            columns[0].text = stats.playerName;
            columns[1].text = stats.kills.ToString();
            columns[2].text = stats.points.ToString();
        }
    }
}
