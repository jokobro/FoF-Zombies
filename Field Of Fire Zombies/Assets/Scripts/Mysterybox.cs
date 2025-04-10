using System.Collections.Generic;
using UnityEngine;

public class Mysterybox : MonoBehaviour
{
    [SerializeField] private List<GameObject> mysterboxItems;




    public void HandleBuyingMysterybox()
    {
        if(GameManager.Instance.Points >= 950)
        {
            GameManager.Instance.Points -= 950;
            Debug.Log("Mysterbox gekocht");
        }
    }

}
