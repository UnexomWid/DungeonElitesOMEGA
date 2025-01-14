using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Vector2 currentPos;


    bool keyboard = true;
    Transform myTransform;
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        keyboard = false;
#endif

        myTransform = transform;
    }

    void FixedUpdate()
    {
        if (keyboard) //0.054ms economisite ahahahahhahah
        {
            currentPos = PublicVariables.mousePositionWorldPoint;
            myTransform.position = currentPos;
        }
    }

    private void Update()
    {
        if (keyboard)
        {
            currentPos = PublicVariables.mousePositionWorldPoint;
            myTransform.position = currentPos;
        }
    }
}
