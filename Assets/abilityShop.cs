using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class abilityShop : MonoBehaviour
{
    public int ability1, ability2, ability3;

    public Sprite[] priceSprites;
    public int abilityPrice;
    public GameObject price;
    public SpriteRenderer abilitySprite;
    public int caracterId;
    public List<int> boughtAbilities;
    public Sprite[] abilitySprites;

    public List<player> players;
    public bool shopOpened = false;
    public GameObject stats;
    //Joystick currentController;

    PlayerData playerData;

    SpriteRenderer priceSprite;

    DungeonData dungeonData;

    InventorySpawn inventorySpawn;

    MultiplatformTutorial tutorial;

    bool onPhone = false;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        inventorySpawn = FindObjectOfType<InventorySpawn>();
        dungeonData = FindObjectOfType<DungeonData>();
        priceSprite = price.GetComponent<SpriteRenderer>();
        playerData = FindObjectOfType<PlayerData>();
        tutorial = FindObjectOfType<MultiplatformTutorial>();

        players = new List<player>();
        boughtAbilities = new List<int>();

        PlayerPrefs.SetInt("Ability" + caracterId + "1", 1);
        PlayerPrefs.SetInt("Ability" + caracterId + "2", 1);
        PlayerPrefs.SetInt("Ability" + caracterId + "3", 1);

        ability1 = PlayerPrefs.GetInt("SavedAbility" + caracterId + "1", 1);
        ability2 = PlayerPrefs.GetInt("SavedAbility" + caracterId + "2", 2);
        ability3 = PlayerPrefs.GetInt("SavedAbility" + caracterId + "3", 3);

        if (ability1 - 1 == 0)
        {
            priceSprite.sprite = priceSprites[1];
            price.SetActive(true);
        }
        if (ability2 - 1 == 0)
        {
            priceSprite.sprite = priceSprites[2];
            price.SetActive(true);
        }

        if (ability3 - 1 == 0)
        {
            priceSprite.sprite = priceSprites[3];
            price.SetActive(true);
        }

        for (int i = 1; i <= 8; i++)
        {
            if (i <= 3)
                boughtAbilities.Add(1);
            else boughtAbilities.Add(PlayerPrefs.GetInt("Ability" + caracterId + "" + i, 0));
        }
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
            if (players.Contains(player))
            {
                players.Remove(player);
                player.UnhighlightInteraction();
                hasPlayer = false;
            }
        }
    }

    bool canDo = true;
    void CanDo()
    {
        canDo = true;
    }
    public bool pressedButton = false;
    public bool pressedButton2 = false;
    public bool pressedButton3 = false;
    public bool pressedButton4 = false;

    player currentPlayer;

    int currentIndex = 0;


    Rigidbody2D playerRb;
    Animator playerAnimator;
    player playerScript;

    ControllerInput gamePad;

    // Update is called once per frame
    void Update()
    {
        if (PublicVariables.TimeScale > 0.05f)
        {
            if (shopOpened == false)
            {
                if (onPhone == false)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (playerData.hasFocus && (players[i].gamePad != null ? (players[i].gamePad.A == 1 && players[i].playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && players[i].playerNumber == 5 && players[i].keyboard) || (players[i].isInteracting() && players[i].playerNumber == 5 && players[i].keyboard == false))
                        {
                            OpenShop(players[i]);

                            break;
                        }
                    }
                }
                else if(hasPlayer)
                {
                    if (playerData.hasFocus && phonePlayer.isInteracting())
                        OpenShop(phonePlayer);
                }
            }
            else
            {
                playerRb.velocity = new Vector2(0, 0);
                playerScript.UpdateState();
                if (playerScript.animState_IsRunning)
                    playerAnimator.Play("idle");
                if (playerScript.cursorTarget != null)
                {
                    playerScript.TargetReached();
                }

                if (onPhone == false)
                {
                    if (playerData.hasFocus && ((gamePad != null ? (gamePad.leftStick.x > 0.5 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.D) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.phoneController.dir.x > 0.5 && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false)) && canDo)
                    {
                        MoveLeft();
                    }
                    else if (playerData.hasFocus && ((gamePad != null ? (gamePad.leftStick.x < -0.5 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.A) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.phoneController.dir.x < -0.5 && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false)) && canDo)
                    {
                        MoveRight();
                    }
                    if (playerData.hasFocus && (gamePad != null ? (gamePad.A == 1 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.Q) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.abillity1.attack && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false))
                    {
                        AssignFirst();
                    }
                    else pressedButton = false;

                    if (playerData.hasFocus && (gamePad != null ? (gamePad.B == 1 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.W) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.abillity2.attack && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false))
                    {
                        AssignSecond();
                    }
                    else pressedButton2 = false;

                    if (playerData.hasFocus && (gamePad != null ? (gamePad.Y == 1 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.abillity3.attack && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false))
                    {
                        AssignThird();
                    }
                    else pressedButton3 = false;

                    if (playerData.hasFocus && (gamePad != null ? (gamePad.X == 1 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.isInteracting() && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false))
                    {
                        Buy();
                    }
                    else pressedButton4 = false;

                    if (playerData.hasFocus && (gamePad != null ? (gamePad.leftBumper == 1 && currentPlayer.playerNumber != 5) : false) || (Input.GetKey(KeyCode.Escape) && currentPlayer.playerNumber == 5 && currentPlayer.keyboard) || (currentPlayer.isReturning() && currentPlayer.playerNumber == 5 && currentPlayer.keyboard == false))
                    {
                        Exit();
                    }
                }
                else
                {
                    if (playerData.hasFocus && currentPlayer.phoneController.dir.x > 0.5 && canDo)
                    {
                        MoveLeft();
                    }
                    else if (playerData.hasFocus &&currentPlayer.phoneController.dir.x < -0.5 && canDo)
                    {
                        MoveRight();
                    }
                    if (playerData.hasFocus && currentPlayer.abillity1.attack)
                    {
                        AssignFirst();
                    }
                    else pressedButton = false;

                    if (playerData.hasFocus && currentPlayer.abillity2.attack)
                    {
                        AssignSecond();
                    }
                    else pressedButton2 = false;

                    if (playerData.hasFocus && currentPlayer.abillity3.attack)
                    {
                        AssignThird();
                    }
                    else pressedButton3 = false;

                    if (playerData.hasFocus && currentPlayer.isInteracting())
                    {
                        Buy();
                    }
                    else pressedButton4 = false;

                    if (playerData.hasFocus && currentPlayer.isReturning())
                    {
                        Exit();
                    }
                }
            }
        }
    }

    private void Exit()
    {
        shopOpened = false;
        currentPlayer.StopReturn();
        currentPlayer.ableToMove = true;
        currentPlayer.interactsAbillity = false;
        stats.SetActive(false);

        if (tutorial != null)
        {

            Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
            if (tutorial.textIndex == 4)
            {
                tutorial.Disable();
                PlayerPrefs.SetInt("ShopsTutorial", 0);
            }

        }
    }

    private void Buy()
    {
        if (pressedButton4 == false)
        {
            if (dungeonData.coins >= abilityPrice && boughtAbilities[currentIndex] == 0)
            {
                currentPlayer.SpawnPurchaseVfx();

                boughtAbilities[currentIndex] = 1;

                price.SetActive(false);

                PlayerPrefs.SetInt("Ability" + caracterId + "" + (currentIndex + 1), 1);

                dungeonData.RemoveCoins(abilityPrice);

                if (tutorial != null)
                {

                    Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 2)
                    {
                        tutorial.NextQuote();
                    }

                }
            }
        }
        pressedButton4 = true;
    }

    private void AssignThird()
    {
        if (pressedButton3 == false)
        {
            if (boughtAbilities[currentIndex] == 1)
            {
                if (!(ability2 - 1 != currentIndex && ability1 - 1 != currentIndex))
                {
                    if (ability2 - 1 == currentIndex)
                    {
                        ability2 = ability3;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "2", ability2);
                    }
                    else if (ability1 - 1 == currentIndex)
                    {
                        ability1 = ability3;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "1", ability1);
                    }
                }
                currentPlayer.SpawnPurchaseVfx();
                ability3 = currentIndex + 1;
                PlayerPrefs.SetInt("SavedAbility" + caracterId + "3", ability3);
                priceSprite.sprite = priceSprites[3];
                price.SetActive(true);
                foreach (player player in FindObjectsOfType<player>())
                {
                    if (player.caracterId == caracterId && player.finalBoss == false && player.bot == false)
                    {
                        player.ResetAbilities();
                        if (player.wizard != null)
                            player.wizard.SetUpText();
                        if (player.knight != null)
                            player.knight.SetUpText();
                        if (player.archer != null)
                            player.archer.SetUpText();
                        if (player.tank != null)
                            player.tank.SetUpText();
                        if (player.supportClass != null)
                            player.supportClass.SetUpText();
                    }
                }
                inventorySpawn.inventory.SetUpCharacters();

                if (tutorial != null)
                {

                    Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 3)
                    {
                        tutorial.NextQuote();
                    }

                }
            }
        }
        pressedButton3 = true;
    }

    private void AssignSecond()
    {
        if (pressedButton2 == false)
        {
            if (boughtAbilities[currentIndex] == 1)
            {
                if (!(ability1 - 1 != currentIndex && ability3 - 1 != currentIndex))
                {
                    if (ability1 - 1 == currentIndex)
                    {
                        ability1 = ability2;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "1", ability1);
                    }
                    else if (ability3 - 1 == currentIndex)
                    {
                        ability3 = ability2;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "3", ability3);
                    }
                }
                currentPlayer.SpawnPurchaseVfx();
                ability2 = currentIndex + 1;
                PlayerPrefs.SetInt("SavedAbility" + caracterId + "2", ability2);
                priceSprite.sprite = priceSprites[2];
                price.SetActive(true);
                foreach (player player in FindObjectsOfType<player>())
                {
                    if (player.caracterId == caracterId && player.finalBoss == false && player.bot == false)
                    {
                        player.ResetAbilities();
                        if (player.wizard != null)
                            player.wizard.SetUpText();
                        if (player.knight != null)
                            player.knight.SetUpText();
                        if (player.archer != null)
                            player.archer.SetUpText();
                        if (player.tank != null)
                            player.tank.SetUpText();
                        if (player.supportClass != null)
                            player.supportClass.SetUpText();
                    }
                }
                inventorySpawn.inventory.SetUpCharacters();

                if (tutorial != null)
                {

                    Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 3)
                    {
                        tutorial.NextQuote();
                    }

                }

            }
        }
        pressedButton2 = true;
    }

    private void AssignFirst()
    {
        if (pressedButton == false)
        {
            if (boughtAbilities[currentIndex] == 1)
            {
                if (!(ability2 - 1 != currentIndex && ability3 - 1 != currentIndex))
                {
                    if (ability2 - 1 == currentIndex)
                    {
                        ability2 = ability1;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "2", ability2);
                    }
                    else if (ability3 - 1 == currentIndex)
                    {
                        ability3 = ability1;
                        PlayerPrefs.SetInt("SavedAbility" + caracterId + "3", ability3);
                    }
                }
                currentPlayer.SpawnPurchaseVfx();
                ability1 = currentIndex + 1;
                PlayerPrefs.SetInt("SavedAbility" + caracterId + "1", ability1);
                priceSprite.sprite = priceSprites[1];
                price.SetActive(true);
                foreach (player player in FindObjectsOfType<player>())
                {
                    if (player.caracterId == caracterId && player.finalBoss == false && player.bot == false)
                    {
                        player.ResetAbilities();
                        if (player.wizard != null)
                            player.wizard.SetUpText();
                        if (player.knight != null)
                            player.knight.SetUpText();
                        if (player.archer != null)
                            player.archer.SetUpText();
                        if (player.tank != null)
                            player.tank.SetUpText();
                        if (player.supportClass != null)
                            player.supportClass.SetUpText();
                    }
                }
                inventorySpawn.inventory.SetUpCharacters();

                if (tutorial != null)
                {

                    Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
                    if (tutorial.textIndex == 3)
                    {
                        tutorial.NextQuote();
                    }

                }

            }
        }
        pressedButton = true;
    }

    private void MoveRight()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = 0;

        abilitySprite.sprite = abilitySprites[currentIndex];

        if (boughtAbilities[currentIndex] == 0)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[0];
        }
        else if (ability1 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[1];
        }
        else if (ability2 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[2];
        }
        else if (ability3 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[3];
        }
        else
            price.SetActive(false);

        canDo = false;
        Invoke("CanDo", 0.25f);
    }

    private void MoveLeft()
    {
        currentIndex++;
        if (currentIndex > 7)
            currentIndex = 7;
        canDo = false;

        abilitySprite.sprite = abilitySprites[currentIndex];

        if (boughtAbilities[currentIndex] == 0)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[0];
        }
        else if (ability1 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[1];
        }
        else if (ability2 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[2];
        }
        else if (ability3 - 1 == currentIndex)
        {
            price.SetActive(true);
            priceSprite.sprite = priceSprites[3];
        }
        else
            price.SetActive(false);

        Invoke("CanDo", 0.25f);
    }

    private void OpenShop(player player)
    {
        gamePad = player.gamePad;
        playerAnimator = player.GetComponent<Animator>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerScript = player;
        shopOpened = true;
        currentPlayer = player;
        player.ableToMove = false;
        player.interactsAbillity = true;
        player.ActivateReturn();
        stats.SetActive(true);
        pressedButton = true;
        pressedButton2 = true;
        pressedButton3 = true;
        pressedButton4 = true;

        if (tutorial != null)
        {
            Tutorial tutorial = this.tutorial.GetComponentInChildren<Tutorial>();
            if (tutorial.textIndex == 1)
            {
                tutorial.NextQuote();
            }
        }
    }
}
