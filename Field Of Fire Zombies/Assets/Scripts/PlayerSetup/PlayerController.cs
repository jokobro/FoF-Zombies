using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    private CharacterController characterController;
    private Weapon weapon;

    [Header("Player Settings")]
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float jumpPower = 10f;
    [HideInInspector] public float playerHealth = 100;
    [HideInInspector] public float walkSpeed;

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
    private bool isInstantKillActive;
    private bool isShooting = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        weapon = FindObjectOfType<Weapon>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleLooking();

        if (isShooting)
        {
            HandleShooting();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        playerHealth -= damageAmount;

        if (playerHealth <= 0)
        {
            if (BuyingUpgrades.Instance.IsQuickReviveBought == false)
            {
                gameObject.SetActive(false);
                PauseManager.instance.HandleEndingTheGame();  //nog fixen
            }
            else
            {
                if (BuyingUpgrades.Instance.hasUsedQuickRevive == false)
                {
                    BuyingUpgrades.Instance.UseQuickRevive();
                }
            }
        }
    }

    private void HandleLooking()
    {
        if (PauseManager.instance.isPaused)
        {
            return;
        }
        else
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * 0.1f;
            float mouseY = Input.GetAxisRaw("Mouse Y") * 0.1f;

            yRotation += mouseX * sensX;
            xRotation -= mouseY * sensY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
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
        if (context.performed)
        {
            isShooting = true;
        }
        else if (context.canceled)
        {
            isShooting = false;
        }
    }

    private void HandleShooting()
    {
        Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();

        if (currentWeapon != null)
        {
            currentWeapon.fireTimer += Time.deltaTime; // Update de fireTimer
            if (currentWeapon.fireTimer >= currentWeapon.fireRate) // Controleer of het wapen weer kan schieten
            {
                currentWeapon.Shoot();
                currentWeapon.fireTimer = 0.0f; // Reset de fireTimer
            }
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        Weapon currentWeapon = WeaponSwitching.instance.GetActiveWeapon();

        if (currentWeapon != null)
        {
            currentWeapon.StartReload();
        }
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
        switch (id)
        {
            case 0:
                if (!isDoublePointsActive)
                {
                    isDoublePointsActive = true;
                    GameManager.Instance.scoreMultiplier = 2f;
                    StartCoroutine(DoublePointsCooldown(duration));
                    Destroy(powerup);
                }
                break;
            case 1:
                GameManager.Instance.AddScore(500);
                Destroy(powerup);

                break;
            case 2:
                Weapon[] allWeapons = WeaponSwitching.instance.GetAllWeapons();
                foreach (Weapon w in allWeapons)
                {
                    w.PickupMaxAmmo();
                }
                Destroy(powerup);
                break;
            case 3:
                if (!isInstantKillActive)
                {
                    isInstantKillActive = true;
                    weapon.damage += 1000;
                    StartCoroutine(InstantKillCooldown(duration));
                    Destroy(powerup);
                }
                break;
            case 4:
                waveManager.Instance.KillAllEnemies();
                GameManager.Instance.AddScore(400);
                Destroy(powerup);
                break;
        }
    }

    IEnumerator InstantKillCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInstantKillActive = false;
        weapon.damage -= 1000;
    }

    IEnumerator DoublePointsCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDoublePointsActive = false;
        GameManager.Instance.scoreMultiplier = 1f;
    }
}