using UnityEngine;
public class BuyingUpgrades : MonoBehaviour
{
    private Weapon weapon;
    private WeaponSwitching weaponSwitching;

   

    private bool isSpeedColaBought = false;
    private bool isJugernautPerkBought = false;
    private bool isDoubleTapBought = false;
    private bool isQuickReviveBought = false;
    private bool hasUsedQuickRevive = false;

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

    public void HandleBuyingDoubleTap()
    {
        if (GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();
            PerkUIManager.Instance.AddPerkToUI(PerkUIManager.Instance.doubleTapSprite); // Voeg toe aan UI
            isDoubleTapBought = true;

            Weapon currentWeapon = weaponSwitching.GetActiveWeapon();

            if (currentWeapon != null)
            {
                weapon.fireRate = 0.1f; // tweaken met procenten
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
            currentWeapon.damage = 500;
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
