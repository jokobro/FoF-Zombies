using UnityEngine;
using UnityEngine.InputSystem;
public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    private float throwForce = 10f;
    private int grenadeAmount = 3;

    public void HandleThrowingGrenade(InputAction.CallbackContext context)
    {
        if (grenadeAmount > 0 && context.performed)
        {
            GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
            grenadeAmount--;
        }
    }
}
