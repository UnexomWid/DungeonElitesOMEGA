using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneControllerTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] PhoneController phoneController;
    public void OnPointerClick(PointerEventData eventData)
    {
        phoneController.TriggerClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        phoneController.TriggerClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        phoneController.StopTrigger();
    }
}
