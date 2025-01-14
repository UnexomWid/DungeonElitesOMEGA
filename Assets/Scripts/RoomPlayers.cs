using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayers : MonoBehaviour
{
    public List<player> players;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (players.Contains(player) == false)
            players.Add(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (players.Contains(player))
            players.Remove(player);
    }
}
