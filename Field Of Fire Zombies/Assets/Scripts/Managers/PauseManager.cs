using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject HighscoreScreenPanel;
    [SerializeField] private TextMeshProUGUI roundReachedText;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject pauseMenuUi;
    /*[SerializeField] private GameObject uiPanel;*/
    [SerializeField] private GameObject showStatsPanel;
    /*private InputActionMap gameActionMap;
    private InputActionMap uiActionMap;*/
    private bool isEnding = false;

    private void Awake()
    {
        Instance = this;
    }

    /*private void Start()
    {
        gameActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");
        gameActionMap.Enable();
        uiActionMap.Disable();
    }*/

    /*public void HandlePausing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Time.timeScale = 0f;
           *//* uiPanel.SetActive(false);*//*
            pauseMenuUi.SetActive(true);
           *//* gameActionMap.Disable();
            uiActionMap.Enable();*//*
        }
    }*/

    private void Resume()
    {
        /*uiPanel.SetActive(true);*/
        pauseMenuUi.SetActive(false);
        /*gameActionMap.Enable();
        uiActionMap.Disable();*/
        Time.timeScale = 1f;
    }

    public void OnResumeButtonPressed()
    {
        Resume();
        Debug.Log("resume button");
    }

    public void HandleOpeningHighScore(InputAction.CallbackContext context)
    {
        if (isEnding) return;

        if (context.performed)
        {
            HighscoreScreenPanel.SetActive(true);
           /* uiPanel.SetActive(false);*/
        }

        if (context.canceled)
        {
            HighscoreScreenPanel.SetActive(false);
            /*uiPanel.SetActive(true);*/
        }
    }

   
    public void EndGame()
    {
        isEnding = true; // Zet dit ALVORENS iets zichtbaar wordt
        
        Time.timeScale = 1f;
        StartCoroutine(HandleEndGameSequence());
    }

    private IEnumerator HandleEndGameSequence()
    {
       /* uiPanel.SetActive(false);*/
        int round = waveManager.Instance != null ? waveManager.Instance.roundNumber : 0;
        roundReachedText.text = $"You reached Round {round}.";
        GameOverPanel.SetActive(true);
        yield return new WaitForSeconds(4f);
        showStatsPanel.SetActive(true);
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("MainMenu");
    }
}
