using Unity.Hierarchy;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float blastRadius = 5f; 
    float countDown;
    bool hasExploded = false;

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
            
        }
        Destroy(gameObject);
    }
}
