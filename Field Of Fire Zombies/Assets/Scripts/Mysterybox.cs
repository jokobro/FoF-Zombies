using System.Collections;
using UnityEngine;

public class Mysterybox : Interactable
{
    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float showDuration = 5f;
    [SerializeField] private int cost = 950;

    private GameObject currentItem;
    private Weapon rolledWeapon;
    private Animator animator;
    private bool isRolling = false;
    private bool canTakeItem = false;
    private int lastWeaponIndex = -1;
    public bool CanTakeItem => canTakeItem;
    public bool IsRolling => isRolling;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void HandleInteraction()
    {
        base.HandleInteraction();
        if (isRolling || GameManager.Instance.Points < cost) return;

        GameManager.Instance.Points -= cost;
        GameUIController.instance.RefreshUI();
        StartCoroutine(RollItem());
    }

    private IEnumerator RollItem()
    {
        isRolling = true;
        canTakeItem = false;

        GameUIController.instance.DisableInteractionText();
        animator.Play("mysterbox_Open_Anim");

        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        yield return new WaitForSeconds(0.5f);

        // Kies een ander wapen dan het vorige
        int index;
        do
        {
            index = Random.Range(0, weaponPrefabs.Length);
        } while (weaponPrefabs.Length > 1 && index == lastWeaponIndex);

        lastWeaponIndex = index;

        GameObject item = Instantiate(weaponPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        currentItem = item;

        StartCoroutine(MoveItemUp(item.transform));
        rolledWeapon = item.GetComponent<Weapon>();

        canTakeItem = true;
        float timer = 0f;

        while (timer < showDuration)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                TakeNewWeapon();
                yield break; // Stop hier, omdat TakeNewWeapon alles afhandelt
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Als tijd op is en item niet genomen is
        canTakeItem = false;
        yield return StartCoroutine(MoveItemDown(currentItem.transform));
        Destroy(currentItem);

        animator.Play("Mysterbox_Closing_anim");

        isRolling = false;
    }

    private IEnumerator MoveItemUp(Transform item)
    {
        Vector3 startPos = item.position;
        Vector3 endPos = startPos + Vector3.up * 0.5f;
        float t = 0f;
        while (t < 1f)
        {
            if (item == null)
                yield break;

            item.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveItemDown(Transform item)
    {
        Vector3 endPos = item.position;
        Vector3 startPos = endPos - Vector3.up * 0.5f;
        float t = 0f;
        while (t < 1f)
        {
            if (item == null)
                yield break;

            item.position = Vector3.Lerp(endPos, startPos, t);
            t += Time.deltaTime;
            yield return null;
        }
        item.position = startPos;
    }

    private void TakeNewWeapon()
    {
        WeaponSwitching weaponSwitching = WeaponSwitching.instance;

        if (rolledWeapon == null)
            return;

        Transform weaponSocket = weaponSwitching.GetWeaponSocket();

        if (weaponSocket.childCount >= 2)
        {
            Weapon oldWeapon = weaponSwitching.GetActiveWeapon();
            if (oldWeapon != null)
            {
                Destroy(oldWeapon.gameObject);
            }
        }

        // Voeg nieuwe wapen toe aan socket
        GameObject newWeaponObj = Instantiate(rolledWeapon.gameObject, weaponSocket);
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;
        // Update en selecteer nieuwe wapen
        weaponSwitching.UpdateWeapons();
        weaponSwitching.SelectLastWeapon();

        Destroy(currentItem);
        GameUIController.instance.DisableInteractionText();
        animator.Play("Mysterbox_Closing_anim");
        canTakeItem = false;
        isRolling = false;
    }
}
