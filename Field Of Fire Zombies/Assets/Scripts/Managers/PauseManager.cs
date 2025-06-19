using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    public static PauseManager instance;
    private VisualElement pauseScreen;
    private VisualElement endgameScreen;
    private VisualElement hud;
    private Button resumeButton;
    private Button endGameButton;
    private Label roundReachedLabel;
    private bool isPaused = false;

    private InputActionMap gameActionMap;
    private InputActionMap uiActionMap;
    private InputAction pauseAction;

    private void Awake()
    {
        instance = this;
        gameActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");
        gameActionMap.Enable();
        uiActionMap.Disable();
        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");
        pauseAction.performed += ctx => TogglePause();
        pauseAction.Enable();
    }

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        pauseScreen = root.Q<VisualElement>("PauseScreen");
        endgameScreen = root.Q<VisualElement>("GameOverScreen");
        hud = root.Q<VisualElement>("HUDContainer");
        resumeButton = root.Q<Button>("ResumeButton");
        endGameButton = root.Q<Button>("EndGameButton");
        endGameButton.RegisterCallback<ClickEvent>(EndGame);
        pauseScreen.style.display = DisplayStyle.None;
        endgameScreen.style.display = DisplayStyle.None;
        roundReachedLabel = root.Q<Label>("RoundReachedText");

        resumeButton.clicked += () =>
        {
            ResumeGame();
        };
    }

    private void TogglePause()
    {
        if (!isPaused)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameActionMap.Disable();
        uiActionMap.Enable();
        pauseScreen.style.display = DisplayStyle.Flex;
        hud.style.display = DisplayStyle.None;
        isPaused = true;
    }

    public void ResumeGame()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Time.timeScale = 1f;
        gameActionMap.Enable();
        uiActionMap.Disable();
        pauseScreen.style.display = DisplayStyle.None;
        hud.style.display = DisplayStyle.Flex;
        isPaused = false;
    }

    private void EndGame(ClickEvent clickEvent)
    {
        HandleEndingTheGame();
    }

    public void HandleEndingTheGame()
    {
        StartCoroutine(EndGameCycle());
        endgameScreen.style.display = DisplayStyle.Flex;
        pauseScreen.style.display = DisplayStyle.None;
        hud.style.display = DisplayStyle.None;

        if (roundReachedLabel != null && waveManager.Instance != null)
        {
            int round = waveManager.Instance != null ? waveManager.Instance.CurrentWave : 0;   
            roundReachedLabel.text = $"Round Reached {round}";
        }
    }

    private IEnumerator EndGameCycle()
    {
        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadScene("MainMenu");
    }
}
