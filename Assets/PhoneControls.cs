using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneControls : MonoBehaviour
{
    [SerializeField] GameObject desktopVariant;
    [SerializeField] GameObject phoneVariant;
    private void Awake()
    {
#if UNITY_IPHONE || UNITY_ANDROID
            if(desktopVariant != null)
            {
                desktopVariant.SetActive(false);
            }
            if(phoneVariant != null)
            {
                phoneVariant.SetActive(true);
            }
#else
        if (desktopVariant != null)
        {
            desktopVariant.SetActive(true);
        }
        if (phoneVariant != null)
        {
            phoneVariant.SetActive(false);
        }
#endif
    }
}
