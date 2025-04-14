using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float duration;

    private void Start ()
    {
        Destroy(this.gameObject,7);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Controleer of het object de speler is
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null) // Controleer of PlayerController bestaat
            {
                playerController.ActivatePowerup(id, duration, this.gameObject);
            }
        }
    }
}

