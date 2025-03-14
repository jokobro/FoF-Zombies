using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float playerInReach = 3f;


    [SerializeField] private Transform target;
    [SerializeField] List<GameObject> Pickups;
    private NavMeshAgent agent;
    private GameManager gameManager;
    public int pointsAmount;
    public float health;

    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        MoveToTarget();
        HandleAttacking();
    }

    private void HandleAttacking()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, playerInReach))
        {

        }
    }


    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if(health <= 0)
        {
            GameManager.Instance.AddScore(pointsAmount);
            Destroy(this.gameObject);
        }
    }

    private void MoveToTarget()
    {
       agent.SetDestination(target.position);       
    }
}
