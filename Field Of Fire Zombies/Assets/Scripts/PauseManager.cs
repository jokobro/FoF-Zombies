using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUi;
    [SerializeField] private InputActionAsset inputActions;
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
            pauseMenuUi.SetActive(true);
            gameActionMap.Disable();
            uiActionMap.Enable();
        }
    }

    private void Resume()
    {
        pauseMenuUi.SetActive(false);
        gameActionMap.Enable();
        uiActionMap.Disable();
        Time.timeScale = 1f;
    }

    public void OnResumeButtonPressed()
    {
        Resume();
    }
}
