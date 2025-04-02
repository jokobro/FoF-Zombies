using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private string doorOpenAnimName;
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
                    BuyingUpgrades Upgrades = newInteractable.GetComponent<BuyingUpgrades>();

                    // Controleer of de perk al is gekocht
                    if (Upgrades != null && !PerkAlreadyBought(Upgrades))
                    {
                        SetNewCurrentInteractable(newInteractable);
                        return;
                    }
                }
            }
            else
            {
                DisableCurrentInteractable();
            }

            if (hit.collider.gameObject.tag == "Door")
            {
                GameObject doorParent = hit.collider.transform.root.gameObject;
                Animator doorAnim = doorParent.GetComponent<Animator>();
                AudioSource doorSound = hit.collider.gameObject.GetComponent<AudioSource>();

                if (GameManager.Instance.Points >= 2000 && Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.Instance.Points -= 2000;
                    GameManager.Instance.UpdatePointsUI();
                    doorAnim.SetBool("OpenDoor", true);
                    /*doorSound.clip = doorOpenSound;
                    doorSound.Play();*/
                }
            }
        }
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
