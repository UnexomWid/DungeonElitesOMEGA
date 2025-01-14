using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplatformTutorial : MonoBehaviour
{
    [SerializeField] GameObject phoneTutorial;
    [SerializeField] GameObject desktopTutorial;
    [SerializeField] public bool shops = false;

    private void Start()
    {
        if (shops && PlayerPrefs.GetInt("ShopsTutorial", 1) == 1)
            Begin();
    }

    public void Begin()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        phoneTutorial.SetActive(true);
#else
        desktopTutorial.SetActive(true);
#endif
    }
}
