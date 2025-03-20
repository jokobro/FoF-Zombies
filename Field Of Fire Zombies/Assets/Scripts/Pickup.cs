using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float duration;
    /*  [SerializeField] private float bonus;
        [SerializeField] private float speed;*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Controleer of het object de speler is
        {
            Debug.Log("Pickup geactiveerd");
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null) // Controleer of PlayerController bestaat
            {
                playerController.ActivatePowerup(id, duration, this.gameObject);
            }
        }
    }
}

