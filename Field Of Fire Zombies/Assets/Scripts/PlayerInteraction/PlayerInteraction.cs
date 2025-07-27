using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    private Interactable currentInteractable;
    public Vector3 relativeDirection = new Vector3(0, 0, 1);
    private float playerInReach = 3f;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        CheckInteraction();
        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
        {
            currentInteractable.HandleInteraction();
        }

        Vector3 debugDirection = Camera.main.transform.TransformDirection(relativeDirection.normalized);
        Debug.DrawRay(Camera.main.transform.position, debugDirection * playerInReach, Color.red);
    }

    private void CheckInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, playerInReach))
        {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();
            if (newInteractable == null)
            {
                DisableCurrentInteractable();
                return;
            }

            Mysterybox box = newInteractable.GetComponent<Mysterybox>();
            if (box != null)
            {
                currentInteractable = newInteractable;

                if (box.CanTakeItem)
                {
                    GameUIController.instance.EnableInteractionText("Press F to pick up weapon");
                }
                else if (!box.IsRolling)
                {
                    GameUIController.instance.EnableInteractionText(box.message);
                }
                else
                {
                    GameUIController.instance.DisableInteractionText();
                }
                return;
            }

            BuyingUpgrades upgrades = newInteractable.GetComponent<BuyingUpgrades>();
            if (upgrades != null && !PerkAlreadyBought(upgrades))
            {
                Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();
                if (currentWeapon != null && currentWeapon.isWeaponUpgraded) return;
                SetNewCurrentInteractable(newInteractable);
                return;
            }
            SetNewCurrentInteractable(newInteractable);
            return;
        }

        DisableCurrentInteractable();
    }

    private bool PerkAlreadyBought(BuyingUpgrades perkUpgrades)
    {
        if (perkUpgrades.IsSpeedColaBought ||
             perkUpgrades.IsJunngernautPerkBought ||
             perkUpgrades.IsDoubleTapBought)
        {
            GameUIController.instance.DisableInteractionText(); // Verberg tekst als een perk is gekocht
            return true;
        }
        return false;
    }

    private void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
       GameUIController.instance.EnableInteractionText(currentInteractable.message);
    }

    private void DisableCurrentInteractable()
    {
        GameUIController.instance.DisableInteractionText();
        currentInteractable = null;
    }

    public void ClearInteraction()
    {
        DisableCurrentInteractable();
    }
}
