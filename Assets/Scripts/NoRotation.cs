using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotation : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
