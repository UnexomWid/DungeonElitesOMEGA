using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thisisaroom : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.transform.name);
    }
}
