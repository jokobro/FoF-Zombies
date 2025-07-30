using System.Collections.Generic;
using UnityEngine;
public class BuyingUpgrades : Interactable
{
    public static BuyingUpgrades Instance;

    private HashSet<PerkType> boughtPerks = new HashSet<PerkType>();
    public bool IsPerkBought(PerkType perk) => boughtPerks.Contains(perk);

    public PerkType perkType;
   

    public enum PerkType
    {
        SpeedCola,
        Juggernog,
        DoubleTap,
    }

    private void Awake()
    {
        Instance = this;
    }
    public override void HandleInteraction()
    {
        base.HandleInteraction(); // Dit roept je onInteraction event aan dat je in de Inspector hebt ingesteld
    }

    
    public void HandleBuyingSpeedCola()
    {
        if (IsPerkBought(PerkType.SpeedCola)) return;

        if (GameManager.Instance.Points >= 1500)
        {
            GameManager.Instance.Points -= 1500;
            GameUIController.instance.RefreshUI();
            PlayerController.Instance.walkSpeed = 12.6f;
            PerkUIManager.Instance.AddPerkToUI("speedcola");
            boughtPerks.Add(PerkType.SpeedCola);

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }

    public void HandleBuyingJuggernaut()
    {
        if (IsPerkBought(PerkType.Juggernog)) return;

        if (GameManager.Instance.Points >= 2500)
        {
            GameManager.Instance.Points -= 2500;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("juggernog");
            PlayerController.Instance.playerMaxHealth = 170f;
            boughtPerks.Add(PerkType.Juggernog);

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }

    public void HandleBuyingDoubleTap()
    {
        if (IsPerkBought(PerkType.DoubleTap)) return;

        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("doubletap");
            boughtPerks.Add(PerkType.DoubleTap);

            Weapon[] allWeapons = WeaponSwitching.instance.GetAllWeapons();
            foreach (Weapon currentWeapon in allWeapons)
            {
                if (currentWeapon != null)
                {
                    currentWeapon.fireRate *= 0.4f;
                }
            }

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
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
