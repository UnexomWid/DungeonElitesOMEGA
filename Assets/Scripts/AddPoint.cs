using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddPoint : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] int cardId;
    [SerializeField] string type;
    [SerializeField] bool remove;
    [SerializeField] CharacterLoader characterLoader;

    public void OnPointerDown(PointerEventData eventData)
    {
        characterLoader.Add(cardId, type, remove);
    }
}
