using UnityEngine;
using UnityEngine.UIElements;

public class PerkUIManager : MonoBehaviour
{
    public static PerkUIManager Instance;
    private VisualElement[] perkSlots;
    private int currentSlotIndex = 0;

    private void Awake()
    {
        Instance = this;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Initialiseer de perk slots als VisualElements
        perkSlots = new VisualElement[4];
        perkSlots[0] = root.Q<VisualElement>("PerkSlot1");
        perkSlots[1] = root.Q<VisualElement>("PerkSlot2");
        perkSlots[2] = root.Q<VisualElement>("PerkSlot3");
    }

    public void AddPerkToUI(string perkName)
    {
        if (currentSlotIndex < perkSlots.Length)
        {
            Texture2D texture = Resources.Load<Texture2D>($"PerkIcons/{perkName}");
            if (texture != null)
            {
                // Zet de achtergrondafbeelding van het VisualElement
                perkSlots[currentSlotIndex].style.backgroundImage = new StyleBackground(texture);
                currentSlotIndex++;
            }
        }
    }

    public void ClearAllPerks()
    {
        for (int i = 0; i < perkSlots.Length; i++)
        {
            perkSlots[i].style.backgroundImage = null;
        }
        currentSlotIndex = 0;
    }
}
