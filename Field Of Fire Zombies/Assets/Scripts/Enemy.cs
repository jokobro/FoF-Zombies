using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
  /*  [SerializeField] private Transform target;*/
    [SerializeField] List<GameObject> pickups;
    [SerializeField] private GameObject ExplosionEffect;
    private NavMeshAgent agent;
    private GameManager gameManager;
    public int pointsAmount;
    public float health;
    


    private void Awake()
    {
        
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        /*MoveToTarget();*/
        HandleAttacking();
    }

    private void HandleAttacking()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            HandleEnemyDying();
            GameManager.Instance.AddScore(pointsAmount);
            GameObject ExplosionEffectClone = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            Destroy(this.gameObject,4);
        }
    }

    private void HandleEnemyDying()
    {
        float dropChance = Random.Range(0f, 1f); // Geeft een float tussen 0 en 1
        if (dropChance <= 0.3f && pickups.Count > 0) // 30% kans en controleer of er pickups zijn
        {
            int randomIndex = Random.Range(0, pickups.Count);
            Instantiate(pickups[randomIndex], transform.position, Quaternion.identity);
        }
    }

    /*private void MoveToTarget()
    {
        agent.SetDestination(target.position);
    }*/
}
