using UnityEngine;
using TMPro;

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
}
