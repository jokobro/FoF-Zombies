using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenu : MonoBehaviour
{
    private UIDocument UIDocument;
    private Button startButton;
    private Button creditsButton;
    private Button controlsButton;
    private Button quitButton;

    private void Awake()
    {
        UIDocument = GetComponent<UIDocument>();

        startButton = UIDocument.rootVisualElement.Q("StartButton") as Button;
        startButton.RegisterCallback<ClickEvent>(OnPlayGameClickEvent);

        creditsButton = UIDocument.rootVisualElement.Q("CreditsButton") as Button;
        creditsButton.RegisterCallback<ClickEvent>(OpenCreditsPanel);

        controlsButton = UIDocument.rootVisualElement.Q("ControlsButton") as Button;
        //controlsButton.RegisterCallback<ClickEvent>

        quitButton = UIDocument.rootVisualElement.Q("QuitButton") as Button;
        quitButton.RegisterCallback<ClickEvent>(OnQuitGameClickEvent);
    }

    

    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(OnPlayGameClickEvent);
        creditsButton.UnregisterCallback<ClickEvent>(OpenCreditsPanel);
    }
    
    private void OpenCreditsPanel(ClickEvent clickEvent)
    {
        SceneManager.LoadScene("CreditsScene");
    }
    private void OnPlayGameClickEvent(ClickEvent clickEvent)
    {
        Debug.Log("button pressed to start the game");
    }

    private void OnQuitGameClickEvent(ClickEvent clickEvent)
    {
        Debug.Log("Quit button pressed");
        Application.Quit();
    }
}
