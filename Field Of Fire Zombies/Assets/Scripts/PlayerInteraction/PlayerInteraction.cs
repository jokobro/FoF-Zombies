using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    [SerializeField] private string doorOpenAnimName;
    [SerializeField] private float playerInReach = 3f;
    private Interactable currentInteractable;
    private HashSet<GameObject> openedDoors = new HashSet<GameObject>();

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
    }

    private void CheckInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, playerInReach))
        {
            if (hit.collider.CompareTag("Door"))
            {
                ShowDoorInteraction(hit.collider);
                return;
            }


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
                    HUDcontroller.instance.EnableInteractionText("Press F to pick up weapon");
                }
                else if (!box.IsRolling)
                {
                    HUDcontroller.instance.EnableInteractionText(box.message);
                }
                else
                {
                    HUDcontroller.instance.DisableInteractionText();
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
             perkUpgrades.IsQuickReviveBought ||
             perkUpgrades.IsJunngernautPerkBought ||
             perkUpgrades.IsDoubleTapBought)
        {
            HUDcontroller.instance.DisableInteractionText(); // Verberg tekst als een perk is gekocht
            return true;
        }
        return false;
    }

    private void ShowDoorInteraction(Collider doorCollider)
    {
        GameObject doorParent = doorCollider.transform.parent?.parent?.gameObject; // Ga 2 niveaus omhoog naar DoorParent
        if (doorParent == null || openedDoors.Contains(doorParent)) return;

        HUDcontroller.instance.EnableInteractionText("Press E to open door (2000 points)");
        if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.Points >= 2000)
        {
            GameManager.Instance.Points -= 2000;
            GameManager.Instance.UpdatePointsUI();
            Animator doorAnim = doorParent.GetComponent<Animator>();
            AudioSource doorSound = doorParent.GetComponent<AudioSource>();
            doorSound.Play();
            if (doorAnim != null)
            {
                doorAnim.SetBool("OpenDoor", true);

            }
            openedDoors.Add(doorParent); // Voeg de geopende deur toe aan de lijst
            HUDcontroller.instance.DisableInteractionText(); // Verberg tekst na aankoop
            ClearInteraction(); // Zorg ervoor dat de interactietekst wordt bijgewerkt
        }
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

    public void ClearInteraction()
    {
        DisableCurrentInteractable();
    }
}
