using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerupUIManager : MonoBehaviour
{
    public static PowerupUIManager Instance;

    /*private Dictionary<int, VisualElement> powerupIcons;*/
    private Dictionary<int, Sprite> powerupSprites;
    private Dictionary<int, Coroutine> activeTimers;
    private VisualElement rootElement;

    // Slot management
    private VisualElement[] powerupSlots = new VisualElement[2]; // PicukpSlot1 en PicukpSlot2
    private int[] slotPowerupIDs = new int[2] { -1, -1 }; // Track welke powerup in welke slot zit



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
        rootElement = uiDocument.rootVisualElement;

        // Initialize collections
        powerupSprites = new Dictionary<int, Sprite>(4);
        activeTimers = new Dictionary<int, Coroutine>(4);

        // Setup UI elements and sprites
        SetupPowerupElements();
        LoadPowerupSprites();
        HideAllPowerups();
    }

    private void SetupPowerupElements()
    {
        powerupSlots[0] = rootElement.Q<VisualElement>("PicukpSlot1");
        powerupSlots[1] = rootElement.Q<VisualElement>("PicukpSlot2");
    }

    private void LoadPowerupSprites()
    {   // Load sprites with fallback support
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
        foreach (var slot in powerupSlots)
        {
            if (slot != null)
            {
                slot.style.display = DisplayStyle.None;
                slot.style.visibility = Visibility.Visible;
            }
        }

        //Reset slot tracking
        for (int i = 0; i < powerupSlots.Length; i++)
        {
            slotPowerupIDs[i] = -1;
        }
    }

    public void ShowPowerup(int powerupID, float duration)
    {
        if (!IsValidPowerupRequest(powerupID, duration)) return;

        // Zoek eerste beschikbare slot
        int availableSlot = FindAvailableSlot();
        if (availableSlot == -1) return; // Geen slots beschikbaar

        var slot = powerupSlots[availableSlot];
        var sprite = powerupSprites[powerupID];

        // Stop any existing timer
        StopExistingTimer(powerupID);

        // Show the powerup
        slot.style.display = DisplayStyle.Flex;
        slot.style.backgroundImage = new StyleBackground(sprite);
        slot.style.visibility = Visibility.Visible;

        // Track welke powerup in welke slot zit
        slotPowerupIDs[availableSlot] = powerupID;

        // Start countdown timer
        activeTimers[powerupID] = StartCoroutine(PowerupTimer(powerupID, duration, slot, availableSlot));
    }

    private int FindAvailableSlot()
    {
        for (int i = 0;i < powerupSlots.Length; i++)
        {
            if (slotPowerupIDs[i] == -1) return i;
        }
        return -1;
    }

    private bool IsValidPowerupRequest(int powerupID, float duration)
    {
        return duration > 0f && powerupSprites.ContainsKey(powerupID);
    }

    private void StopExistingTimer(int powerupID)
    {
        if (activeTimers.TryGetValue(powerupID, out var timer) && timer != null)
        {
            StopCoroutine(timer);
            activeTimers.Remove(powerupID);
        }
    }

    // BLINK FUNCTIONALITEIT BLIJFT EXACT HETZELFDE!
    private IEnumerator PowerupTimer(int powerupID, float duration, VisualElement slot, int slotIndex)
    {
        float elapsed = 0f;
        float blinkStartTime = duration - BLINK_START_OFFSET;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Start blinking when approaching the end (HETZELFDE ALS VOORHEEN!)
            if (elapsed >= blinkStartTime)
            {
                bool shouldShow = (Time.time % BLINK_INTERVAL) < BLINK_ON_DURATION;
                slot.style.visibility = shouldShow ? Visibility.Visible : Visibility.Hidden;
            }

            yield return null;
        }

        // Hide powerup when timer expires
        HidePowerupInSlot(powerupID, slotIndex);
    }

    private void HidePowerupInSlot(int powerupID, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < powerupSlots.Length)
        {
            var slot = powerupSlots[slotIndex];
            slot.style.display = DisplayStyle.None;
            slot.style.visibility = Visibility.Visible;
            slotPowerupIDs[slotIndex] = -1; // Clear slot
        }

        StopExistingTimer(powerupID);
    }

    public void HidePowerup(int powerupID)
    {
        // Find which slot contains this powerup
        for (int i = 0; i < slotPowerupIDs.Length; i++)
        {
            if (slotPowerupIDs[i] == powerupID)
            {
                HidePowerupInSlot(powerupID, i);
                break;
            }
        }
    }

    public bool IsPowerupActive(int powerupID)
    {
        return activeTimers.ContainsKey(powerupID);
    }

    private void OnDestroy()
    {
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