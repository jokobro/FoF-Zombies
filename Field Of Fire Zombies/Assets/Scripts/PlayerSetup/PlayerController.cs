using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    private CharacterController characterController;
    private GameUIController cachedUIController;
    private WeaponSwitching weaponSwitching;
    private PauseManager pauseManager;
    private GameManager gameManager;
    private Weapon cachedWeapon;

    [Header("Player Settings")]
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float jumpPower = 10f;
    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float playerCurrentHealth;
    [HideInInspector] public float playerMaxHealth;

    [Header("Look Settings")]
    [SerializeField] private float sensX = 10f;
    [SerializeField] private float sensY = 10f;

    [Header("Drag")]
    private float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Regeneration")]
    private float timeBetweenDamageAndRegen = 6f;
    private float startRegenTime = 0.0f;
    private float regenRate = 5f;
    private bool needsRegen = false;

    [Header("Powerup Audio")]
    [SerializeField] private AudioClip bonusPointsSound;
    [SerializeField] private AudioClip doublePointsSound;
    [SerializeField] private AudioClip maxAmmoSound;
    [SerializeField] private AudioClip instantKillSound;
    [SerializeField] private AudioClip nukeSound;
    private float powerupAudioVolume = 1f;

    private Vector3 moveDirection;
    private Vector2 inputMovement;
    private float yRotation;
    private float xRotation;
    private bool isDoublePointsActive;
    private bool isInstantKillActive;
    private bool isShooting = false;

    // Pre-calculated values
    private readonly float mouseInputMultiplier = 0.1f;

    // Performance flags - vermijd herhaaldelijke null checks
    private bool hasValidUIController;
    private bool hasValidPauseManager;
    private bool hasValidWeaponSwitching;
    private bool RegenCanStart => Time.time > startRegenTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        CacheAllReferences();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        yRotation = 180f; // Pas deze waarde aan naar de gewenste richting
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        UpdateHealthUI();
    }

    private void CacheAllReferences()
    {
        // Cache alle references in één keer en set validity flags
        cachedUIController = GameUIController.instance;
        hasValidUIController = cachedUIController != null;

        pauseManager = PauseManager.instance;
        hasValidPauseManager = pauseManager != null;

        weaponSwitching = WeaponSwitching.instance;
        hasValidWeaponSwitching = weaponSwitching != null;

        gameManager = GameManager.Instance;

        // Only search for weapon once if really needed
        if (cachedWeapon == null)
        {
            cachedWeapon = FindFirstObjectByType<Weapon>();
        }
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

        if (needsRegen && RegenCanStart)
        {
            RegenerateHealth();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        playerCurrentHealth -= damageAmount;
        needsRegen = true;
        startRegenTime = Time.time + timeBetweenDamageAndRegen;

        UpdateHealthUI();

        if (playerCurrentHealth <= 0)
        {
            isShooting = false;

            if (hasValidPauseManager)
            {
                pauseManager.StartCoroutine(pauseManager.DelayedEndGame());
            }
        }
    }

    private void RegenerateHealth()
    {
        playerCurrentHealth += regenRate * Time.deltaTime;

        if (playerCurrentHealth >= playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth;
            needsRegen = false;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Gebruik validity flag in plaats van null check elke keer
        if (!hasValidUIController) return;

        float healthRatio = playerCurrentHealth / playerMaxHealth;

        cachedUIController.bloodSplatter1.style.visibility = Visibility.Hidden;
        cachedUIController.bloodSplatter2.style.visibility = Visibility.Hidden;

        if (healthRatio <= 0.45f)
        {
            cachedUIController.bloodSplatter2.style.visibility = Visibility.Visible;
        }
        else if (healthRatio <= 0.8f)
        {
            cachedUIController.bloodSplatter1.style.visibility = Visibility.Visible;
        }
    }

    private void HandleLooking()
    {
        // Gebruik cached reference en validity flag
        if (hasValidPauseManager && pauseManager.isPaused)
            return;

        // Pre-calculated multiplier
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseInputMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseInputMultiplier;

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
        if (WeaponUpgradeManager.Instance != null && WeaponUpgradeManager.Instance.isUpgrading)
        {
            return;
        }

        // Gebruik cached reference zonder null-conditional operator
        if (!hasValidWeaponSwitching)
        {
            return;
        }

        Weapon currentWeapon = weaponSwitching.GetActiveWeapon();
        if (currentWeapon != null)
        {
            currentWeapon.fireTimer += Time.deltaTime;
            if (currentWeapon.fireTimer >= currentWeapon.fireRate)
            {
                currentWeapon.Shoot();
                currentWeapon.fireTimer = 0.0f;
            }
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed && hasValidWeaponSwitching)
        {
            Weapon currentWeapon = weaponSwitching.GetActiveWeapon();
            if (currentWeapon != null)
            {
                currentWeapon.StartReload();
            }
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
            case 0: // Double Points
                if (!isDoublePointsActive && gameManager != null)
                {
                    isDoublePointsActive = true;
                    gameManager.scoreMultiplier = 2f;
                    PowerupUIManager.Instance?.ShowPowerup(0, duration);
                    StartCoroutine(DoublePointsCooldown(duration));
                    AudioSource.PlayClipAtPoint(doublePointsSound, transform.position, powerupAudioVolume);
                    Destroy(powerup);
                }
                break;
            case 1: // Bonus Points
                if (gameManager != null)
                {
                    gameManager.AddScore(500);
                    AudioSource.PlayClipAtPoint(bonusPointsSound, transform.position, powerupAudioVolume);
                    Destroy(powerup);
                }
                break;
            case 2: // Max Ammo
                if (hasValidWeaponSwitching)
                {
                    AudioSource.PlayClipAtPoint(maxAmmoSound, transform.position, powerupAudioVolume);
                    Weapon[] allWeapons = weaponSwitching.GetAllWeapons();
                    if (allWeapons != null)
                    {
                        for (int i = 0; i < allWeapons.Length; i++)
                        {
                            if (allWeapons[i] != null)
                                allWeapons[i].PickupMaxAmmo();
                        }
                    }
                }
                Destroy(powerup);
                break;
            case 3: // Instant Kill
                if (!isInstantKillActive && hasValidWeaponSwitching)
                {
                    isInstantKillActive = true;

                    Weapon[] allWeapons = weaponSwitching.GetAllWeapons();
                    if (allWeapons != null)
                    {
                        for (int i = 0; i < allWeapons.Length; i++)
                        {
                            if (allWeapons[i] != null)
                            {
                                allWeapons[i].damage += 1000;
                            }
                        }
                    }

                    PowerupUIManager.Instance?.ShowPowerup(3, duration);
                    StartCoroutine(InstantKillCooldown(duration));
                    AudioSource.PlayClipAtPoint(instantKillSound, transform.position, powerupAudioVolume);
                    Destroy(powerup);
                }
                break;
            case 4: // Nuke
                waveManager waveManagerInstance = waveManager.Instance;
                if (waveManagerInstance != null)
                {
                    AudioSource.PlayClipAtPoint(nukeSound, transform.position, powerupAudioVolume);
                    waveManagerInstance.KillAllEnemies();
                    if (gameManager != null)
                    {
                        gameManager.AddScore(400);
                    }
                }
                Destroy(powerup);
                break;
        }
    }

    private IEnumerator InstantKillCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInstantKillActive = false;

        if (hasValidWeaponSwitching)
        {
            Weapon[] allWeapons = weaponSwitching.GetAllWeapons();
            if(allWeapons != null)
            {
                for (int i = 0; i < allWeapons.Length; i++)
                {
                    if (allWeapons[i] != null)
                    {
                        allWeapons[i].damage -= 1000;
                    }
                }
            }
        }
    }

    private IEnumerator DoublePointsCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDoublePointsActive = false;
        if (gameManager != null)
        {
            gameManager.scoreMultiplier = 1f;
        }
    }
}