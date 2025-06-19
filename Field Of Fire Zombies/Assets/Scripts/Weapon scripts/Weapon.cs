using System.Collections;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private new Transform camera;
    [SerializeField] private ParticleSystem muzzleFlash;
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
    private bool reloading;


   


    private void Awake()
    {
        if (camera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                camera = mainCam.transform;
            }
        }
    }

    private void Update()
    {   //dit later verwijderen
        if (camera != null)
        {
            Debug.DrawRay(camera.position, camera.forward * maxDistance);
        }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }
    public void Shoot()
    {
        if (currentMagAmmo > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, maxDistance, enemyLayerMask))
            {
                Debug.Log("Hit: " + hitInfo.transform.name);
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

    private void OnGunShot()
    {
        muzzleFlash.Play();
    }

    public void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf)
        {
            nextFire = Time.time + reloadTime;
            StartCoroutine(Reload());
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
        GameUIController uiController = FindObjectOfType<GameUIController>();
        if (uiController != null)
        {
            uiController.UpdateAmmoText(currentMagAmmo, currentAmmo);
        }
    }
}
