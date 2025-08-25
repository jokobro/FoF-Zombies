using UnityEngine;

public class BuyingUpgrades : Interactable
{
    [SerializeField] private AudioSource weaponUpgradeSound;
   
    [Header("Perk Configuration")]
    public PerkType perkType;

    public enum PerkType
    {
        SpeedCola,
        Juggernog,
        DoubleTap,
        WeaponUpgrade
    }

    public override void HandleInteraction()
    {
        switch (perkType)
        {
            case PerkType.SpeedCola:
                HandleBuyingSpeedCola();
                break;
            case PerkType.Juggernog:
                HandleBuyingJuggernaut();
                break;
            case PerkType.DoubleTap:
                HandleBuyingDoubleTap();
                break;
            case PerkType.WeaponUpgrade:
                HandleBuyingWeaponUpgrade();
                break;
        }
    }

    public void HandleBuyingSpeedCola()
    {
        if (PerkManager.Instance.IsPerkBought(PerkType.SpeedCola)) return;

        if (GameManager.Instance.Points >= 1500)
        {
            GameManager.Instance.Points -= 1500;
            GameUIController.instance.RefreshUI();
            PlayerController.Instance.walkSpeed = 5.85f;
            PerkUIManager.Instance.AddPerkToUI("speedcola");
            PerkManager.Instance.BuyPerk(PerkType.SpeedCola);

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }

    public void HandleBuyingJuggernaut()
    {
        if (PerkManager.Instance.IsPerkBought(PerkType.Juggernog)) return;

        if (GameManager.Instance.Points >= 2500)
        {
            GameManager.Instance.Points -= 2500;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("juggernog");
            PlayerController.Instance.playerMaxHealth = 170f;
            PlayerController.Instance.playerCurrentHealth = 170f;
            PerkManager.Instance.BuyPerk(PerkType.Juggernog);

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }

    public void HandleBuyingDoubleTap()
    {
        if (PerkManager.Instance.IsPerkBought(PerkType.DoubleTap)) return;

        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameUIController.instance.RefreshUI();
            PerkUIManager.Instance.AddPerkToUI("doubletap");
            PerkManager.Instance.BuyPerk(PerkType.DoubleTap);

            // Herbereken upgrades voor alle wapens
            Weapon[] allWeapons = WeaponSwitching.instance.GetAllWeapons();
            foreach (Weapon weapon in allWeapons)
            {
                if (weapon != null)
                {
                    weapon.RefreshUpgrades();
                }
            }
            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }

    public void HandleBuyingWeaponUpgrade()
    {
        Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();
        if (currentWeapon.isWeaponUpgraded) return;

        if (GameManager.Instance.Points >= 5000)
        {
            GameManager.Instance.Points -= 5000;
            GameUIController.instance.RefreshUI();

            WeaponUpgradeManager.Instance.StartWeaponUpgrade(5f);
            weaponUpgradeSound.Play();

            // Markeer wapen als upgraded
            currentWeapon.isWeaponUpgraded = true;
            currentWeapon.damage *= 1.8f;

            // Herbereken alle upgrades
            currentWeapon.RefreshUpgrades();

            GameUIController.instance.DisableInteractionText();
            PlayerInteraction.Instance.ClearInteraction();
        }
    }
}