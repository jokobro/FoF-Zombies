using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Return : MonoBehaviour
{
    private UIDocument UIDocument;
    private Button ReturnButton;

    private void Awake()
    {
        UIDocument = GetComponent<UIDocument>();  
        ReturnButton = UIDocument.rootVisualElement.Q("ReturnButton") as Button;
        ReturnButton.RegisterCallback<ClickEvent>(OnReturnButtonClicked);
    }

    private void OnDisable()
    {
        ReturnButton.UnregisterCallback<ClickEvent>(OnReturnButtonClicked);
    }

    private void OnReturnButtonClicked(ClickEvent clickEvent)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
