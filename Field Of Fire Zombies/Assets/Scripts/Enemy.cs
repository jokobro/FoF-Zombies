using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
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

    public void TakeDamager(float damageAmount)
    {
        health -= damageAmount;
        if(health <= 0)
        {
            GameManager.Instance.AddScore(pointsAmount);
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}
