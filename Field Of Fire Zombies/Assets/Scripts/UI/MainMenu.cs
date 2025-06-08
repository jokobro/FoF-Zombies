using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenu : MonoBehaviour
{
    private UIDocument UIDocument;
    private Dictionary<Button, EventCallback<ClickEvent>> registeredCallbacks = new();

    private void Awake()
    {
        UIDocument = GetComponent<UIDocument>();
        RegisterButton("StartButton", OnPlayGameClickEvent);
        RegisterButton("CreditsButton", e => LoadScene("CreditsScene"));
        RegisterButton("ControlsButton", e => LoadScene("ControlsScene"));
        RegisterButton("QuitButton", OnQuitGameClickEvent);
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

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnPlayGameClickEvent(ClickEvent evt)
    {
        Debug.Log("Game start button pressed");
        // SceneManager.LoadScene("GameScene"); // Uncomment as needed
    }

    private void OnQuitGameClickEvent(ClickEvent evt)
    {
        Application.Quit();
    }
}
