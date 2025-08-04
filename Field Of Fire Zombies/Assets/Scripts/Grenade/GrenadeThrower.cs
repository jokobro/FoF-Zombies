using UnityEngine;
using UnityEngine.InputSystem;
public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    private GrenadeUIManager uiManager;
    private float throwForce = 10f;
    private int grenadeAmount = 3;
    private const int maxGrenades = 3;

    public int CurrentGrenadeCount => grenadeAmount;
    public int MaxGrenades => maxGrenades;

    private void Start()
    {
        uiManager = FindAnyObjectByType<GrenadeUIManager>();
        uiManager.UpdateGrenadeUI(grenadeAmount);
    }

    public void AddGrenades(int amount)
    {
        grenadeAmount = Mathf.Min(grenadeAmount + amount, maxGrenades);
        uiManager.UpdateGrenadeUI(grenadeAmount);
    }

    public void HandleThrowingGrenade(InputAction.CallbackContext context)
    {
        if (grenadeAmount > 0 && context.performed)
        {
            GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
            grenadeAmount--;
            uiManager.UpdateGrenadeUI(grenadeAmount);
        }
    }

    public int GetGrenadeCount()
    {
        return grenadeAmount;
    }

    public void RefillGrenades()
    {
        grenadeAmount = maxGrenades;
        uiManager.UpdateGrenadeUI(grenadeAmount);
    }
}
