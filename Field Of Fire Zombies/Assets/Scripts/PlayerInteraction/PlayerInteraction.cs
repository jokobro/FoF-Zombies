using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    private Interactable currentInteractable;
    private float playerInReach = 3f;
    [SerializeField] private float sphereRadius = 0.4f; // Radius voor SphereCast detectie
    private PlayerControls.PlayerControls controls;

    private void Awake()
    {
        Instance = this;
        controls = new PlayerControls.PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteract;
        controls.Player.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.HandleInteraction();
        }
    }

    private void Update()
    {
        CheckInteraction();
    }

    private void CheckInteraction()
    {
        RaycastHit hit;

        // Gebruik SphereCast voor betere detectie - je hoeft niet precies te mikken
        if (Physics.SphereCast(Camera.main.transform.position, sphereRadius, Camera.main.transform.forward, out hit, playerInReach))
        {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();

            if (newInteractable == null)
            {
                DisableCurrentInteractable();
                return;
            }

            // Mysterybox
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

            // Perk machine
            BuyingUpgrades upgrade = newInteractable.GetComponent<BuyingUpgrades>();
            if (upgrade != null)
            {
                if (PerkAlreadyBought(upgrade))
                {
                    DisableCurrentInteractable();
                    return;
                }

                // Check voor weapon upgrade
                if (upgrade.perkType == BuyingUpgrades.PerkType.WeaponUpgrade)
                {
                    Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();
                    if (currentWeapon != null && currentWeapon.isWeaponUpgraded)
                    {
                        DisableCurrentInteractable();
                        return;
                    }
                }

                currentInteractable = newInteractable;
                GameUIController.instance.EnableInteractionText(currentInteractable.message);
                return;
            }

            // Algemene interactable
            currentInteractable = newInteractable;
            GameUIController.instance.EnableInteractionText(currentInteractable.message);
            return;
        }

        DisableCurrentInteractable();
    }

    private bool PerkAlreadyBought(BuyingUpgrades upgrade)
    {
        return upgrade != null && PerkManager.Instance.IsPerkBought(upgrade.perkType);
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
