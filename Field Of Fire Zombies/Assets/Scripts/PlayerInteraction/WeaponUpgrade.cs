using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    private Weapon weapon;
    private WeaponSwitching weaponSwitching;

    private bool isWeaponUpgradeBought = false;
    public bool IsWeaponUpgradeBought => isWeaponUpgradeBought;


    private void Start()
    {
        weaponSwitching = FindAnyObjectByType<WeaponSwitching>(); // Zoek WeaponSwitching
        weapon = FindAnyObjectByType<Weapon>();
    }

    

    public void HandleBuyingWeaponUpgrade()
    {
        if (GameManager.Instance.Points >= 5000)
        {
            GameManager.Instance.Points -= 5000;
            GameManager.Instance.UpdatePointsUI();
            weapon.fireRate = 0.150f;
            weapon.damage = 500;
            HUDcontroller.instance.DisableInteractionText();
            isWeaponUpgradeBought = true;
            // nog toevoegen van het alleen kunnen kopen voor 1 wapen en dat allebij de wapens in invontory geupgrade kunnen worden
        }
    }
}
