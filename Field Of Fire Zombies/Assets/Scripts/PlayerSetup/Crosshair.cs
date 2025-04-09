using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float restingSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float speed;
    private CharacterController characterController;
    private RectTransform reticle;
    private float currentSize;

    private void Start()
    {
        characterController = FindAnyObjectByType<CharacterController>();
        reticle = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isMoving)
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }

    bool isMoving
    {
        get
        {
            if (characterController != null)
                if (characterController.velocity.sqrMagnitude != 0)
                    return true;
                else
                    return false;

            if (
                Input.GetAxis("Horizontal") != 0 ||
                Input.GetAxis("Vertical") != 0 ||
                Input.GetAxis("Mouse X") != 0 ||
                Input.GetAxis("Mouse Y") != 0
                    )
                return true;
            else
                return false;
        }
    }
}
