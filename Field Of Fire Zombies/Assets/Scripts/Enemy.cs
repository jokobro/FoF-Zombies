using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Combat Settings")]
    public float health = 100f;
    public float damage = 10f;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1.5f;
    public int pointsAmount = 10;

    [Header("References")]
    [SerializeField] private List<GameObject> pickups;
    [SerializeField] private Transform spawnPlace;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private NavMeshAgent agent;
    private Transform playerPosition;
    private float lastAttackTime;
    private bool isDead = false;
    public event Action OnDeath;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerPosition = GameObject.Find("Player").transform;
        animator.applyRootMotion = true;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
        animator.SetFloat("DistanceToPlayer", distanceToPlayer);
        MoveToTarget(distanceToPlayer);
        HandleAttacking(distanceToPlayer);
    }

    private void HandleAttacking(float distanceToPlayer)
    {
        if (!isDead && distanceToPlayer <= attackDistance && Time.time > lastAttackTime + attackCooldown)
        {
            Vector3 lookDirection = (playerPosition.position - transform.position).normalized;
            lookDirection.y = 0f; // Zorgt dat hij niet omhoog/omlaag kijkt
            transform.rotation = Quaternion.LookRotation(lookDirection);

            PlayerController player = playerPosition.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }

    private void MoveToTarget(float distanceToPlayer)
    {
        if (isDead == false)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(playerPosition.position);
                agent.isStopped = distanceToPlayer <= attackDistance;
                animator.applyRootMotion = false;
            }
        }
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
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke(); // Belangrijk voor WaveManager!
        agent.enabled = false;
        animator.applyRootMotion = true;
        animator.SetTrigger("Die");
        capsuleCollider.enabled = false;
        GameManager.Instance.AddScore(125);        
        Destroy(gameObject, 3f);
    }


    private void HandleEnemyDyingPickUpDropChange()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f); // Geeft een float tussen 0 en 1
        if (dropChance <= 0.3f && pickups.Count > 0) // 30% kans en controleer of er pickups zijn
        {
            int randomIndex = UnityEngine.Random.Range(0, pickups.Count);
            Instantiate(pickups[randomIndex], spawnPlace.position, Quaternion.identity);
        }
    }
}