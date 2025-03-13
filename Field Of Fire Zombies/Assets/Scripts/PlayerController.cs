using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    private Weapon weapon;
    private CharacterController characterController;

    [Header("Player Settings")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float walkSpeed;
    /*[SerializeField] private int points = 500;*/
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float playerHealth = 100; // moet nog getweaked worden
    GameManager gameManager;

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
    private bool isDoublePointsActive;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weapon = FindObjectOfType<Weapon>();
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

    public void Shoot(InputAction.CallbackContext context)
    {
        if (weapon.fireTimer > weapon.fireRate)
        {
            weapon.Shoot();
            weapon.fireTimer = 0.0f;
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        weapon.StartReload();
        Debug.Log("Start reload");
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

    public void ActivatePowerup(int id, float duration, GameObject powerup)
    {
        if (id == 0)
        {
            if (!isDoublePointsActive)
            {
                ActivateDoublePoints(duration);
                Destroy(powerup);
            }
        }
        else if (id == 1)
        {
            BonusPoints();
            Destroy(powerup);
            Debug.Log("bonus points opgepakt");
        }
        else if (id == 2)
        {
            weapon.Addammo();
            Destroy(powerup);
            Debug.Log("maxammo opgepakt");
        }
    }

    private void ActivateDoublePoints(float duration)
    {
        Debug.Log("double points active");
        isDoublePointsActive = true;
        gameManager.scoreMultiplier = 2f;
        StartCoroutine(DoublePointsCooldown(duration));
    }

    IEnumerator DoublePointsCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDoublePointsActive = false;
        gameManager.scoreMultiplier = 1f;
        Debug.Log("double points uitgezet");
    }

    private void BonusPoints()
    {
        gameManager.AddScore(500);
    }
}
