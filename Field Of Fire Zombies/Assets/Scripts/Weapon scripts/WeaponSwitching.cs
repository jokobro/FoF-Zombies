using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponSocket; // Hier spawnen nieuwe wapens
    private Weapon activeWeapon;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    public static WeaponSwitching instance;
    private float timeSinceLastSwitch;
    private int selectedWeapon;
    /*[SerializeField]*/ private Transform[] weapons;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);
        timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
            {
                if (i < weapons.Length && weapons[i] != null)
                {
                    selectedWeapon = i;
                }
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            Select(selectedWeapon);
        }

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void SetWeapons()
    {
        weapons = new Transform[weaponSocket.childCount];
        for (int i = 0; i < weaponSocket.childCount; i++)
        {
            weapons[i] = weaponSocket.GetChild(i);
        }

        if (keys == null || keys.Length != weapons.Length)
        {
            keys = new KeyCode[weapons.Length];
        }
    }

    private void Select(int weaponIndex)
    {
        UpdateWeapons(); // <- Zorg dat je altijd up-to-date bent

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }
        }

        // Probeer het nieuwe actieve wapen op te halen
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