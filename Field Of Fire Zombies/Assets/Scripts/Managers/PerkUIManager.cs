using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkUIManager : MonoBehaviour
{
    public static PerkUIManager Instance;

    [Header("Perk UI Slots")]
    public Image[] perkSlots; // UI-slots voor perks (vier slots in je Canvas)

    [Header("Perk Sprites")]
    public Sprite speedColaSprite;
    public Sprite juggernautSprite;
    public Sprite doubleTapSprite;
    public Sprite quickReviveSprite;

    private int currentSlotIndex = 0; // Houd bij welke slot gevuld wordt

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
