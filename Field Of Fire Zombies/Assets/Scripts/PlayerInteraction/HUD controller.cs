using TMPro;
using UnityEngine;

public class HUDcontroller : MonoBehaviour
{
    public static HUDcontroller instance;
    [SerializeField] TMP_Text interactionText;

    private void Awake()
    {
        instance = this;
    }

    public void EnableInteractionText(string text)
    {
        interactionText.text = text;
        interactionText.gameObject.SetActive(true);
    }

    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }
}
