using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float playerInReach = 3f;
    private Interactable currentInteractable;

    private void Update()
    {
        CheckInteraction();
        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
        {
            currentInteractable.HandleInteraction();
        }
    }

    private void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //checkt of de collider in reach of de player
        if (Physics.Raycast(ray, out hit, playerInReach))
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (newInteractable != null && newInteractable.enabled)
                {
                    PerkUpgrades perkUpgrades = newInteractable.GetComponent<PerkUpgrades>();
                    WeaponUpgrade weaponUpgrade = newInteractable.GetComponent<WeaponUpgrade>();

                    // Controleer of de perk al is gekocht
                    if (perkUpgrades != null && !PerkAlreadyBought(perkUpgrades,weaponUpgrade))
                    {
                        SetNewCurrentInteractable(newInteractable);
                        return;
                    }
                    if(weaponUpgrade != null && !PerkAlreadyBought(perkUpgrades,weaponUpgrade))
                    {
                        SetNewCurrentInteractable(newInteractable);
                        return;
                    }
                }
            }
        }
        else
        {
            DisableCurrentInteractable();
        }
    }

    private bool PerkAlreadyBought(PerkUpgrades perkUpgrades, WeaponUpgrade weaponUpgrade)
    {
        if (perkUpgrades.IsSpeedColaBought ||
             perkUpgrades.IsQuickReviveBought ||
             perkUpgrades.IsJunngernautPerkBought ||
             perkUpgrades.IsDoubleTapBought||

             weaponUpgrade.IsWeaponUpgradeBought)
        {
            HUDcontroller.instance.DisableInteractionText(); // Verberg tekst als een perk is gekocht
            return true;
        }
        return false;
    }

    private void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        HUDcontroller.instance.EnableInteractionText(currentInteractable.message);
    }

    private void DisableCurrentInteractable()
    {
        HUDcontroller.instance.DisableInteractionText();
        currentInteractable = null;
    }
}
