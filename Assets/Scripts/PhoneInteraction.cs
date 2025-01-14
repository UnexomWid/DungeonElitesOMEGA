using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] Image image;
    [SerializeField] Sprite lockImage;
    [SerializeField] Sprite unlockImage;

    public bool isInteracting = false;

    public void SetLockImage()
    {
        image.sprite = lockImage;
    }

    public void SetUnlockImage()
    {
        image.sprite = unlockImage;
    }

    void OnDisable()
    {
        isInteracting = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInteracting = true;

        if(lockImage != null)
        {
            if (TutorialScene.instance != null)
            {
                if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                {
                    Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 4)
                        tutorial.NextQuote();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInteracting = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInteracting = false;
    }

    public void Highlight()
    {
        image.color = new Color32(255, 190, 0, 192);
    }

    public void Unhighlight()
    {
        image.color = new Color32(255, 190, 0, 64);
    }

    public void Interact()
    {

    }
}
