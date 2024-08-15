using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] public GameObject canvas;
    [SerializeField] public GameObject phoneControls;
    [SerializeField] public MultiplatformTutorial movement;
    [SerializeField] public MultiplatformTutorial inventory;
    [SerializeField] public MultiplatformTutorial megamap;
    [SerializeField] public MultiplatformTutorial interact;
    [SerializeField] public GameObject inventoryIcon;
    [SerializeField] public GameObject megamapIcon;
    [SerializeField] public GameObject interactIcon;

    public bool activatedInventory = false;

    public static TutorialScene instance;

    public void EnableInteractIcon()
    {
        interactIcon.SetActive(true);
    }

    public void EnableInventoryIcon()
    {
        inventoryIcon.SetActive(true);
    }

    public void EnableMegamapIcon()
    {
        megamapIcon.SetActive(true);
    }

    void Start()
    {
        Camera.main.GetComponent<CameraFollow>().Begin();
        foreach (player player in FindObjectsOfType<player>())
            player.ableToMove = false;
        canvas.GetComponent<Animator>().Play("fadeIn");
        Invoke("Tutorial", 4);
        instance = this;
    }

    void Tutorial()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        phoneControls.SetActive(true);
#endif

        movement.Begin();

        Camera.main.GetComponent<CameraFollow>().enabled = true;

        foreach (TutorialWalkingBot player in FindObjectsOfType<TutorialWalkingBot>())
        {
            player.enabled = false;
            player.GetComponent<player>().ableToMove = true;
        }
    }
}
