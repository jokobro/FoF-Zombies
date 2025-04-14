using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform weaponAimingPosition;
    [SerializeField] private Transform weaponDefaultPosition;
    [SerializeField] private GameObject HighscoreScreenPanel;
    [SerializeField] private GameObject UiPanel;
    private CharacterController characterController;
    private Camera playerCamera;
    private Weapon weapon;

    [Header("Player Settings")]
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float aimSpeed = 0.25f;
    [SerializeField] private float jumpPower = 10f;
    public float playerHealth = 100; // moet nog getweaked worden // en prive van de inspector gezet worden
   /* [HideInInspector]*/ public float walkSpeed;

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
    private bool isShooting = false;

    [SerializeField] private Transform activeWeapon;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();//kijken of ik dit kan verbeteren qua references
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleLooking();

        /*HanleAiming();*/

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
                PauseManager.Instance.EndGame();
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
           /* Debug.Log("bonus points opgepakt");*/
        }
        else if (id == 2)
        {
            Weapon[] allWeapons = WeaponSwitching.instance.GetAllWeapons();
            foreach (Weapon w in allWeapons)
            {
                w.PickupMaxAmmo();
            }
            Destroy(powerup);
        }
        else if (id == 3)
        {
            if (!isInstantKillActive)
            {
                ActivateInstantKill(duration);
                Destroy(powerup);
            }
        }
        else if (id == 4)
        {
            waveManager.Instance.KillCurrentWave();
            GameManager.Instance.AddScore(400);
            Destroy(powerup);
        }
    }

    private void ActivateInstantKill(float duration)
    {
      /*  Debug.Log("Instant kill active");*/
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
       /* Debug.Log("double points active");*/
        isDoublePointsActive = true;
        GameManager.Instance.scoreMultiplier = 2f;
        StartCoroutine(DoublePointsCooldown(duration));
    }

    IEnumerator DoublePointsCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDoublePointsActive = false;
        GameManager.Instance.scoreMultiplier = 1f;
        /*Debug.Log("double points uitgezet");*/
    }

    private void BonusPoints()
    {
        GameManager.Instance.AddScore(500);
    }
}
