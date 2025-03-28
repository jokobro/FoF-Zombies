using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour
{ }
    /*public float interactionDistance;
    public GameObject intText;
    public string doorOpenAnimName;
    public AudioClip doorOpen;
   

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                GameObject doorParent = hit.collider.transform.root.gameObject;
                Animator doorAnim = doorParent.GetComponent<Animator>();
                AudioSource doorSound = hit.collider.gameObject.GetComponent<AudioSource>();
                intText.SetActive(true);

                if (GameManager.Instance.Points >= 2000 && Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.Instance.Points -= 2000;
                    GameManager.Instance.UpdatePointsUI();
                    doorAnim.SetBool("OpenDoor", true);
                }
            }
            else
            {
                intText.SetActive(false);
            }
        }
        else
        {
            intText.SetActive(false);
        }
    }
}
*/