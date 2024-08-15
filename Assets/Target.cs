using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int playerNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
            if(player.playerNumber == playerNumber)
            {
                player.TargetReached();
            }
        }
    }
}
