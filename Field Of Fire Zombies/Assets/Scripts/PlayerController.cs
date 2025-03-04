using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    CharacterController characterController;
 

    [Header("Player Settings")]
    [SerializeField] private float walkSpeed;
    
    private Vector3 moveDirection;
    private Vector2 inputMovement;
    


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 move = orientation.forward * inputMovement.y + orientation.right * inputMovement.x;
        characterController.Move(move * walkSpeed * Time.deltaTime + moveDirection * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
      inputMovement = context.ReadValue<Vector2>();
    }

}
