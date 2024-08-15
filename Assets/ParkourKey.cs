using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourKey : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
            if (player != null)
            {
                if (player.bot == false)
                    Destroy(gameObject);
            }
        
    }
}
