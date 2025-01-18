using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class statShop : MonoBehaviour
{
    public GameObject parent;
    public player player;
    public GameObject[] bars;
    public GameObject[] stars;
    public Sprite[] barTextures;
    public Sprite starSelect;
    public Sprite starDeselect;
    public int selectedStar = 0;
    public TextMeshPro price;
    public GameObject pointsLeft;
    public GameObject pointsAvailable;
    public Sprite[] numbers;
    bool openedShop = false;
    player playerScript;
    Rigidbody2D playerRb;
    Animator playerAnimator;
    PlayerData playerData;
    public void InitShop(int hp, int reg, int spd, int dex, int str, int cdr, int pointsLeft, int pointsAvailable, player player)
    {
        playerScript = player;
        playerRb = player.GetComponent<Rigidbody2D>();
        playerAnimator = player.GetComponent<Animator>();
        playerData = FindObjectOfType<PlayerData>();
        int[] values = { hp, reg, spd, dex, str, cdr };
        this.player = player;
        openedShop = true;
        selectedStar = 0;
        foreach(GameObject star in stars)
        {
            star.GetComponent<SpriteRenderer>().sprite = starDeselect;
        }
        stars[0].GetComponent<SpriteRenderer>().sprite = starSelect;
        for(int i=0;i<values.Length;i++)
        {
            bars[i].GetComponent<SpriteRenderer>().sprite = barTextures[values[i]];
        }
        this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[pointsLeft];
        this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[pointsAvailable];
        pressedButton = true;
        player.ActivateReturn();

        price.text = OMEGA.Data.GetStatShopCost(player).ToString();

        if (TutorialScene.instance != null)
        {
            if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
            {
                Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                if (tutorial.textIndex == 1)
                    tutorial.NextQuote();
            }
        }
    }
    bool canDo = true;
    void CanDo()
    {
        canDo = true;
    }
    bool pressedButton = false;
    void Update()
    {
        if (PublicVariables.TimeScale > 0.05f)
        {
            if (openedShop)
            {
                playerRb.velocity = new Vector2(0, 0);
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("run"))
                    playerAnimator.Play("idle");
                if (playerScript.cursorTarget != null)
                {

                    playerScript.followTarget = false;
                }
                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.y < -0.5 && player.playerNumber != 5) : false) || (Input.GetKey(KeyCode.S) && player.playerNumber == 5 && player.keyboard) || (player.phoneController != null && player.phoneController.dir.y < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && canDo)
                {
                    stars[selectedStar].GetComponent<SpriteRenderer>().sprite = starDeselect;
                    selectedStar++;
                    if (selectedStar >= stars.Length)
                        selectedStar = stars.Length - 1;
                    stars[selectedStar].GetComponent<SpriteRenderer>().sprite = starSelect;
                    canDo = false;
                    Invoke("CanDo", 0.25f);
                }
                else if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.y > 0.5 && player.playerNumber != 5) : false) || (Input.GetKey(KeyCode.W) && player.playerNumber == 5 && player.keyboard) || (player.phoneController != null && player.phoneController.dir.y > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && canDo)
                {
                    stars[selectedStar].GetComponent<SpriteRenderer>().sprite = starDeselect;
                    selectedStar--;
                    if (selectedStar < 0)
                        selectedStar = 0;
                    stars[selectedStar].GetComponent<SpriteRenderer>().sprite = starSelect;
                    canDo = false;
                    Invoke("CanDo", 0.25f);
                }

                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.A == 1 && player.playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && player.playerNumber == 5 && player.keyboard) || (player.isInteracting() && player.playerNumber == 5 && player.keyboard == false)))
                {
                    if (pressedButton == false)
                    {
                        var cost = OMEGA.Data.GetStatShopCost(player);

                        if (selectedStar == 0 && FindObjectOfType<InventorySpawn>().coins >= cost)
                        {
                            if (player.pointsAvailable > 0)
                            {
                                player.SpawnPurchaseVfx();
                                player.pointsAvailable--;
                                player.pointsLeft++;
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];
                                FindObjectOfType<InventorySpawn>().coins -= cost;

                                price.text = OMEGA.Data.GetStatShopCost(player).ToString();

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 2)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                        }
                        else if (player.pointsLeft > 0)
                        {
                            var limit = OMEGA.Data.GetStatShopMax();

                            if (selectedStar == 1 && player.hp < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewHp(true);
                                bars[0].GetComponent<SpriteRenderer>().sprite = barTextures[player.hp];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (selectedStar == 2 && player.reg < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewReg(true);
                                bars[1].GetComponent<SpriteRenderer>().sprite = barTextures[player.reg];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (selectedStar == 3 && player.spd < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewSPeed(true);
                                bars[2].GetComponent<SpriteRenderer>().sprite = barTextures[player.spd];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (selectedStar == 4 && player.dex < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewDex(true);
                                bars[3].GetComponent<SpriteRenderer>().sprite = barTextures[player.dex];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (selectedStar == 5 && player.str < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewStr(true);
                                bars[4].GetComponent<SpriteRenderer>().sprite = barTextures[player.str];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (selectedStar == 6 && player.cdr < limit)
                            {
                                player.SpawnPurchaseVfx();
                                player.SetNewCdr(true);
                                bars[5].GetComponent<SpriteRenderer>().sprite = barTextures[player.cdr];
                                this.pointsLeft.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsLeft];
                                this.pointsAvailable.GetComponent<SpriteRenderer>().sprite = numbers[player.pointsAvailable];

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 3)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                        }
                    }
                    pressedButton = true;
                }
                else pressedButton = false;
                if (playerData.hasFocus && (player.gamePad != null ? (player.gamePad.leftBumper == 1 && player.playerNumber != 5) : false && FindObjectOfType<InventorySpawn>().megamap.activeInHierarchy == false) || (Input.GetKey(KeyCode.Escape) && player.playerNumber == 5 && player.keyboard) || (player.isReturning() && player.playerNumber == 5 && player.keyboard == false))
                {
                    openedShop = false;
                    player.StopReturn();
                    parent.GetComponent<Shop>().shopOpened = false;
                    player.ableToMove = true;
                    gameObject.SetActive(false);

                    if (TutorialScene.instance != null)
                    {
                        if (TutorialScene.instance.interact.gameObject.activeInHierarchy)
                        {
                            Tutorial tutorial = TutorialScene.instance.interact.GetComponentInChildren<Tutorial>();
                            if (tutorial.textIndex == 4)
                            {
                                tutorial.NextQuote();

                                foreach (itemMenu menu in FindObjectsOfType<itemMenu>())
                                    menu.GetComponent<BoxCollider2D>().enabled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
