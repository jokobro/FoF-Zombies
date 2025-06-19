using UnityEngine;
using UnityEngine.UIElements;

public class Crosshair : MonoBehaviour
{
    private VisualElement topLine, bottomLine, leftLine, rightLine;
    private float currentSize;
    private float restingSize = 29f;
    private float maxSize = 120f;
    private float speed = 10f;
    private CharacterController characterController;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var crosshair = root.Q<VisualElement>("Crosshair");
        topLine = crosshair.Q<VisualElement>("TopLine");
        bottomLine = crosshair.Q<VisualElement>("BottomLine");
        leftLine = crosshair.Q<VisualElement>("LeftLine");
        rightLine = crosshair.Q<VisualElement>("RightLine");
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
