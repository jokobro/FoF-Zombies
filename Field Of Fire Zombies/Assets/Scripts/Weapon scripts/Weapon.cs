using System.Collections;
using TMPro;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private new Transform camera;
    /*[SerializeField] private Camera playerCamera;*/
    [SerializeField] private ParticleSystem muzzleFlash;

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
            else
            {
                Debug.LogWarning("Weapon kon geen MainCamera vinden!");
            }
        }
    }


    private void Update()
    {
        if (camera != null)
        {
            Debug.DrawRay(camera.position, camera.forward * maxDistance);
        }

        if (fireTimer < fireRate /*+ 1.0f*/)
        {
            fireTimer += Time.deltaTime;
        }
        AmmoText();
    }

    public void Shoot()
    {
        if (currentMagAmmo > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, maxDistance))
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

        reloading = false;
    }

    private void OnDisable() => reloading = false;

    public void PickupMaxAmmo()
    {
        currentAmmo = maxAmmo; // Vul reserveammo maximaal aan
        currentMagAmmo = maxClipSize; // Vul het magazijn volledig
    }

    public void AmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentMagAmmo}/{currentAmmo}";
        }
    }
}
