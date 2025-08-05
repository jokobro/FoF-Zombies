using System.Collections;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource gunShotSound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Weapon settings")]
    public float damage;
    public int currentMagAmmo; // Ammo in magazijn
    public int maxClipSize; // Max ammo in magazijn
    public int currentAmmo; // Huidige ammo in reserve
    public int maxAmmo; // Maximaal aantal kogels dat je kunt dragen
    public float maxDistance;
    public float reloadTime = 3;
    public float fireRate;

    [HideInInspector] public float fireTimer;
    [HideInInspector] public float nextFire;
    [HideInInspector] public bool isWeaponUpgraded = false;


    private GameUIController cachedUIController;
    private Camera mainCamera;
    private float aimOffsetY = 20f;
    private bool reloading;
    private Vector3 screenCenter;
    private float originalFireRate;

    private void Start()
    {
        cachedUIController = GameUIController.instance;
        mainCamera = Camera.main;
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, - aimOffsetY);
        originalFireRate = fireRate;
        UpdateAmmoUI();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
    }
    public void Shoot()
    {
        if (currentMagAmmo > 0 && Time.time > nextFire && !reloading)
        {
            nextFire = Time.time + fireRate;

            Ray ray = mainCamera.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, enemyLayerMask))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
            else if (reloading == true)
            {
                nextFire = Time.time + reloadTime;
            }
            OnGunShot();
            currentMagAmmo--;
            UpdateAmmoUI();
        }
    }
    public void ResetToOriginalFireRate()
    {
        fireRate = originalFireRate;
    }

    private void OnGunShot()
    {
        muzzleFlash.Play();
        gunShotSound.Play();
    }

    public void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf && currentMagAmmo < maxClipSize)
        {
            nextFire = Time.time + reloadTime;
            StartCoroutine(Reload());
            reloadSound.Play();
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = maxClipSize - currentMagAmmo; // Hoeveel kogels nodig
        int ammoToLoad = Mathf.Min(neededAmmo, currentAmmo); // Laad alleen wat beschikbaar is

        currentMagAmmo += ammoToLoad; // Voeg de kogels toe aan het magazijn
        currentAmmo -= ammoToLoad; // Trek de gebruikte kogels af van de reserve

        UpdateAmmoUI();

        reloading = false;
    }

    private void OnDisable()
    {
        reloading = false;
    }

    public void PickupMaxAmmo()
    {
        currentAmmo = maxAmmo; // Vul reserveammo maximaal aan
        currentMagAmmo = maxClipSize; // Vul het magazijn volledig
        UpdateAmmoUI();
    }

    public void UpdateAmmoUI()
    {
        if (cachedUIController != null)
        {
            cachedUIController.UpdateAmmoText(currentMagAmmo, currentAmmo);
        }
    }
}
