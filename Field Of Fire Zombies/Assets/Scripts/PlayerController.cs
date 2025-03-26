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
    private WeaponSwitching weaponSwitching;
    private GameManager gameManager;
    private Weapon weapon;
    private Camera playerCamera;

    [SerializeField] private Transform weaponAimingPosition;
    [SerializeField] private Transform weaponDefaultPosition;


    [Header("Player Settings")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    public float playerHealth = 100; // moet nog getweaked worden
    public float walkSpeed;
    [SerializeField] private float aimSpeed = 0.25f;



    [Header("Look Settings")]
    [SerializeField] private float sensX = 10f;
    [SerializeField] private float sensY = 10f;
    private float defaultFOV = 90f;
    private float zoomAmount = 0.5f;

    [Header("Drag")]
    private float gravity = -9.81f;
    private float verticalVelocity;

    private Vector3 moveDirection;
    private Vector3 weaponPostion;
    private Vector2 inputMovement;
    private float yRotation;
    private float xRotation;
    private bool isDoublePointsActive;
    private bool isInstantKillActive;


    [SerializeField] private Transform activeWeapon;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        characterController = GetComponent<CharacterController>();
        weaponSwitching = FindAnyObjectByType<WeaponSwitching>(); // Zoek WeaponSwitching
        weapon = FindAnyObjectByType<Weapon>();
        playerCamera = FindAnyObjectByType<Camera>();


        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleLooking();

        /*HanleAiming();*/

        PlayerHealth();
    }

    private void PlayerHealth()
    {

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

    /*private void HanleAiming()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            weaponPostion = Vector3.Lerp(weaponPostion, weaponAimingPosition.localPosition, aimSpeed * Time.deltaTime);
            activeWeapon.localPosition = weaponPostion;
            SetFieldOFView(Mathf.Lerp(playerCamera.fieldOfView, zoomAmount * defaultFOV, aimSpeed * Time.deltaTime));
        }
        else
        {
            weaponPostion = Vector3.Lerp(weaponPostion, weaponDefaultPosition.localPosition, aimSpeed * Time.deltaTime);
            activeWeapon.localPosition = weaponPostion;
            SetFieldOFView(Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, aimSpeed * Time.deltaTime));
        }
    }*/

    private void SetFieldOFView(float FieldOfView)
    {
        playerCamera.fieldOfView = FieldOfView;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        Weapon currentWeapon = weaponSwitching.GetActiveWeapon(); // Gebruik het actieve wapen

        if (currentWeapon != null && currentWeapon.fireTimer > currentWeapon.fireRate)
        {
            currentWeapon.Shoot();
            currentWeapon.fireTimer = 0.0f;
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        Weapon currentWeapon = weaponSwitching.GetActiveWeapon();

        if (currentWeapon != null)
        {
            currentWeapon.StartReload();
        }

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
            Weapon[] allWeapons = weaponSwitching.GetAllWeapons();
            foreach (Weapon w in allWeapons)
            {
                w.PickupMaxAmmo();
            }
            Destroy(powerup);
            /*Debug.Log("maxammo opgepakt");*/
        }
        else if (id == 3)
        {
            if (!isInstantKillActive)
            {
                ActivateInstantKill(duration);
                Destroy(powerup);
            }
        }
    }

    private void ActivateInstantKill(float duration)
    {
        Debug.Log("Instant kill active");
        isInstantKillActive = true;
        weapon.damage += 1000;
        StartCoroutine(InstantKillCooldown(duration));
    }

    IEnumerator InstantKillCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInstantKillActive = false;
        weapon.damage -= 1000;
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
