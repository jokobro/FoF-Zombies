using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float Points;


    [SerializeField] private float scoreMultiplier = 1f; // wanneer alles goed werkt serialzefield weghalen.

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int pointsAmount)
    {
        Points += Mathf.RoundToInt(pointsAmount * scoreMultiplier);
    }

}
