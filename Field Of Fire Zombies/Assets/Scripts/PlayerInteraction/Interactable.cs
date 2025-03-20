using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string message;
    public UnityEvent onInteraction;

    public void HandleInteraction()
    {
        onInteraction.Invoke();
    }
}
