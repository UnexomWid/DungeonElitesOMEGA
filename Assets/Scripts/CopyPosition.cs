using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public GameObject target;

    Transform myTransform;
    Transform targetTransform;
    private void Start()
    {
        myTransform = transform;
        targetTransform = target.transform;
    }

    void Update()
    {
        myTransform.position = targetTransform.position;
    }
}
