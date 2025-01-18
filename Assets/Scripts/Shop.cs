using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<player> players;
    public bool shopOpened = false;
    public GameObject stats;

    PlayerData playerData;


    bool hasPlayer = false;
    player phonePlayer;
    bool onPhone = false;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        players = new List<player>();
        playerData = FindObjectOfType<PlayerData>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
           if(player.bot == false)
            {
                players.Add(player);
                player.HighlightInteraction();
                hasPlayer = true;
                phonePlayer = player;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
            if(players.Contains(player))
            {
                players.Remove(player);
                player.UnhighlightInteraction();
                hasPlayer = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (shopOpened == false)
        {
            if (onPhone == false)
            {
                foreach (player player in players)
                {
                    if (playerData.hasFocus && (player.gamePad != null ? (player.gamePad.A == 1 && player.playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && player.playerNumber == 5 && player.keyboard) || (player.isInteracting() && player.playerNumber == 5 && player.keyboard == false))
                    {
                        OpenShop(player);
                        break;
                    }
                }
            }
            else if (hasPlayer)
            {
                if (playerData.hasFocus && phonePlayer.isInteracting())
                {
                    OpenShop(phonePlayer);
                }
            }
        }
    }

    private void OpenShop(player player)
    {
        shopOpened = true;
        stats.GetComponent<statShop>().InitShop(player.hp, player.reg, player.spd, player.dex, player.str, player.cdr, player.pointsLeft, player.pointsAvailable, player);
        player.ableToMove = false;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        stats.SetActive(true);
    }
}
