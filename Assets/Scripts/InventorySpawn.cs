using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventorySpawn : MonoBehaviour
{
    public GameObject questObj;
    public GameObject characterObj;
    public GameObject pause;
    public GameObject megamapCamera;
    public Inventory inventory;
    bool activated = false;
    public Text coinText;
    public int coins;

    public void AddCoins(int count)
    {
        count = OMEGA.Data.GetCoinIncome(count);

        coins += count;

        DungeonData.instance.AddToQuest(13, count);

        if (coins >= 750)
            DungeonData.instance.AddToQuest(12, 1);
    }

   public  GameObject megamap;
    GameObject minimap;

    public bool mega = false;

    PlayerData playerData;
    CameraFollow cameraFollow;

    GameObject music;
    TimeController timeController;

    arrivePad arrivePad;

    bool onPhone = false;

    private void OnEnable()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif
        try
        {
            playerData = FindObjectOfType<PlayerData>();
        }
        catch
        {

        }
        try
        {
            arrivePad = FindObjectOfType<arrivePad>();
        }
        catch
        {

        }
        try
        {
            timeController = FindObjectOfType<TimeController>();
        }
        catch
        {

        }

        try
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
        catch
        {

        }
        if (activated == false)
        {
            activated = true;

            inventory.SetUpCharacters();

            itemDescs[1] = "33% chance to drop a bomb when getting hit. When it explodes, deals <color=red><b>250</b></color> damage and <color=black><b>knockbacks</b></color> all nearby enemies.";
            itemDescs[3] = "20% more healing from ally <color=lime><b>heals</b></color>.";
 
            foreach (Transform child in FindObjectOfType<Canvas>().transform)
            {
                if (child.name.Contains("Megamap"))
                    megamap = child.gameObject;
                if (child.name.Contains("Image") || child.name.Contains("Minimap"))
                    minimap = child.gameObject;
            }

            foreach(Megamap child in FindObjectsOfType<Megamap>())
            {
                if (child.transform.name.Contains("Megamap"))
                    megamapCamera = child.gameObject;
            }
        }
        music = FindObjectOfType<Music>().gameObject;
        music.SetActive(false);
        Invoke("ActivateMinimap", 4f);
        //minimap.SetActive(false);
    }

    void ActivateMinimap()
    {
        music.SetActive(true);
        //minimap.SetActive(true);
    }
    bool canDo = true;
    bool pressed = false;
    private void Update()
    {
        bool ok = true;
        bool ok2 = true;
        bool ok3 = true;

        try
        {
            if (pauseOn || megamapOn || inventoryOn)
            {
                Time.timeScale = 0.000001f;
            }
        }
        catch
        {

        }

        if (onPhone == false)
        {
            foreach (player player in cameraFollow.playerScripts)
            {

                try
                {
                    if (player != null && (arrivePad != null ? arrivePad.input : true))
                    {
                        if (player.bot == false)
                        {
                            if (playerData.hasFocus && (questObj != null ? questObj.activeInHierarchy == false : true) && (characterObj != null ? characterObj.activeInHierarchy == false : true) && megamap.activeInHierarchy == false && ((player.gamePad != null ? (player.gamePad.rightStickPress == 1 && player.playerNumber != 5) : false) || ((Input.GetKey(KeyCode.I) && player.playerNumber == 5))))
                            {
                                ok = false;
                                if (canDo)
                                {
                                    canDo = false;
                                    TriggerInventory();
                                }
                            }
                            if (playerData.hasFocus && (questObj != null ? questObj.activeInHierarchy == false : true) && (characterObj != null ? characterObj.activeInHierarchy == false : true) && ((player.gamePad != null ? (player.gamePad.back == 1 && player.playerNumber != 5) : false) || ((Input.GetKey(KeyCode.M) && player.playerNumber == 5))))
                            {
                                ok = false;
                                if (inventory.gameObject.activeInHierarchy == false && pressed == false)
                                {
                                    pressed = true;
                                    TriggerMegamap();
                                }
                            }
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.start == 1 && player.playerNumber != 5) : false) || ((Input.GetKeyDown(KeyCode.Escape) && player.playerNumber == 5))))
                            {
                                //if (FindObjectOfType<Tutorial>() == null)
                                //{
                                ok3 = false;
                                if (inventory.gameObject.activeInHierarchy == false && (questObj != null ? questObj.activeInHierarchy == false : true) && (characterObj != null ? characterObj.activeInHierarchy == false : true) && megamap.gameObject.activeInHierarchy == false && pressed2 == false && player.ableToMove)
                                {
                                    pressed2 = true;
                                    if (!(pause.gameObject.activeInHierarchy == true))
                                    {
                                        Time.timeScale = 0.000001f;
                                    }
                                    else Time.timeScale = timeController.currentTime;
                                    pause.SetActive(!pause.activeInHierarchy);
                                }
                                /*}
                                else if(FindObjectOfType<Tutorial>().tutorialObj.activeInHierarchy == false )
                                {
                                    ok3 = false;
                                    if (inventory.gameObject.activeInHierarchy == false && megamap.gameObject.activeInHierarchy == false && pressed2 == false && player.ableToMove)
                                    {
                                        pressed2 = true;
                                        TriggerPause();
                                    }
                                }*/
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            if (ok)
                canDo = true;
            if (ok)
                pressed = false;
            if (ok3)
                pressed2 = false;
        }
    }

    public void CloseAll()
    {
        if (inventory.gameObject.activeInHierarchy)
            TriggerInventory();
        if (megamap.gameObject.activeInHierarchy)
            TriggerMegamap();
    }

    bool inventoryOn = false;
    bool megamapOn = false;
    bool pauseOn = false;
    public void TriggerInventory()
    {
        if (!(inventory.gameObject.activeInHierarchy == true))
            Time.timeScale = 0.000001f;
        else Time.timeScale = timeController.currentTime;
        inventory.gameObject.SetActive(!inventory.gameObject.activeInHierarchy);

        inventoryOn = inventory.gameObject.activeInHierarchy;

        coinText.text = coins.ToString();
        inventory.GetComponent<Inventory>().Enable();

        if (TutorialScene.instance != null)
        {
            if (TutorialScene.instance.inventory.gameObject.activeInHierarchy)
            {
                Tutorial tutorial = TutorialScene.instance.inventory.GetComponentInChildren<Tutorial>();
                if (tutorial.textIndex == 1 || tutorial.textIndex == 4)
                    tutorial.NextQuote();
            }
        }
    }

    public void TriggerMegamap()
    {
        if (!(megamap.gameObject.activeInHierarchy == true))
            Time.timeScale = 0.000001f;
        else Time.timeScale = 1;
        mega = !mega;
        megamap.SetActive(mega);
        megamapCamera.SetActive(megamap.activeInHierarchy);

        megamapOn = mega;

        minimap.SetActive(!mega);

        if (TutorialScene.instance != null)
        {
            if (TutorialScene.instance.megamap.gameObject.activeInHierarchy)
            {
                Tutorial tutorial = TutorialScene.instance.megamap.GetComponentInChildren<Tutorial>();
                if (tutorial.textIndex == 1)
                    tutorial.NextQuote();
                else if (tutorial.textIndex == 2)
                    tutorial.Disable();
            }
        }
    }

    public void TriggerPause()
    {
        if (!(pause.gameObject.activeInHierarchy == true))
            Time.timeScale = 0.000001f;
        else Time.timeScale = timeController.currentTime;
        pause.SetActive(!pause.activeInHierarchy);

        pauseOn = pause.activeInHierarchy;
    }

    bool pressed2 = false;
    public SelectableItem[] itemSlots;
    public Sprite[] itemSprites;
    public string[] itemDescs;
    public bool AddItem(int type)
    {
        bool ok = false;
        if (type == -1)
            return false;
        foreach (SelectableItem itemSlot in itemSlots)
        {
            if (itemSlot.containsItem == false)
            {
                ok = true;
                if (type >= 7 && type <= 9)
                    itemSlot.item.sprite = itemSprites[7];
                else if (type > 9)
                    itemSlot.item.sprite = itemSprites[type - 2];
                else itemSlot.item.sprite = itemSprites[type];
                itemSlot.item.gameObject.SetActive(true);
                itemSlot.isAbility = true;

                AbilityDesc itemSlotAbilityDesc = itemSlot.abilityDescription.GetComponentInParent<AbilityDesc>();
                if (type >= 7 && type <= 9)
                    itemSlotAbilityDesc.abilityText.text = itemDescs[7] + " Remaining uses: <color=red><b>" + (10 - type) + "</b></color>.";
                else if (type > 9)
                    itemSlotAbilityDesc.abilityText.text = itemDescs[type - 2];
                else itemSlotAbilityDesc.abilityText.text = itemDescs[type];
                if (type >= 7 && type <= 9)
                    itemSlot.itemType = 7;
                else if(type > 9)
                    itemSlot.itemType = type - 2;
                else itemSlot.itemType = type;
                itemSlot.containsItem = true;
                if (type >= 7 && type <= 9)
                    itemSlot.usesLeft = 10 - type;
                if (type == 6)
                {
                    foreach (player player in cameraFollow.playerScripts)
                    {
                        if (player != null)
                        {
                            if (player.bot == false)
                            {
                                player.StatBoost();
                            }
                        }
                    }
                }
                break;
            }
        }
        return ok;
    }

}
