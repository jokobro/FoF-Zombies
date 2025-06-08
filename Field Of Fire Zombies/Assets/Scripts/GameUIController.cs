using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label ammoLabel;
    private Label scoreLabel;
    private Label waveLabel;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;

        ammoLabel = root.Q<Label>("ammoLabel");
        scoreLabel = root.Q<Label>("scoreLabel");
        waveLabel = root.Q<Label>("waveLabel");

        GameManager.OnPointsChanged += UpdateScoreText;
        waveManager.OnWaveChanged += UpdateWaveText;
    }

    private void OnDestroy()
    {
        GameManager.OnPointsChanged -= UpdateScoreText;
        waveManager.OnWaveChanged -= UpdateWaveText;
    }

    public void UpdateAmmoText(int magAmmo, int reserveAmmo)
    {
        if (ammoLabel != null)
            ammoLabel.text = $"{magAmmo}/{reserveAmmo}";
    }

    public void UpdateScoreText(int score)
    {
        if (scoreLabel != null)
            scoreLabel.text = $"Score: {score}";
    }

    public void UpdateWaveText(int wave)
    {
        if (waveLabel != null)
            waveLabel.text = $"{wave}";
    }
}
