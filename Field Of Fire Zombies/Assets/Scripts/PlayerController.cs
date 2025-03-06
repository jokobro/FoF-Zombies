using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    [SerializeField] private Weapon weapon;
    private CharacterController characterController;

    [Header("Player Settings")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float walkSpeed;
    [SerializeField] private int points = 500;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float playerHealth = 100; // moet nog getweaked worden

    [Header("Look Settings")]
    [SerializeField] private float sensX = 10f;
    [SerializeField] private float sensY = 10f;

    [Header("Drag")]
    private float gravity = -9.81f;
    private float verticalVelocity;

    private Vector3 moveDirection;
    private Vector2 inputMovement;
    private float yRotation;
    private float xRotation;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleLooking();
    }

    private void HandleLooking()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * 0.1f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * 0.1f;

        yRotation += mouseX * sensX;
        xRotation -= mouseY * sensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void HandleMovement()
    {
        Vector3 move = orientation.forward * inputMovement.y + orientation.right * inputMovement.x;
        characterController.Move(move * walkSpeed * Time.deltaTime + moveDirection * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (IsGrounded() && verticalVelocity < 0)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }
        moveDirection.y = verticalVelocity;
    }
   
    public void Move(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            verticalVelocity = jumpPower;
        }
    }

    private bool IsGrounded() => characterController.isGrounded;
}
