using UnityEngine;
using UnityEngine.UI;

public class PerkUIManager : MonoBehaviour
{
    [Header("Perk UI Slots")]
    public Image[] perkSlots; // UI-slots voor perks (vier slots in je Canvas)

    [Header("Perk Sprites")]
    public Sprite speedColaSprite;
    public Sprite juggernautSprite;
    public Sprite doubleTapSprite;
    public Sprite quickReviveSprite;

    public static PerkUIManager Instance;
    private int currentSlotIndex = 0; // Houd bij welke slot gevuld wordt

    private void Awake()
    {
        Instance = this;
    }

    public void AddPerkToUI(Sprite perkSprite)
    {
        if (currentSlotIndex < perkSlots.Length)
        {
            perkSlots[currentSlotIndex].sprite = perkSprite; // Zet de afbeelding in de juiste slot
            perkSlots[currentSlotIndex].enabled = true; // Zorg dat de afbeelding zichtbaar is
            currentSlotIndex++;
        }
    }
}
