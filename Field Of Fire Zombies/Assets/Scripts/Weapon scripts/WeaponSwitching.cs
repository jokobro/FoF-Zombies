using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] weapons;
    private Weapon activeWeapon;
    
    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    public static WeaponSwitching instance;
    private float timeSinceLastSwitch;
    private int selectedWeapon;
    
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

    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            weapons[i] = transform.GetChild(i);

        if (keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
                selectedWeapon = i;

        if (previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == weaponIndex);

        activeWeapon = weapons[weaponIndex].GetComponent<Weapon>();

        if (activeWeapon != null)
        {
            activeWeapon.AmmoText();
        }

        timeSinceLastSwitch = 0f;
    }
    public Weapon[] GetAllWeapons()
    {
        Weapon[] allWeapons = new Weapon[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            allWeapons[i] = weapons[i].GetComponent<Weapon>();
        }

        return allWeapons;
    }
    public Weapon GetActiveWeapon()
    {
        return activeWeapon;
    }

    public void UpdateWeapons()
    {
        weapons = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }
    }

    public void SelectLastWeapon()
    {
        selectedWeapon = weapons.Length - 1;
        Select(selectedWeapon);
    }
}
