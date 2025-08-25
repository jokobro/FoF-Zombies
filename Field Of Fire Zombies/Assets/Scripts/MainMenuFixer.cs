using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MainMenuFixer : MonoBehaviour
{
    private void Awake()
    {
        // Forceer Time.timeScale reset
        Time.timeScale = 1f;

        // Fix voor EventSystem/Input System in builds
        FixEventSystem();
    }

    private void FixEventSystem()
    {
        // Zoek bestaande EventSystem
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();

        if (eventSystem != null)
        {
            // Disable en enable InputSystemUIInputModule voor refresh
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule != null)
            {
                inputModule.enabled = false;
                inputModule.enabled = true;
            }
        }
        else
        {
            // Maak nieuw EventSystem als er geen is
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<InputSystemUIInputModule>();
        }
    }
}
