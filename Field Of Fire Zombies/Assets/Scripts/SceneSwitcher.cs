using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnMainmenu()
    {
        creditsPanel.SetActive(false);
    }

    public void ShowCreditsPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void Quit()
    {
        
        Application.Quit();
    }

    public void EndGame()
    {
        // nog inplementeren wanneer je game beidnigt
    }
}
