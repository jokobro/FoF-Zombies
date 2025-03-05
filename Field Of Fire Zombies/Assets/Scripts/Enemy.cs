using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] List<GameObject> Pickups;
    GameManager gameManager;
    public int pointsAmount;
    public float health;

    private void Awake()
    {

        gameManager = FindObjectOfType<GameManager>();
    }



}
