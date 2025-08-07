using UnityEngine;
using UnityEngine.UIElements;

public class RoundDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    private VisualElement roundNumberElement;
    private UIDocument uiDocument;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        InitializeUI();
        waveManager.OnWaveChanged += OnRoundChanged;
        HideRoundNumber();
    }

    private void OnDestroy()
    {
        waveManager.OnWaveChanged -= OnRoundChanged;
    }

    private void InitializeUI()
    {
        var root = uiDocument.rootVisualElement;
        roundNumberElement = root.Q<VisualElement>("RoundNumber");

        /*if (roundNumberContainer == null)
        {
            Debug.LogError("RoundNumber VisualElement niet gevonden in UI! Voeg deze toe aan je UXML.");
        }*/
    }

    private void OnRoundChanged(int newRound)
    {
        // ALLEEN round 0 verbergen, alle andere rounds tonen
        if (newRound <= 0)
        {
            // Round 0 of lager = verberg
            HideRoundNumber();
        }
        else
        {
            // Round 1 en hoger = toon
            DisplayRoundNumber(newRound);
        }
    }

    private void HideRoundNumber()
    {
        if (roundNumberElement != null)
        {
            roundNumberElement.style.backgroundImage = StyleKeyword.None;
        }
    }

    private void DisplayRoundNumber(int roundNumber)
    {
        if (roundNumberElement == null) return;

        // Laad de juiste sprite voor deze round
        Sprite roundSprite = LoadRoundSprite(roundNumber);

        if (roundSprite != null)
        {
            roundNumberElement.style.backgroundImage = new StyleBackground(roundSprite);
        }
        else
        {
            HideRoundNumber();
        }
    }

    private Sprite LoadRoundSprite(int roundNumber)
    {
        if (roundNumber <= 0) return null;

        if (roundNumber >= 1 && roundNumber <= 10)
        {
            return LoadFromSpriteStrip("RoundNumber/RoundNumber", roundNumber, 1);
        }
        else if (roundNumber >= 11 && roundNumber <= 19)
        {
            return LoadFromSpriteStrip("RoundNumber/roundnubmer_edited_10_19-removebg-preview (1)", roundNumber, 11);
        }
        else if (roundNumber >= 20 && roundNumber <= 30)
        {
            return LoadFromSpriteStrip("RoundNumber/20_tot_30-removebg-preview", roundNumber, 20);
        }

        return null;
    }

    private Sprite LoadFromSpriteStrip(string spritePath, int roundNumber, int startRound)
    {
        Sprite[] spriteStrip = Resources.LoadAll<Sprite>(spritePath);

        if (spriteStrip != null && spriteStrip.Length > 0)
        {
            int spriteIndex;
            if (startRound == 1)
            {
                spriteIndex = roundNumber - 1;
            }
            else if (startRound == 11)
            {
                spriteIndex = roundNumber - 11;
            }
            else
            {
                spriteIndex = roundNumber - startRound;
            }

            if (spriteIndex >= 0 && spriteIndex < spriteStrip.Length)
            {
                return spriteStrip[spriteIndex];
            }
        }

        return null;
    }
}
