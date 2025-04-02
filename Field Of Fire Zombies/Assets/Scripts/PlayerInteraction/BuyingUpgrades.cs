using UnityEngine;
public class BuyingUpgrades : MonoBehaviour
{
    private WeaponSwitching weaponSwitching;
    private Weapon weapon;

    private bool isSpeedColaBought = false;
    private bool isJugernautPerkBought = false;
    private bool isQuickReviveBought = false;
    public bool hasUsedQuickRevive = false;
    public bool isDoubleTapBought = false;

    public bool IsSpeedColaBought => isSpeedColaBought;
    public bool IsQuickReviveBought => isQuickReviveBought && !hasUsedQuickRevive;
    public bool IsJunngernautPerkBought => isJugernautPerkBought;
    public bool IsDoubleTapBought => isDoubleTapBought;

    private void Start()
    {
        weaponSwitching = FindAnyObjectByType<WeaponSwitching>();
        weapon = FindAnyObjectByType<Weapon>();
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

    public void HandleBuyingDoubleTap()// nog toevegen van reduction in firerate werkt nog niet helemaal
    {
        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.doubleTapSprite); // Voeg toe aan UI
            
            isDoubleTapBought = true;

            // Haal alle wapens op uit de weaponSwitching
            Weapon[] allWeapons = weaponSwitching.GetAllWeapons();

            // Loop door alle wapens en pas de vuursnelheid aan
            foreach (Weapon currentWeapon in allWeapons)
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
        if (GameManager.Instance.Points >= 5000)
        {
            GameManager.Instance.Points -= 5000;
            GameManager.Instance.UpdatePointsUI();
            Weapon currentWeapon = weaponSwitching.GetActiveWeapon();
            currentWeapon.fireRate = 0.150f;
            currentWeapon.damage *= 1.8f;
            currentWeapon.reloadTime *= 0.4f; 
            
            currentWeapon.isWeaponUpgraded = true;
            
            /*HUDcontroller.instance.DisableInteractionText();*/
            

            // nog toevoegen van het alleen kunnen kopen voor 1 wapen en dat allebij de wapens in invontory geupgrade kunnen worden
        }
    }


   

    /*public void HandleOpeningDoor()
    {
        GameObject doorParent = hit.collider.transform.root.gameObject;
        Animator doorAnim = doorParent.GetComponent<Animator>();
        AudioSource doorSound = hit.collider.gameObject.GetComponent<AudioSource>();

        if (GameManager.Instance.Points >= 2000 && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();
            doorAnim.SetBool("OpenDoor", true);
            *//*doorSound.clip = doorOpenSound;
            doorSound.Play();*//*
        }

    }*/

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
