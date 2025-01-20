using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class arrivePad : MonoBehaviour
{
    public Color32[] colors;
    public Image[] maps;
    public GameObject arrow;
    public GameObject map;
    public AudioClip arrive;
    public bool input = false;


    GameObject minimap;
    GameObject touchControls;
    InventorySpawn inventorySpawn;
    DungeonData dungeonData;
    // Start is called before the first frame update
    void Awake()
    {
        inventorySpawn = FindObjectOfType<InventorySpawn>();
        dungeonData = FindObjectOfType<DungeonData>();
        Input.multiTouchEnabled = true;

            for (int i = 0; i < maps.Length; i++)
            {
                maps[i].color = colors[dungeonData.maps[i] - 1];
            }

        RectTransform arrowRect = arrow.GetComponent<RectTransform>();

        arrowRect.position = new Vector2(arrowRect.position.x + 320 * (dungeonData.currentMap - 1), arrowRect.position.y);

        if (dungeonData.restarted)
        {
            dungeonData.restarted = false;
            GetComponent<Animator>().Play("prearrive", 0, 0.985f); // Skip the first 1s of the animation (fade in)
            arrive = Resources.Load("Sounds\\arrive") as AudioClip;
        }
        else
        {
            GetComponent<Animator>().Play("prearrive");
        }

        try
        {
            touchControls = GameObject.Find("TouchControls");
            touchControls.SetActive(false);
        }
        catch (Exception ex)
        {
            
        }

        try
        {
            minimap = GameObject.Find("Minimap");
            minimap.SetActive(false);
        }
        catch
        {

        }
    }

    public void EndAnim()
    {
        foreach(player player in FindObjectsOfType<player>())
        {
            if(player.bot == false)
            {
                player.ableToMove = true;
            }
        }

        try
        {
            touchControls.SetActive(true);
        }
        catch
        {

        }

        try
        {
            minimap.SetActive(true);
        }
        catch
        {

        }

        input = true;

        if(dungeonData.currentMap > 1)
        DungeonData.instance.AddToQuest(4, 1);
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("Shops");
    }

    public void PlayArrive()
    {
        AudioSource.PlayClipAtPoint(arrive, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }

    void OnMapGenerated()
    {
        PrepareDungeon();
        OMEGA.Events.OnDungeonGenerated();
        GetComponent<Animator>().Play("arrive");
    }

    void LoadMap()
    {
        try
        {
           //Awake();
        }
        catch
        {

        }
        
        if (map != null)
        {
            map.SetActive(true);

            var generator = map.GetComponentInChildren<MapGeneration>();

            if (generator != null)
            {
                StartCoroutine(map.GetComponentInChildren<MapGeneration>().Generate(OnMapGenerated));
            }
            else
            {
                OnMapGenerated();
            }
        }
        else
        {
            PrepareDungeon();
        }
    }

    void PrepareDungeon()
    {
        foreach (SelectableItem item in inventorySpawn.itemSlots)
        {
            item.itemType = -1;
        }

        player[] players = FindObjectsOfType<player>();

        foreach (player player in players)
        {
            player.GetRooms();
            player.ResetPermPoints();
        }

        if (dungeonData.currentMap > 1)
        {

            for (int i = 0; i < dungeonData.items.Length; i++)
            {
                if (dungeonData.items[i] != -1)
                {
                    inventorySpawn.AddItem((OMEGA.Items.ID)dungeonData.items[i]);
                }
            }

            foreach (player player in players)
            {

                if (player.bot == false)
                {

                    for (int i = 0; i < dungeonData.statuses[player.caracterId].hp; i++)
                    {
                        player.SetNewHp(false);
                    }
                    for (int i = 0; i < dungeonData.statuses[player.caracterId].reg; i++)
                    {
                        player.SetNewReg(false);
                    }
                    for (int i = 0; i < dungeonData.statuses[player.caracterId].str; i++)
                    {
                        player.SetNewStr(false);
                    }
                    for (int i = 0; i < dungeonData.statuses[player.caracterId].cdr; i++)
                    {
                        player.SetNewCdr(false);
                    }
                    for (int i = 0; i < dungeonData.statuses[player.caracterId].dex; i++)
                    {

                        player.SetNewDex(false);
                    }
                    for (int i = 0; i < dungeonData.statuses[player.caracterId].spd; i++)
                    {
                        player.SetNewSPeed(false);
                    }

                    foreach (CharacterSlot slot in inventorySpawn.inventory.slots)
                    {
                        if (slot.characterId == player.caracterId)
                        {
                            SelectableItem abillity1Slot = slot.ability1.GetComponent<SelectableItem>();
                            SelectableItem abillity2Slot = slot.ability2.GetComponent<SelectableItem>();
                            SelectableItem abillity3Slot = slot.ability3.GetComponent<SelectableItem>();

                            abillity1Slot.lvl = dungeonData.abil1Lvl[player.caracterId];

                            if (abillity1Slot.lvl == 6)
                            {
                                abillity1Slot.level.text = "M";
                            }
                            else if (abillity1Slot.lvl == 7)
                            {
                                abillity1Slot.level.text = "P";
                            }
                            else
                            {
                                abillity1Slot.level.text = abillity1Slot.lvl.ToString();
                            }

                            abillity2Slot.lvl = dungeonData.abil2Lvl[player.caracterId];

                            if (abillity2Slot.lvl == 6)
                            {
                                abillity2Slot.level.text = "M";
                            }
                            else if (abillity2Slot.lvl == 7)
                            {
                                abillity2Slot.level.text = "P";
                            }
                            else
                            {
                                abillity2Slot.level.text = abillity2Slot.lvl.ToString();
                            }

                            abillity3Slot.lvl = dungeonData.abil3Lvl[player.caracterId];

                            if (abillity3Slot.lvl == 6)
                            {
                                abillity3Slot.level.text = "M";
                            }
                            else if (abillity3Slot.lvl == 7)
                            {
                                abillity3Slot.level.text = "P";
                            }
                            else
                            {
                                abillity3Slot.level.text = abillity3Slot.lvl.ToString();
                            }


                            for (int i = 1; i < abillity1Slot.lvl; i++)
                            {
                                if (i == 6)
                                {
                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(0, true);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(0, true);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(0, true);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(0, true);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(0, true);
                                    }
                                }
                                else
                                {

                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(0, false);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(0, false);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(0, false);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(0, false);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(0, false);
                                    }

                                }
                            }

                            for (int i = 1; i < abillity2Slot.lvl; i++)
                            {
                                if (i == 6)
                                {
                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(1, true);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(1, true);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(1, true);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(1, true);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(1, true);
                                    }
                                }
                                else
                                {

                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(1, false);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(1, false);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(1, false);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(1, false);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(1, false);
                                    }

                                }
                            }

                            for (int i = 1; i < abillity3Slot.lvl; i++)
                            {
                                if (i == 6)
                                {
                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(2, true);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(2, true);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(2, true);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(2, true);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(2, true);
                                    }
                                }
                                else
                                {

                                    if (player.wizard != null)
                                    {
                                        player.wizard.LevelAbil(2, false);
                                    }
                                    if (player.knight != null)
                                    {
                                        player.knight.LevelAbil(2, false);
                                    }
                                    if (player.supportClass != null)
                                    {
                                        player.supportClass.LevelAbil(2, false);
                                    }
                                    if (player.tank != null)
                                    {
                                        player.tank.LevelAbil(2, false);
                                    }
                                    if (player.archer != null)
                                    {
                                        player.archer.LevelAbil(2, false);
                                    }

                                }
                            }



                            break;
                        }
                    }

                    player.pointsLeft = dungeonData.statuses[player.caracterId].pointsLeft;
                    player.xp = dungeonData.xp[player.caracterId];
                    player.lvl = dungeonData.lvl[player.caracterId];

                    player.abilityPoints = dungeonData.statuses[player.caracterId].abilityPoints;
                    player.masterPoints = dungeonData.statuses[player.caracterId].masterPoints;

                    inventorySpawn.coins = dungeonData.dungeonCoins;

                    inventorySpawn.coinText.text = inventorySpawn.coins.ToString();
                }
            }
        }

        foreach (room room in FindObjectsOfType<room>())
            room.GetRooms();
    }
}
