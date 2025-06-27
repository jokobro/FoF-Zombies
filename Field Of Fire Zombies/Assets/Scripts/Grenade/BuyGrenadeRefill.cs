using UnityEngine;

public class BuyGrenadeRefill : MonoBehaviour
{
    [SerializeField] private int refillCost = 600;
    private GrenadeThrower grenadeThrower;

    private void Start()
    {
        grenadeThrower = FindObjectOfType<GrenadeThrower>();
    }

    public void HandleInteraction()
    {
        if (grenadeThrower == null) return;

        if (grenadeThrower.CurrentGrenadeCount >= grenadeThrower.MaxGrenades)
        {
            Debug.Log("You already have the maximum number of grenades.");
            return;
        }

        if (GameManager.Instance.Points >= refillCost)
        {
            GameManager.Instance.Points -= refillCost;
            GameUIController.instance.RefreshUI();
            grenadeThrower.RefillGrenades();
        }
        else
        {
            Debug.Log("Not enough points to refill grenades.");
        }
    }
}
