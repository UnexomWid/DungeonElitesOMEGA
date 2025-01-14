using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDetect : MonoBehaviour
{
    public GameObject player;

    CircleCollider2D collider;
    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.GetComponent<player>();
        if (player != null)
        {
            if (player == null && player.bot == false)
                this.player = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player = null;

            collider.enabled = false;
            collider.enabled = true;
        }
    }
}
