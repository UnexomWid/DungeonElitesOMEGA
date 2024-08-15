using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicVariables : MonoBehaviour
{
    public static float TimeScale;
    public static Vector2 mousePosition;
    public static Vector3 mousePositionWorldPoint;
    public static float deltaTime;
    public static float fixedDeltaTime;
    public static Camera mainCamera;

    private void Start()
    {
        if (FindObjectsOfType<PublicVariables>().Length >= 2)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void FixedUpdate()
    {
        mousePosition = Input.mousePosition;
        mousePositionWorldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        fixedDeltaTime = Time.fixedDeltaTime;
        mainCamera = Camera.main;
    }

    void Update()
    {
        mainCamera = Camera.main;
        TimeScale = Time.timeScale;
        mousePosition = Input.mousePosition;
        mousePositionWorldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        deltaTime = Time.deltaTime;
    }
}
