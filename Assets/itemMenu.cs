using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemMenu : MonoBehaviour
{
    public Sprite[] itemSprites;
    public int itemType;
    public SpriteRenderer itemRenderer;
    public Sprite[] menuSprites;
    public int playersNumber;

    bool onPhone = false;

    public void SetUp(int itemType)
    {
        this.itemType = itemType;
        itemRenderer.sprite = itemSprites[itemType];
        if (itemType == 10)
        {
            GetComponent<SpriteRenderer>().sprite = menuSprites[6];
            price = 200;
        }
    }
    int price = 300;
    
    public List<player> players;
    public bool shopOpened = true;
    public GameObject stats;

    InventorySpawn inventorySpawn;
    PlayerData playerData;
    private void Start()
    {

#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        playerData = FindObjectOfType<PlayerData>();
        inventorySpawn = FindObjectOfType<InventorySpawn>();
        playersNumber = 1;
        GetComponent<SpriteRenderer>().sprite = menuSprites[playersNumber - 1];
        if (itemType == 10)
            GetComponent<SpriteRenderer>().sprite = menuSprites[6];
        players = new List<player>();
        if (Mathf.Abs(transform.eulerAngles.z) > 179)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }

    bool hasPlayer = false;
    player phonePlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
            if (player.bot == false)
            {
                players.Add(player);
                player.HighlightInteraction(this);
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
            if (players.Contains(player))
            {
                players.Remove(player);
                player.UnhighlightInteraction(this);
                hasPlayer = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (PublicVariables.TimeScale > 0.05f)
        {
            if (shopOpened)
            {
                if (onPhone == false)
                {
                    foreach (player player in players)
                    {
                        if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.A == 1 && player.playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && player.playerNumber == 5 && player.keyboard) || (player.isInteracting() && player.playerNumber == 5 && player.keyboard == false)) && inventorySpawn.coins >= price * playersNumber)
                        {
                            Buy(player);
                            break;
                        }
                    }
                }
                else if(hasPlayer)
                {
                    if (playerData.hasFocus && phonePlayer.isInteracting() && inventorySpawn.coins >= price * playersNumber)
                    {
                        Buy(phonePlayer);
                    }
                }
            }
        }
    }

    private void Buy(player player)
    {
        bool ok = inventorySpawn.AddItem(itemType < 8 ? itemType : (itemType + 2));
        if (ok)
        {
            player.SpawnPurchaseVfx();
            inventorySpawn.coins -= price * playersNumber;
            shopOpened = false;
            itemRenderer.enabled = false;

            if (TutorialScene.instance != null)
            {
                if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                {
                    Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 5)
                    {
                        tutorial.Disable();
                    }
                }
            }
            else DungeonData.instance.AddToQuest(28, 1);
        }
    }
}

