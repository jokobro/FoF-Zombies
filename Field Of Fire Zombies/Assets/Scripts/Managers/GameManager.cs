using System;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static event Action<int> OnPointsChanged;
    public static GameManager Instance;
    public float scoreMultiplier = 1f;
    public int Points;

    private void Awake()
    {
        Instance = this;
        OnPointsChanged?.Invoke(Points);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 165;
    }

    public void AddScore(int pointsAmount)
    {
        Points += Mathf.RoundToInt(pointsAmount * scoreMultiplier);
        GameUIController.instance.RefreshUI();
    }
}
