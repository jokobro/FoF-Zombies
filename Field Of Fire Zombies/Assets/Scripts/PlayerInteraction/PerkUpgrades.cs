using UnityEngine;
public class PerkUpgrades : MonoBehaviour
{
    private Weapon weapon;
    private WeaponSwitching weaponSwitching;

    private bool isSpeedColaBought = false;
    private bool isJunngernautPerkBought = false;
    private bool isDoubleTapBought = false;
    private bool isQuickReviveBought = false;
    private bool hasUsedQuickRevive = false;

    public bool IsSpeedColaBought => isSpeedColaBought;
    public bool IsQuickReviveBought => isQuickReviveBought && !hasUsedQuickRevive;
    public bool IsJunngernautPerkBought => isSpeedColaBought;
    public bool IsDoubleTapBought => isDoubleTapBought;

    private void Start()
    {
        weaponSwitching = FindAnyObjectByType<WeaponSwitching>(); // Zoek WeaponSwitching
        weapon = FindAnyObjectByType<Weapon>();
    }

    public void HandleBuyingSpeedCola()
    {
        if (GameManager.Instance.Points >= 1500)
        {
            GameManager.Instance.Points -= 1500;
            GameManager.Instance.UpdatePointsUI();
            PlayerController.Instance.walkSpeed = 12.6f; // Pas de snelheid aan
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.speedColaSprite); // Voeg toe aan UI
            HUDcontroller.instance.DisableInteractionText();
            isSpeedColaBought = true;
        }
        else
        {
            HUDcontroller.instance.EnableInteractionText("Niet genoeg punten!");
        }
    }

    public void HandleBuyingQuickRevive()
    {
        if (GameManager.Instance.Points >= 1000)
        {
            GameManager.Instance.Points -= 1000;
            GameManager.Instance.UpdatePointsUI();
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.quickReviveSprite);
            HUDcontroller.instance.DisableInteractionText();
            isQuickReviveBought = true;
            hasUsedQuickRevive = false;
        }
        else
        {
            Debug.Log("Niet genoeg punten voor Quick Revive!");
            HUDcontroller.instance.EnableInteractionText("Niet genoeg punten!"); // Toon foutmelding
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
            HUDcontroller.instance.DisableInteractionText();
            isJunngernautPerkBought = true;
        }
        else
        {
            HUDcontroller.instance.EnableInteractionText("Niet genoeg punten!");
        }
    }

    public void HandleBuyingDoubleTap()
    {
        if (isJunngernautPerkBought)
        {
            Debug.Log("double tap gekocht!");
            return;
        }

        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();

            //logica toevoegen voor het upgraden firerate

            Weapon currentWeapon = weaponSwitching.GetActiveWeapon();

            if (currentWeapon != null)
            {
                weapon.fireRate = 0.1f; // tweaken met procenten
            }

            isDoubleTapBought = true;

            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.doubleTapSprite); // Voeg toe aan UI
            HUDcontroller.instance.DisableInteractionText();
        }
        else
        {
            Debug.Log("Niet genoeg punten voor doubletap!");
            HUDcontroller.instance.EnableInteractionText("Niet genoeg punten!");
        }
    }


    // Roep deze methode aan wanneer de speler Quick Revive gebruikt
    public void UseQuickRevive()
    {
        if (isQuickReviveBought)
        {
            Debug.Log("Quick Revive gebruikt!");
            hasUsedQuickRevive = true;
            isQuickReviveBought = false;

            // extra logica voor reviven toevoegen
        }
    }
}
