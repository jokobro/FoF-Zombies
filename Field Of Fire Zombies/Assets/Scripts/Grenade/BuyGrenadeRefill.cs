using UnityEngine;
public class BuyGrenadeRefill: Interactable
{
    [SerializeField] private int refillCost = 600;
    private GrenadeThrower grenadeThrower;

    private void Start()
    {
        grenadeThrower = FindObjectOfType<GrenadeThrower>();
        UpdateMessage();
    }

    private void Update()
    {
        UpdateMessage();
    }

    private void UpdateMessage()
    {
        if (grenadeThrower == null) return;

        if (grenadeThrower.CurrentGrenadeCount >= grenadeThrower.MaxGrenades)
        {
            message = "Max amount";
        }
        else if (GameManager.Instance.Points >= refillCost)
        {
            message = $"Press F to refill grenades {refillCost} points";
        }
        else
        {
            message = $"Need {refillCost} points to refill grenades";
        }
    }

    public override void HandleInteraction()
    {
        if (grenadeThrower == null) return;

        if (grenadeThrower.CurrentGrenadeCount >= grenadeThrower.MaxGrenades)
        {
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
            return;
        }
    }
}
