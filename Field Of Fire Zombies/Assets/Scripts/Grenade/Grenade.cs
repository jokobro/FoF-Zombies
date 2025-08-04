using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioSource explosionsound;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float delay = 3f;
    private float playerDamage = 39f; // Reduced damage for player
    private float enemyDamage = 88f; // Full damage for enemies
    private float countDown;
    private bool hasExploded = false;

    private void Start()
    {
        countDown = delay;
        explosionsound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0 && !hasExploded)
        {
            HandleExploding();
            HandleExplosionSound();
            hasExploded = true;
        }
    }

    private void HandleExplosionSound()
    {
        if (explosionsound != null && explosionsound.clip != null)
        {
            AudioSource.PlayClipAtPoint(explosionsound.clip, transform.position);
        }
    }

    private void HandleExploding()
    {
        GameObject instantiatedExplosionEffect = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
       
        // Vernietig het explosie-effect na 2 seconden
        Destroy(instantiatedExplosionEffect, 2f);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            IDamageable damageable = nearbyObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float damage = (damageable is PlayerController) ? playerDamage : enemyDamage;
                damageable.TakeDamage(damage);
            }
        }        
        Destroy(gameObject);
    }
}
