using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenu : MonoBehaviour
{
    private UIDocument UIDocument;
    private VisualElement[] overlayPanels;

    private Dictionary<Button, EventCallback<ClickEvent>> registeredCallbacks = new();

    private void Awake()
    {
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        // Auto-discover alle overlay panels
        overlayPanels = new VisualElement[]
        {
            root.Q<VisualElement>("Credits"),
            root.Q<VisualElement>("Controls")
        };

        HideAllPanels();
        SetupButtons();
    }

    private void SetupButtons()
    {
        // Game flow buttons
        RegisterButton("StartButton", _ => SceneManager.LoadScene("GameScene"));
        RegisterButton("QuitButton", _ => Application.Quit());

        // Panel buttons
        RegisterButton("CreditsButton", _ => ShowPanel("Credits"));
        RegisterButton("ControlsButton", _ => ShowPanel("Controls"));

        // Return buttons - werken automatisch voor alle panels!
        RegisterButton("CreditsReturnButton", _ => HideAllPanels());
        RegisterButton("ControlsReturnButton", _ => HideAllPanels());
    }

    private void ShowPanel(string panelName)
    {
        HideAllPanels();
        var panel = UIDocument.rootVisualElement.Q<VisualElement>(panelName);
        if (panel != null)
            panel.style.visibility = Visibility.Visible;
    }

    private void HideAllPanels()
    {
        // Automatisch alle overlay panels verbergen - super scalable!
        foreach (var panel in overlayPanels)
        {
            if (panel != null)
                panel.style.visibility = Visibility.Hidden;
        }
    }

    private void RegisterButton(string name, EventCallback<ClickEvent> callback)
    {
        var button = UIDocument.rootVisualElement.Q<Button>(name);
        if (button != null)
        {
            button.RegisterCallback(callback);
            registeredCallbacks[button] = callback;
        }
    }

    private void OnDisable()
    {
        foreach (var pair in registeredCallbacks)
        {
            pair.Key.UnregisterCallback(pair.Value);
        }
        registeredCallbacks.Clear();
    }
}
