using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject HighscoreScreenPanel;
    [SerializeField] private GameObject pauseMenuUi;
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnMainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowCreditsPanel()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void ShowControlSPanel()
    {
        SceneManager.LoadScene("ControlsScreen");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EndGame()
    {

        Debug.Log("nggger");
    }

    public void Resume()
    {
        pauseMenuUi.SetActive(false);
    }

   /* IEnumerator EndTheGame()
    {
        yield return WaitForSeconds(5);
    }*/
}
