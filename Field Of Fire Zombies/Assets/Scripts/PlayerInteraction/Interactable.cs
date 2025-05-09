using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string message;
    public UnityEvent onInteraction;

    public virtual void HandleInteraction()
    {
        onInteraction.Invoke();
    }
}
