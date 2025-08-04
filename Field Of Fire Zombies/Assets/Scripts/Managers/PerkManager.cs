using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    public static PerkManager Instance;

    private HashSet<BuyingUpgrades.PerkType> boughtPerks = new HashSet<BuyingUpgrades.PerkType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsPerkBought(BuyingUpgrades.PerkType perk)
    {
        return boughtPerks.Contains(perk);
    }

    public void BuyPerk(BuyingUpgrades.PerkType perk)
    {
        boughtPerks.Add(perk);
    }
}
