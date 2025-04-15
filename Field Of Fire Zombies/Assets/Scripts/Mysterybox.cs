using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mysterybox : Interactable
{
    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float showDuration = 5f;
    [SerializeField] private int cost = 950;

    private GameObject currentItem;
    private bool isRolling = false;
    private bool canTakeItem = false;
    private Weapon rolledWeapon;

    public bool CanTakeItem => canTakeItem;
    public bool IsRolling => isRolling;

    public override void HandleInteraction()
    {
        base.HandleInteraction();
        Debug.Log("Mysterybox HandleInteraction called");
        if (isRolling || GameManager.Instance.Points < cost) return;

        GameManager.Instance.Points -= cost;
        GameManager.Instance.UpdatePointsUI();
        StartCoroutine(RollItem());
    }

    private IEnumerator RollItem()
    {
        isRolling = true;
        canTakeItem = false;


        HUDcontroller.instance.DisableInteractionText();

        if (currentItem != null)
        {
          Destroy(currentItem);
        }
            

        yield return new WaitForSeconds(0.5f);

        int index = Random.Range(0, weaponPrefabs.Length);
        GameObject item = Instantiate(weaponPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        currentItem = item;

        // Optionele animatie omhoog
        StartCoroutine(MoveItemUp(item.transform));

        rolledWeapon = item.GetComponent<Weapon>();
        if (rolledWeapon == null && item.CompareTag("Grenade"))
        {
            GrenadeThrower thrower = FindObjectOfType<GrenadeThrower>();
            thrower.SendMessage("AddGrenades", 3, SendMessageOptions.DontRequireReceiver);
            canTakeItem = false;
            yield return new WaitForSeconds(1f);
            Destroy(currentItem);
        }
        else
        {
            canTakeItem = true;
            float timer = 0f;
            while (timer < showDuration)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TakeNewWeapon();
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (currentItem != null)
                Destroy(currentItem);
        }

        canTakeItem = false;
        isRolling = false;
    }

    private IEnumerator MoveItemUp(Transform item)
    {
        Vector3 startPos = item.position;
        Vector3 endPos = startPos + Vector3.up * 0.5f;
        float t = 0f;
        while (t < 1f)
        {
            item.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void TakeNewWeapon()
    {
        WeaponSwitching weaponSwitching = WeaponSwitching.instance;

        if (rolledWeapon == null)
        {
         return;
        }
        

        // Verwijder huidig actief wapen
        Weapon oldWeapon = weaponSwitching.GetActiveWeapon();
        if (oldWeapon != null)
        {
            Destroy(oldWeapon.gameObject);
        }

        // Voeg nieuwe wapen toe aan speler
        GameObject newWeaponObj = Instantiate(currentItem, weaponSwitching.transform);
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;

        // Update weapon list en selecteer deze
        weaponSwitching.UpdateWeapons();
        weaponSwitching.SelectLastWeapon();

        Destroy(currentItem); // Verwijder het wapen uit de box

        HUDcontroller.instance.DisableInteractionText();
    }
}
