using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Points;
    [SerializeField] private TMP_Text pointsUiText;
    [SerializeField] public float scoreMultiplier = 1f; // wanneer alles goed werkt serialzefield weghalen.

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
}
