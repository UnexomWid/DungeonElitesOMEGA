using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullets : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D col)
    {
        hitbox hitbox = col.gameObject.GetComponent<hitbox>();
        if (hitbox!=null)
        {
            if (hitbox.canDie)
            {
                Destroy(col.gameObject);
            }
        }
    }
}
