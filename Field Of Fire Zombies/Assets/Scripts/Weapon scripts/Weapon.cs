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
    private float aimOffsetY = 20f;
    private bool reloading;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        /*Ray debugRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2 - aimOffsetY));
        Debug.DrawRay(debugRay.origin, debugRay.direction * maxDistance, Color.red);*/
    }
    public void Shoot()
    {
        if (currentMagAmmo > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2 - aimOffsetY));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, enemyLayerMask))
            {
                /*Debug.Log("Hit: " + hitInfo.transform.name);*/
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
        gunShotSound.Play();
    }

    public void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf)
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
        GameUIController uiController = FindObjectOfType<GameUIController>();
        if (uiController != null)
        {
            uiController.UpdateAmmoText(currentMagAmmo, currentAmmo);
        }
    }
}
