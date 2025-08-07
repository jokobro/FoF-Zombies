using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerupUIManager : MonoBehaviour
{
    public static PowerupUIManager Instance;

    private Dictionary<int, VisualElement> powerupIcons;
    private Dictionary<int, Sprite> powerupSprites;
    private Dictionary<int, Coroutine> activeTimers;
    private VisualElement rootElement;

    // Cached constants for better performance
    private const float BLINK_INTERVAL = 0.25f;
    private const float BLINK_ON_DURATION = 0.125f;
    private const float BLINK_START_OFFSET = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePowerupUI();
    }

    private void InitializePowerupUI()
    {
        var uiDocument = GetComponent<UIDocument>();
        /*if (uiDocument?.rootVisualElement == null)
        {
            Debug.LogError("PowerupUIManager: UIDocument or root element not found!");
            return;
        }*/

        rootElement = uiDocument.rootVisualElement;

        // Initialize collections with fixed capacity for better memory management
        powerupIcons = new Dictionary<int, VisualElement>(4);
        powerupSprites = new Dictionary<int, Sprite>(4);
        activeTimers = new Dictionary<int, Coroutine>(4);

        // Setup UI elements and sprites
        SetupPowerupElements();
        LoadPowerupSprites();
        HideAllPowerups();
    }

    private void SetupPowerupElements()
    {
        powerupIcons[0] = FindPowerupIcon("DoublePoints");
        powerupIcons[3] = FindPowerupIcon("Instantkill");
    }

    private VisualElement FindPowerupIcon(string elementName)
    {
        return rootElement.Q<VisualElement>(elementName) ?? rootElement.Q<Image>(elementName);
    }

    private void LoadPowerupSprites()
    {
        // Load sprites with fallback support
        powerupSprites[0] = LoadSpriteWithFallback("pickupicons/doublepoints", "pickupicons/DoublePoints");
        powerupSprites[3] = LoadSpriteWithFallback("pickupicons/instantKill", "pickupicons/InstantKill", "pickupicons/instakill");
    }

    private Sprite LoadSpriteWithFallback(params string[] paths)
    {
        foreach (string path in paths)
        {
            var sprite = Resources.Load<Sprite>(path);
            if (sprite != null) return sprite;

            // Fallback: try loading as Texture2D and convert to Sprite
            var texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                return Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            }
        }
        return null;
    }

    private void HideAllPowerups()
    {
        foreach (var icon in powerupIcons.Values)
        {
            if (icon != null)
            {
                icon.style.display = DisplayStyle.None;
                icon.style.visibility = Visibility.Visible;
            }
        }
    }

    public void ShowPowerup(int powerupID, float duration)
    {
        if (!IsValidPowerupRequest(powerupID, duration)) return;

        var icon = powerupIcons[powerupID];
        var sprite = powerupSprites[powerupID];

        // Stop any existing timer
        StopExistingTimer(powerupID);

        // Show the powerup
        icon.style.display = DisplayStyle.Flex;
        icon.style.backgroundImage = new StyleBackground(sprite);
        icon.style.visibility = Visibility.Visible;

        // Start countdown timer
        activeTimers[powerupID] = StartCoroutine(PowerupTimer(powerupID, duration, icon));
    }

    private bool IsValidPowerupRequest(int powerupID, float duration)
    {
        return duration > 0f &&
               powerupIcons.TryGetValue(powerupID, out var icon) && icon != null &&
               powerupSprites.TryGetValue(powerupID, out var sprite) && sprite != null;
    }

    private void StopExistingTimer(int powerupID)
    {
        if (activeTimers.TryGetValue(powerupID, out var timer) && timer != null)
        {
            StopCoroutine(timer);
            activeTimers.Remove(powerupID);
        }
    }

    private IEnumerator PowerupTimer(int powerupID, float duration, VisualElement icon)
    {
        float elapsed = 0f;
        float blinkStartTime = duration - BLINK_START_OFFSET;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Start blinking when approaching the end
            if (elapsed >= blinkStartTime)
            {
                bool shouldShow = (Time.time % BLINK_INTERVAL) < BLINK_ON_DURATION;
                icon.style.visibility = shouldShow ? Visibility.Visible : Visibility.Hidden;
            }

            yield return null;
        }

        // Hide powerup when timer expires
        HidePowerup(powerupID);
    }

    public void HidePowerup(int powerupID)
    {
        if (!powerupIcons.TryGetValue(powerupID, out var icon) || icon == null) return;

        StopExistingTimer(powerupID);
        icon.style.display = DisplayStyle.None;
        icon.style.visibility = Visibility.Visible;
    }

    public bool IsPowerupActive(int powerupID)
    {
        return activeTimers.ContainsKey(powerupID);
    }

    private void OnDestroy()
    {
        // Cleanup all running coroutines
        if (activeTimers != null)
        {
            foreach (var timer in activeTimers.Values)
            {
                if (timer != null) StopCoroutine(timer);
            }
            activeTimers.Clear();
        }

        if (Instance == this) Instance = null;
    }
}