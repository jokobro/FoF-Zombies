using UnityEngine;
public class BuyingUpgrades : MonoBehaviour
{
    public static BuyingUpgrades Instance;
    [HideInInspector] public bool hasUsedQuickRevive = false;
    private bool isQuickReviveBought = false;
    private bool isJugernautPerkBought = false;
    private bool isDoubleTapBought = false;
    private bool isSpeedColaBought = false;

    public bool IsSpeedColaBought => isSpeedColaBought;
    public bool IsQuickReviveBought => isQuickReviveBought && !hasUsedQuickRevive;
    public bool IsJunngernautPerkBought => isJugernautPerkBought;
    public bool IsDoubleTapBought => isDoubleTapBought;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleBuyingSpeedCola()
    {
        if (GameManager.Instance.Points >= 1500)
        {
            GameManager.Instance.Points -= 1500;
            GameUIController.instance.RefreshUI();
            PlayerController.Instance.walkSpeed = 12.6f; // Past de loopsnelheid aan.
            PerkUIManager.Instance.AddPerkToUI("speedcola"); // Voeg toe aan UI
            isSpeedColaBought = true;
        }
    }

    public void HandleBuyingJuggernaut()
    {
        if (GameManager.Instance.Points >= 2500)
        {
            GameManager.Instance.Points -= 2500;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("juggernog"); // Voeg toe aan UI
            PlayerController.Instance.playerHealth = 170f;
            isJugernautPerkBought = true;
        }
    }

    public void HandleBuyingDoubleTap()
    {
        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("doubletap");
            isDoubleTapBought = true;

            // Haal alle wapens op uit de weaponSwitching
            Weapon[] allWeapons = WeaponSwitching.instance.GetAllWeapons();

            foreach (Weapon currentWeapon in allWeapons)// Loop door alle wapens en pas de vuursnelheid aan
            {
                if (currentWeapon != null)
                {
                    currentWeapon.fireRate *= 0.4f; // Verminder de vuursnelheid met 60%
                }
            }
        }
    }

    public void HandleBuyingQuickRevive()
    {
        if (GameManager.Instance.Points >= 1000)
        {
            GameManager.Instance.Points -= 1000;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("quickrevive");
            isQuickReviveBought = true;
            hasUsedQuickRevive = false; // reset!
        }
    }
    // Roep deze methode aan wanneer de speler Quick Revive gebruikt
    public void UseQuickRevive()
    {
        if (isQuickReviveBought && !hasUsedQuickRevive)
        {
            hasUsedQuickRevive = true;
            Debug.Log("Quick Revive gebruikt!");
            PerkUIManager.Instance.RemovePerkFromUI("quickrevive");

            PlayerController.Instance.StartCoroutine(PlayerController.Instance.QuickReviveRoutine());
        }
    }

    public void HandleBuyingWeaponUpgrade()
    {
        Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();
        if (currentWeapon.isWeaponUpgraded) return; // Voorkom herhaalde interactie

        if (GameManager.Instance.Points >= 5000)
        {
            GameManager.Instance.Points -= 5000;
            GameUIController.instance.RefreshUI();
            currentWeapon.fireRate = 0.150f;
            currentWeapon.damage *= 1.8f;
            currentWeapon.reloadTime *= 0.4f;
            currentWeapon.isWeaponUpgraded = true;
            GameUIController.instance.DisableInteractionText(); // Verberg tekst na aankoop
            PlayerInteraction.Instance.ClearInteraction(); // Zorg ervoor dat de interactietekst wordt bijgewerkt
        }
    }
}
