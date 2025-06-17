using UnityEngine;
using UnityEngine.UIElements;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float restingOffset = 10f;
    [SerializeField] private float maxOffset = 30f;
    [SerializeField] private float speed = 10f;

    private CharacterController characterController;
    private VisualElement topLine;
    private VisualElement bottomLine;
    private VisualElement leftLine;
    private VisualElement rightLine;
    private float currentOffset;

    private void Awake()
    {
        characterController = FindAnyObjectByType<CharacterController>();
        var root = GetComponent<UIDocument>().rootVisualElement;

        topLine = root.Q<VisualElement>("Topline");
        bottomLine = root.Q<VisualElement>("Bottomline");
        leftLine = root.Q<VisualElement>("Leftline");
        rightLine = root.Q<VisualElement>("Rightline");

        currentOffset = restingOffset;
        ApplyOffset();
    }

    private void Update()
    {
        float targetOffset = IsMoving ? maxOffset : restingOffset;
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * speed);
        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if (topLine != null)
            topLine.style.top = currentOffset;

        if (bottomLine != null)
            bottomLine.style.bottom = currentOffset;

        if (leftLine != null)
            leftLine.style.left = currentOffset;

        if (rightLine != null)
            rightLine.style.right = currentOffset;
    }

    private bool IsMoving
    {
        get
        {
            if (characterController != null && characterController.velocity.sqrMagnitude > 0.01f)
                return true;

            return Input.GetAxis("Horizontal") != 0 ||
                   Input.GetAxis("Vertical") != 0 ||
                   Input.GetAxis("Mouse X") != 0 ||
                   Input.GetAxis("Mouse Y") != 0;
        }
    }
}
