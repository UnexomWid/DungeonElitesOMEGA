using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneTutorial : MonoBehaviour
{
    [SerializeField] RectTransform phoneRect;
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IPHONE

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.localPosition = phoneRect.transform.localPosition;
            rectTransform.localScale = phoneRect.transform.localScale;
#endif
    }
}
