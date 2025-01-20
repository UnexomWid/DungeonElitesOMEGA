using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class itemMenu : MonoBehaviour
{
    public TextMeshPro priceText;
    public Sprite[] itemSprites;
    public OMEGA.Items.ID itemType;
    public SpriteRenderer itemRenderer;
    public Sprite[] menuSprites; // TODO: Add mythic items with a purple menu sprite
    public int playersNumber;

    bool onPhone = false;

    public void SetUp(OMEGA.Items.ID itemType)
    {
        this.itemType = itemType;
        itemRenderer.sprite = itemSprites[(int)itemType];
        itemRenderer.enabled = true;

        saleInfo = OMEGA.Items.GenerateSaleInfo();

        UpdatePrice();

        shopOpened = true;

        //GetComponent<SpriteRenderer>().sprite = menuSprites[6];
    }

    public void UpdatePrice()
    {
        price = OMEGA.Items.GetPrice(itemType, boughtAmount);
        price = (int)Mathf.Floor(price * saleInfo.priceFactor);
        priceText.text = price.ToString();
        priceText.color = saleInfo.textColor;
    }

    OMEGA.Items.SaleInfo saleInfo;

    int price;
    int boughtAmount = 0;
    
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
        //GetComponent<SpriteRenderer>().sprite = menuSprites[playersNumber - 1];

        players = new List<player>();
        if (Mathf.Abs(transform.eulerAngles.z) > 179)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }

    bool hasPlayer = false;
    bool pressedButton = false;
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
                            // TODO: This also restricts button presses for other players, fix it
                            if (!pressedButton)
                            {
                                Buy(player);
                            }

                            pressedButton = true;
                            break;
                        }
                        else
                        {
                            pressedButton = false;
                        }
                    }
                }
                else if(hasPlayer)
                {
                    if (playerData.hasFocus && phonePlayer.isInteracting() && inventorySpawn.coins >= price * playersNumber)
                    {
                        if (!pressedButton)
                        {
                            Buy(phonePlayer);
                        }

                        pressedButton = true;
                    }
                    else
                    {
                        pressedButton = false;
                    }
                }
            }
        }
    }

    private void Buy(player player)
    {
        bool ok = inventorySpawn.AddItem((int)itemType < 8 ? itemType : (OMEGA.Items.ID)((int)itemType + 2));
        if (ok)
        {
            player.SpawnPurchaseVfx();
            inventorySpawn.coins -= price * playersNumber;
            shopOpened = false;
            itemRenderer.enabled = false;
            boughtAmount++;

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

            OMEGA.Events.OnBuyItem(this);
        }
    }
}

