using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneDungeonSelectButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] UnityEvent function;

    bool run = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        run = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        run = false;
    }

    void Update()
    {
        if (run)
            function.Invoke();
    }
}
