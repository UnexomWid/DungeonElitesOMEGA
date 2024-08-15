using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneAbillities : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Image image;

    public Transform sizer;
    public Image cooldownImage;
    public void GetSizer()
    {
        sizer = image.transform.GetChild(0);
        sizer.transform.localPosition = -sizer.transform.localPosition;
        sizer.transform.localEulerAngles = new Vector3(0, 180, 0);
        cooldownImage = sizer.GetChild(0).GetComponent<Image>();
    }

    public void Activate()
    {
        if(image != null)
            image.color = new Color32(255, 255, 255, 192);
    }

    public void Deactivate()
    {
        if (image != null)
            image.color = new Color32(128, 128, 128, 192);
    }

    public bool attack = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        attack = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        attack = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        attack = false;
    }
}
