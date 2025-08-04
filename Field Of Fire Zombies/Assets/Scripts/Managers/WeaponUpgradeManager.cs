using System.Collections;
using UnityEngine;

public class WeaponUpgradeManager : MonoBehaviour
{
    public static WeaponUpgradeManager Instance;

    [HideInInspector] public bool isUpgrading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartWeaponUpgrade(float upgradeDuration)
    {
        if (!isUpgrading)
        {
            StartCoroutine(UpgradeCoroutine(upgradeDuration));
        }
    }

    private IEnumerator UpgradeCoroutine(float duration)
    {
        isUpgrading = true;
        yield return new WaitForSeconds(duration);
        isUpgrading = false;
    }
}
