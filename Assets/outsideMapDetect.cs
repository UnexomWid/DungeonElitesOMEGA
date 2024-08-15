using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outsideMapDetect : MonoBehaviour
{
    public GameObject parent;
    Vector2 oldPos;
    Vector2 newPos;
    private void Update()
    {/*
        oldPos = newPos;
        newPos = transform.position;
        Collider2D[] boxes = Physics2D.OverlapBoxAll(newPos, new Vector2(0.01f, 0.01f), 0f);
        bool ok = false;
        foreach (Collider2D box in boxes)
        {
            if (box.GetComponent<room>() != null)
            {
                ok = true;
            }
        }
        if (ok == false)
        {
            parent.GetComponent<hitbox>().DestroyObject();
        }*/
    }
}
