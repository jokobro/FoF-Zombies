using UnityEngine;

/*public class WeaponUpgrade : MonoBehaviour
{
    private Weapon weapon;
    private WeaponSwitching weaponSwitching;

    
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
            Weapon currentWeapon = weaponSwitching.GetActiveWeapon();

            currentWeapon.isWeaponUpgraded = true;
            // nog toevoegen van het alleen kunnen kopen voor 1 wapen en dat allebij de wapens in invontory geupgrade kunnen worden
        }
    }
}
*/