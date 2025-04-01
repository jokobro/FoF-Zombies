using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject pauseMenuUi;
    [SerializeField] private GameObject uiPanel;
    private InputActionMap gameActionMap;
    private InputActionMap uiActionMap;

    private void Start()
    {
        gameActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");
        gameActionMap.Enable();
        uiActionMap.Disable();
    }

    public void HandlePausing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Time.timeScale = 0f;
            uiPanel.SetActive(false);
            pauseMenuUi.SetActive(true);
            gameActionMap.Disable();
            uiActionMap.Enable();
        }
    }

    private void Resume()
    {
        uiPanel.SetActive(true);
        pauseMenuUi.SetActive(false);
        gameActionMap.Enable();
        uiActionMap.Disable();
        Time.timeScale = 1f;
    }

    public void OnResumeButtonPressed()
    {
        Resume();
    }

    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
        //impelenemteren wanneer je doodgaat dat de player score bord te zien krijgt en daarna naar main menu gaat
    }
}
