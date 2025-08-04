using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponSwitching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponSocket;
    private Weapon activeWeapon;

    [Header("Settings")]
    [SerializeField] private float switchTime = 0.5f;

    public static WeaponSwitching instance;
    private float timeSinceLastSwitch;
    private int selectedWeapon;
    private Transform[] weapons;

    private PlayerControls.PlayerControls controls;

    private void Awake()
    {
        instance = this;
        controls = new PlayerControls.PlayerControls(); 

        controls.Player.ScrollWeapon.performed += OnScrollWeapon;
        controls.Player.SelectWeapon1.performed += OnSelectWeapon1;
        controls.Player.SelectWeapon2.performed += OnSelectWeapon2;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);
        timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        timeSinceLastSwitch += Time.deltaTime;
    }

    private void OnScrollWeapon(InputAction.CallbackContext context)
    {
        if (timeSinceLastSwitch < switchTime)
            return;

        Vector2 scrollValue = context.ReadValue<Vector2>();
        float scrollY = scrollValue.y;

        if (Mathf.Abs(scrollY) > 0.1f) // Threshold om kleine scroll bewegingen te negeren
        {
            int previousSelectedWeapon = selectedWeapon;

            if (scrollY > 0) // Scroll up
            {
                selectedWeapon--;
                if (selectedWeapon < 0)
                    selectedWeapon = weapons.Length - 1;
            }
            else if (scrollY < 0) // Scroll down
            {
                selectedWeapon++;
                if (selectedWeapon >= weapons.Length)
                    selectedWeapon = 0;
            }

            if (previousSelectedWeapon != selectedWeapon)
            {
                Select(selectedWeapon);
            }
        }
    }

    // Voor directe selectie van wapen 1 (key "1")
    private void OnSelectWeapon1(InputAction.CallbackContext context)
    {
        SelectSpecificWeapon(0);
    }

    // Voor directe selectie van wapen 2 (key "2")
    private void OnSelectWeapon2(InputAction.CallbackContext context)
    {
        SelectSpecificWeapon(1);
    }

    // Method voor directe weapon selection
    public void SelectSpecificWeapon(int weaponIndex)
    {
        if (timeSinceLastSwitch < switchTime)
            return;

        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            selectedWeapon = weaponIndex;
            Select(selectedWeapon);
        }
    }

    private void SetWeapons()
    {
        weapons = new Transform[weaponSocket.childCount];
        for (int i = 0; i < weaponSocket.childCount; i++)
        {
            weapons[i] = weaponSocket.GetChild(i);
        }
    }

    private void Select(int weaponIndex)
    {
        UpdateWeapons();

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }
        }

        if (weaponIndex >= 0 && weaponIndex < weapons.Length && weapons[weaponIndex] != null)
        {
            activeWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            activeWeapon?.UpdateAmmoUI();
        }
        else
        {
            activeWeapon = null;
        }

        timeSinceLastSwitch = 0f;
    }

    public Weapon[] GetAllWeapons()
    {
        Weapon[] allWeapons = new Weapon[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                allWeapons[i] = weapons[i].GetComponent<Weapon>();
            }
        }
        return allWeapons;
    }

    public Weapon GetActiveWeapon()
    {
        return activeWeapon;
    }

    public void UpdateWeapons()
    {
        weapons = new Transform[weaponSocket.childCount];
        for (int i = 0; i < weaponSocket.childCount; i++)
        {
            weapons[i] = weaponSocket.GetChild(i);
        }
    }

    public void SelectLastWeapon()
    {
        selectedWeapon = weapons.Length - 1;
        Select(selectedWeapon);
    }

    public Transform GetWeaponSocket()
    {
        return weaponSocket;
    }
}