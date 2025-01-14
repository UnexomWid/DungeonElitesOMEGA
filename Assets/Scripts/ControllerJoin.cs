using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerJoin : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IPHONE
    gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        if (FindObjectsOfType<ControllerJoin>().Length >= 2)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }
}
