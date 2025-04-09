using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    public static Enemy Instance;
    [SerializeField] private GameObject ExplosionEffect;
    [SerializeField] List<GameObject> pickups;
    private Transform PlayerPosition;
    private NavMeshAgent agent;
    private float attackCooldown = 1.5f;
    private float lastAttackTime;
    public float attackDistance;
    public int pointsAmount;
    public float health;
    public float damage;

    private void Awake()
    {
        Instance = this;
        PlayerPosition = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        HandleAttacking();
        MoveToTarget();
    }

    private void HandleAttacking()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerPosition.position);
        if (distanceToPlayer <= attackDistance && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerController player = PlayerPosition.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }

    private void MoveToTarget()
    {
        agent.SetDestination(PlayerPosition.position);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            GameManager.Instance.AddScore(pointsAmount);
            HandleEnemyDyingPickUpDropChange();
            enemyDead();
        }
    }

    public void enemyDead()
    {
        GameObject ExplosionEffectClone = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(this.gameObject, 3);
        Destroy(ExplosionEffectClone, 2);
    }

    private void HandleEnemyDyingPickUpDropChange()
    {
        float dropChance = Random.Range(0f, 1f); // Geeft een float tussen 0 en 1
        if (dropChance <= 0.3f && pickups.Count > 0) // 30% kans en controleer of er pickups zijn
        {
            int randomIndex = Random.Range(0, pickups.Count);
            Instantiate(pickups[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
