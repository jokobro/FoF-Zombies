using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeUIManager : MonoBehaviour
{
    private VisualElement[] grenadeIcons;
    private Sprite grenadeSprite;
    private const int maxGrenades = 3;

    private void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        grenadeIcons = new VisualElement[maxGrenades];
        grenadeIcons[0] = root.Q<VisualElement>("GrenadeIcon1");
        grenadeIcons[1] = root.Q<VisualElement>("GrenadeIcon2");
        grenadeIcons[2] = root.Q<VisualElement>("GrenadeIcon3");

        grenadeSprite = Resources.Load<Sprite>("grenadeicons/grenade");
        if (grenadeSprite == null)
        {
            Debug.LogError("Grenade sprite not found at Resources/grenadeicons/grenade.png");
            return;
        }

        ApplyIcons();
        UpdateGrenadeUI(maxGrenades); // Begin met alle icons actief
    }

    private void ApplyIcons()
    {
        foreach (var icon in grenadeIcons)
        {
            icon.style.backgroundImage = new StyleBackground(grenadeSprite);
        }
    }

    public void UpdateGrenadeUI(int currentGrenades)
    {
        for (int i = 0; i < grenadeIcons.Length; i++)
        {
            grenadeIcons[i].style.display = (i < currentGrenades) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
