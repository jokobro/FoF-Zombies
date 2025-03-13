using System.Collections;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private new Transform camera;

    [Header("Weapon settings")]
    public float damage;
    public int currentMagAmmo;
    public int maxclipSize;

    public int Maxammo;
    public float maxDistance;
    public float reloadTime;


    private int reloadAmount;

    [HideInInspector] public float fireTimer;
    [HideInInspector] public float nextFire;
    [HideInInspector] public float fireRate = 0.245f;
    private bool reloading;


    private void Update()
    {
        Debug.DrawRay(camera.position, camera.forward * maxDistance);

        if (fireTimer < fireRate + 1.0f)
        {
            fireTimer += Time.deltaTime;
        }
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
            OnGunShot();
            currentMagAmmo--;
        }
    }

    private void OnGunShot()
    {
        // toevoegen van particles en andere dingen 
    }

    public void Addammo()
    {
        currentMagAmmo = maxclipSize;
    }
    public void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentMagAmmo = maxclipSize;
        reloading = false;
    }

    private void OnDisable() => reloading = false;
}
