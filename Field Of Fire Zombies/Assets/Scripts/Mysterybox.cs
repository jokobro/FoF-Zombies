using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mysterybox : Interactable
{
    [Header("Mystery Box Settings")]
    [SerializeField] private AudioSource mysteryboxSound;
    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float showDuration = 5f;
    [SerializeField] private int cost = 950;

    [Header("COD Style Rolling")]
    [SerializeField] private float rollingDuration = 6.8f;
    [SerializeField] private float rollingSpeed = 0.1f;
    [SerializeField] private float finalSlowdownDuration = 1f;

    private PlayerControls.PlayerControls controls;
    private GameObject currentItem;
    private Weapon rolledWeapon;
    private Animator animator;
    private bool isRolling = false;
    private bool canTakeItem = false;
    private int lastWeaponIndex = -1;
    private bool interactPressed = false;
    private bool canUseBox = true;

    public bool CanTakeItem => canTakeItem;
    public bool IsRolling => isRolling;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new PlayerControls.PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Interact.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteractPressed;
        controls.Player.Disable();
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (canTakeItem)
        {
            interactPressed = true;
        }
    }

    public override void HandleInteraction()
    {
        base.HandleInteraction();
        if (!canUseBox || isRolling || GameManager.Instance.Points < cost) return;

        GameManager.Instance.Points -= cost;
        GameUIController.instance.RefreshUI();
        StartCoroutine(RollItemCODStyle());
    }

    private IEnumerator RollItemCODStyle()
    {
        isRolling = true;
        canTakeItem = false;
        canUseBox = false;
        interactPressed = false;

        GameUIController.instance.DisableInteractionText();
        mysteryboxSound.Play();
        animator.Play("mysterbox_Open_Anim");

        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        yield return new WaitForSeconds(0.5f);

        // Start the COD-style rolling effect
        yield return StartCoroutine(CODStyleRolling());

        // After rolling, spawn the final weapon (this one WILL update UI)
        int finalIndex;
        do
        {
            finalIndex = Random.Range(0, weaponPrefabs.Length);
        } while (weaponPrefabs.Length > 1 && finalIndex == lastWeaponIndex);

        lastWeaponIndex = finalIndex;

        GameObject finalItem = Instantiate(weaponPrefabs[finalIndex], spawnPoint.position, spawnPoint.rotation);
        currentItem = finalItem;

        StartCoroutine(MoveItemUp(finalItem.transform));
        rolledWeapon = finalItem.GetComponent<Weapon>();

        canTakeItem = true;
        float timer = 0f;

        while (timer < showDuration)
        {
            if (interactPressed)
            {
                interactPressed = false;
                TakeNewWeapon();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        canTakeItem = false;
        yield return StartCoroutine(MoveItemDown(currentItem.transform));
        Destroy(currentItem);

        animator.Play("Mysterbox_Closing_anim");

        isRolling = false;
        yield return new WaitForSeconds(rollingDuration - (rollingDuration - finalSlowdownDuration));
        canUseBox = true;
    }

    private IEnumerator CODStyleRolling()
    {
        float elapsedTime = 0f;
        float currentSpeed = rollingSpeed;

        while (elapsedTime < rollingDuration - finalSlowdownDuration)
        {
            if (currentItem != null)
            {
                Destroy(currentItem);
            }

            // Spawn rolling item WITHOUT weapon component active
            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            GameObject rollingItem = Instantiate(weaponPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

            // Disable the weapon component to prevent UI updates
            Weapon weaponComponent = rollingItem.GetComponent<Weapon>();
            if (weaponComponent != null)
            {
                weaponComponent.enabled = false;
            }

            currentItem = rollingItem;
            StartCoroutine(QuickMoveUp(rollingItem.transform));

            yield return new WaitForSeconds(currentSpeed);
            elapsedTime += currentSpeed;
        }

        // Slow down phase
        float slowdownTime = 0f;
        while (slowdownTime < finalSlowdownDuration)
        {
            if (currentItem != null)
            {
                Destroy(currentItem);
            }

            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            GameObject rollingItem = Instantiate(weaponPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

            // Disable the weapon component to prevent UI updates
            Weapon weaponComponent = rollingItem.GetComponent<Weapon>();
            if (weaponComponent != null)
            {
                weaponComponent.enabled = false;
            }

            currentItem = rollingItem;
            StartCoroutine(QuickMoveUp(rollingItem.transform));

            float slowdownProgress = slowdownTime / finalSlowdownDuration;
            float currentRollingSpeed = Mathf.Lerp(rollingSpeed, rollingSpeed * 3f, slowdownProgress);

            yield return new WaitForSeconds(currentRollingSpeed);
            slowdownTime += currentRollingSpeed;
        }

        if (currentItem != null)
        {
            Destroy(currentItem);
        }
    }

    private IEnumerator QuickMoveUp(Transform item)
    {
        if (item == null) yield break;

        Vector3 startPos = item.position;
        Vector3 endPos = startPos + Vector3.up * 0.3f;
        float duration = 0.15f;
        float t = 0f;

        while (t < 1f && item != null)
        {
            item.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / duration;
            yield return null;
        }
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
        interactPressed = false;

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

        GameObject newWeaponObj = Instantiate(rolledWeapon.gameObject, weaponSocket);
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;

        weaponSwitching.UpdateWeapons();
        weaponSwitching.SelectLastWeapon();

        Destroy(currentItem);
        GameUIController.instance.DisableInteractionText();
        animator.Play("Mysterbox_Closing_anim");
        canTakeItem = false;
        isRolling = false;
        canUseBox = true;
    }

}
