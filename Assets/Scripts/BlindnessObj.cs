using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindnessObj : MonoBehaviour
{
    [SerializeField] GameObject child;

    public void Enable()
    {
        child.SetActive(false);
        child.SetActive(true);
    }
}
