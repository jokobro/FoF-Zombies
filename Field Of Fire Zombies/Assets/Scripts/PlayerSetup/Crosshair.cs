using UnityEngine;
using UnityEngine.UIElements;

public class Crosshair : MonoBehaviour
{
    private VisualElement topLine, bottomLine, leftLine, rightLine;
    private float currentSize;
    [SerializeField] private float restingSize = 10f;
    [SerializeField] private float maxSize = 30f;
    [SerializeField] private float speed = 5f;

    private CharacterController characterController;

    void Start()
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

    void Update()
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
