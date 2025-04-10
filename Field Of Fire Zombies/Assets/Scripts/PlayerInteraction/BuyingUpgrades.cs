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
            GameManager.Instance.UpdatePointsUI();
            PlayerController.Instance.walkSpeed = 12.6f; // Past de loopsnelheid aan.
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.speedColaSprite); // Voeg toe aan UI
            isSpeedColaBought = true;
        }
    }

    public void HandleBuyingQuickRevive()
    {
        if (GameManager.Instance.Points >= 1000)
        {
            GameManager.Instance.Points -= 1000;
            GameManager.Instance.UpdatePointsUI();
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.quickReviveSprite);
            isQuickReviveBought = true;
            hasUsedQuickRevive = false;
        }
    }

    public void HandleBuyingJuggernaut()
    {
        if (GameManager.Instance.Points >= 2500)
        {
            GameManager.Instance.Points -= 2500;
            GameManager.Instance.UpdatePointsUI();
            PlayerController.Instance.playerHealth = 170f;
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.juggernautSprite);
            isJugernautPerkBought = true;
        }
    }

    public void HandleBuyingDoubleTap()
    {
        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.doubleTapSprite); // Voeg toe aan UI
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

    public void HandleBuyingWeaponUpgrade()
    {
        Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();
        if (currentWeapon.isWeaponUpgraded) return; // Voorkom herhaalde interactie

        if (GameManager.Instance.Points >= 5000)
        {
            GameManager.Instance.Points -= 5000;
            GameManager.Instance.UpdatePointsUI();
            currentWeapon.fireRate = 0.150f;
            currentWeapon.damage *= 1.8f;
            currentWeapon.reloadTime *= 0.4f;
            currentWeapon.isWeaponUpgraded = true;
            HUDcontroller.instance.DisableInteractionText(); // Verberg tekst na aankoop
            PlayerInteraction.Instance.ClearInteraction(); // Zorg ervoor dat de interactietekst wordt bijgewerkt
        }
    }

    // Roep deze methode aan wanneer de speler Quick Revive gebruikt
    public void UseQuickRevive()
    {
        if (isQuickReviveBought)
        {
            Debug.Log("Quick Revive gebruikt!");
            hasUsedQuickRevive = false;
            isQuickReviveBought = true;

            // extra logica voor reviven toevoegen
        }
    }
}
