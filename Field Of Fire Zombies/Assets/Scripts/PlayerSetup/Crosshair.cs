using UnityEngine;
using UnityEngine.UIElements;

public class Crosshair : MonoBehaviour
{
    private VisualElement topLine, bottomLine, leftLine, rightLine;
    private CharacterController characterController;
    private float currentSize;
    private float restingSize = 22f;
    private float maxSize = 106f;
    private float speed = 10f;
    
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var crosshair = root.Q<VisualElement>("crosshair-center");
        topLine = crosshair.Q<VisualElement>("crosshair-top");
        bottomLine = crosshair.Q<VisualElement>("crosshair-bottom");
        leftLine = crosshair.Q<VisualElement>("crosshair-left");
        rightLine = crosshair.Q<VisualElement>("crosshair-right");
        characterController = FindAnyObjectByType<CharacterController>();
        currentSize = restingSize;
    }

    private void Update()
    {
        float targetSize = IsMoving() ? maxSize : restingSize;
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * speed);
        ApplySpacing(currentSize);
    }

    private bool IsMoving()
    {
        return characterController != null && characterController.velocity.sqrMagnitude > 0.1f;
    }

    private void ApplySpacing(float spacing)
    {
        topLine.style.top = -spacing;
        bottomLine.style.top = spacing;
        leftLine.style.left = -spacing;
        rightLine.style.left = spacing;
    }
}
