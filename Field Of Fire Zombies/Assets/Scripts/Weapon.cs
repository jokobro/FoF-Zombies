using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform camera;
    [SerializeField] private GunData gunData;
   
    private float timeSinceLastShot;

    private void Update()
    {
        timeSinceLastShot = Time.deltaTime;
        Debug.DrawRay(camera.position, camera.forward * gunData.maxDistance);
    }
   
    private void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    damageable?.TakeDamage(gunData.damage);
                }
                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
    }

    private void OnGunShot()
    {
        // toevoegen van particles en andere dingen 
    }

    private void StartReload()
    {
        if (!gunData.reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    private void OnDisable() => gunData.reloading = false;
}
