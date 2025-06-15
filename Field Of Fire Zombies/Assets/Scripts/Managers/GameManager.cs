using System;
using UnityEngine;
using UnityEngine.UIElements;
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
    }
        
    public void AddScore(int pointsAmount)
    {
        Points += Mathf.RoundToInt(pointsAmount * scoreMultiplier);
        GameUIController.instance.RefreshUI();
        //hopelijk werkt hij nu goed nog ff testen
    }
}
