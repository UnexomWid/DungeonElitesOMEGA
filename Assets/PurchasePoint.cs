using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PurchasePoint : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] int cardId;
    [SerializeField] CharacterLoader characterLoader;

    public void OnPointerDown(PointerEventData eventData)
    {
        characterLoader.PurchasePoint(cardId);
    }
}
