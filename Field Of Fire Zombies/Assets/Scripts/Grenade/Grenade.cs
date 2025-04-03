using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float delay = 3f;
    private float playerDamage = 39f; // Reduced damage for player
    private float enemyDamage = 88f; // Full damage for enemies
    private float countDown;
    private bool hasExploded = false;

    private void Start()
    {
        countDown = delay;
    }

    private void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0 && !hasExploded)
        {
            HandleExploding();
            hasExploded = true;
        }
    }

    private void HandleExploding()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(enemyDamage);
            }

            PlayerController player = nearbyObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(playerDamage);
            }
        }
        Destroy(gameObject);
    }
}
