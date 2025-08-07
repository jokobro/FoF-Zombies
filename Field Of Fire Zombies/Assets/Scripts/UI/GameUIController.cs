using UnityEngine;
using UnityEngine.UIElements;
public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    public UIDocument uiDocument;
    private Label interactionLabel;
    private Label scoreLabel;
    private Label ammoLabel;
    
    [Header("healthUI")]
    public VisualElement bloodSplatter1;
    public VisualElement bloodSplatter2;

    private void Awake()
    {
        instance = this;
        var root = uiDocument.rootVisualElement;

        ammoLabel = root.Q<Label>("ammoLabel");
        scoreLabel = root.Q<Label>("scoreLabel");
        
        interactionLabel = root.Q<Label>("InteractionText");

        bloodSplatter1 = root.Q<VisualElement>("BloodSplatter1");
        bloodSplatter2 = root.Q<VisualElement>("BloodSplatter2");
        bloodSplatter1.style.visibility = Visibility.Hidden;
        bloodSplatter2.style.visibility = Visibility.Hidden;

        GameManager.OnPointsChanged += UpdateScoreText;

        if (GameManager.Instance != null)
        {
            UpdateScoreText(GameManager.Instance.Points);
        }

        if (interactionLabel != null)
        {
            interactionLabel.visible = false;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnPointsChanged -= UpdateScoreText;
    }

    public void UpdateAmmoText(int magAmmo, int reserveAmmo)
    {
        if (ammoLabel != null)
        {
            ammoLabel.text = $"{magAmmo}/{reserveAmmo}";
        }
    }

    public void UpdateScoreText(int score)
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = $"{score}";
        }
    }

    public void RefreshUI()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = GameManager.Instance.Points.ToString();
        }
    }

    public void EnableInteractionText(string text)
    {
        if (interactionLabel != null)
        {
            interactionLabel.text = text;
            interactionLabel.visible = true;
        }
    }

    public void DisableInteractionText()
    {
        if (interactionLabel != null)
        {
            interactionLabel.visible = false;
        }
    }
}
