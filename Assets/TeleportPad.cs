using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPad : MonoBehaviour
{

    float t;
    bool hasParent = false;

    public AudioClip teleport;

    room parentRoom;
    LockRoom parentLock;

    InventorySpawn inventorySpawn;
    DungeonData dungeonData;
    Animator animator;
    CameraFollow cameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        teleport = Resources.Load("Sounds\\teleport") as AudioClip;
        cameraFollow = FindObjectOfType<CameraFollow>();
        animator = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "Shops")
            animator.Play("shopStart");
        parentRoom = GetComponentInParent<room>();
        inventorySpawn = FindObjectOfType<InventorySpawn>();
        dungeonData = FindObjectOfType<DungeonData>();
        if(GetComponentInParent<thisisaroom>() != null)
        {
            hasParent = true;
            parentLock = GetComponentInParent<LockRoom>();
        }
    }

    public void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            SceneManager.LoadScene("Shops");
            PlayerPrefs.SetInt("Tutorial", 0);
        }
        else dungeonData.NextMap();
    }

    float time;

    // Update is called once per frame
    void Update()
    {
        time += PublicVariables.deltaTime;
        if (time >= 1)
        {
            time = 0;
            if (hasParent == false || (hasParent && parentRoom.lockable == false))
            {
                List<player> players = cameraFollow.playerScripts;
                bool ok = true;
                bool ok2 = false;
                foreach (player player in players)
                {
                    if (player != null)
                    {
                        ok2 = true;
                        if (Vector2.Distance(player.transform.position, transform.position) > 4f && player.finalBoss == false)
                        {
                            ok = false;
                        }
                    }
                }
                if (ok2)
                {
                    if (hasParent)
                    {
                        if (parentLock.enabled)
                        {
                            ok = false;
                        }
                    }
                    if (ok)
                    {
                        try
                        {
                            Destroy(FindObjectOfType<Music>().gameObject);
                        }
                        catch
                        {

                        }
                        if(SceneManager.GetActiveScene().name == "TutorialScene")
                        {
                            Destroy(dungeonData.gameObject);
                        }
                        else try
                        {
                            if (SceneManager.GetActiveScene().name == "Shops")
                            {
                                dungeonData.dungeonCoins = ShopsSettings.coins;
                                if (ShopsSettings.dungeonLevel > 0)
                                {
                                    dungeonData.currentMap = ShopsSettings.dungeonLevel;
                                    if (ShopsSettings.dungeonIndex > 0)
                                        dungeonData.GenerateMaps(ShopsSettings.dungeonIndex);
                                }
                                for (int i = 0; i <= 4; i++)
                                {
                                    try
                                    {
                                        dungeonData.lvl[cameraFollow.playerScripts[i].caracterId] = ShopsSettings.characterSettings[i].level;
                                        dungeonData.abil1Lvl[cameraFollow.playerScripts[i].caracterId] = ShopsSettings.characterSettings[i].a1level;
                                        dungeonData.abil2Lvl[cameraFollow.playerScripts[i].caracterId] = ShopsSettings.characterSettings[i].a2level;
                                        dungeonData.abil3Lvl[cameraFollow.playerScripts[i].caracterId] = ShopsSettings.characterSettings[i].a3level;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].hp = ShopsSettings.characterSettings[i].hp;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].reg = ShopsSettings.characterSettings[i].reg;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].spd = ShopsSettings.characterSettings[i].spd;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].dex = ShopsSettings.characterSettings[i].dex;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].str = ShopsSettings.characterSettings[i].str;
                                        dungeonData.statuses[cameraFollow.playerScripts[i].caracterId].cdr = ShopsSettings.characterSettings[i].cdr;
                                    }
                                    catch
                                    {

                                    }
                                }
                                for (int i = 0; i <= 9; i++)
                                {
                                    dungeonData.items[i] = ShopsSettings.items[i];
                                }
                            }
                        }
                        catch
                        {

                        }
                        t += 1;
                        if (t >= 1)
                        {
                            t = -999;
                            if (SceneManager.GetActiveScene().name != "TutorialScene")
                            {
                                if (dungeonData.currentMap >= 1)
                                {
                                    foreach (player player in players)
                                    {
                                        if (player != null)
                                        {
                                            dungeonData.attempt = 1;
                                            dungeonData.health[player.caracterId] = player.health;
                                            foreach (CharacterSlot slot in inventorySpawn.inventory.slots)
                                            {
                                                if (slot.characterId == player.caracterId)
                                                {
                                                    dungeonData.abil1Lvl[player.caracterId] = slot.ability1.GetComponent<SelectableItem>().lvl;
                                                    dungeonData.abil2Lvl[player.caracterId] = slot.ability2.GetComponent<SelectableItem>().lvl;
                                                    dungeonData.abil3Lvl[player.caracterId] = slot.ability3.GetComponent<SelectableItem>().lvl;
                                                    break;
                                                }
                                            }
                                            dungeonData.xp[player.caracterId] = player.xp;
                                            dungeonData.lvl[player.caracterId] = player.lvl;

                                            dungeonData.statuses[player.caracterId].hp = player.hp;
                                            dungeonData.statuses[player.caracterId].reg = player.reg;
                                            dungeonData.statuses[player.caracterId].str = player.str;
                                            dungeonData.statuses[player.caracterId].cdr = player.cdr;
                                            dungeonData.statuses[player.caracterId].spd = player.spd;
                                            dungeonData.statuses[player.caracterId].dex = player.dex;

                                            dungeonData.statuses[player.caracterId].abilityPoints = player.abilityPoints;
                                            dungeonData.statuses[player.caracterId].masterPoints = player.masterPoints;

                                            dungeonData.statuses[player.caracterId].pointsLeft = player.pointsLeft;

                                            dungeonData.dungeonCoins = inventorySpawn.coins;

                                            player.ableToMove = false;
                                        }
                                    }
                                    for (int i = 0; i < inventorySpawn.itemSlots.Length; i++)
                                    {
                                        if (inventorySpawn.itemSlots[i].itemType == 7 && inventorySpawn.itemSlots[i].usesLeft == 2)
                                            dungeonData.items[i] = 8;
                                        if (inventorySpawn.itemSlots[i].itemType == 7 && inventorySpawn.itemSlots[i].usesLeft == 1)
                                            dungeonData.items[i] = 9;

                                        if (inventorySpawn.itemSlots[i].itemType > 7)
                                            inventorySpawn.itemSlots[i].itemType += 2;

                                        dungeonData.items[i] = inventorySpawn.itemSlots[i].itemType;
                                    }
                                }
                            }

                            foreach(player player in players)
                            {
                                player.ableToMove = false;
                            }
                            cameraFollow.players.Clear();
                            cameraFollow.players.Add(gameObject);
                            animator.Play("teleport");
                            AudioSource.PlayClipAtPoint(teleport, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));

                            try
                            {
                                GameObject.Find("TouchControls").SetActive(false);
                            }
                            catch
                            {

                            }

                            try
                            {
                                GameObject.Find("Minimap").SetActive(false);
                            }
                            catch
                            {

                            }

                            if (SceneManager.GetActiveScene().name == "Shops")
                            {
                                string notification = "User started playing in the dungeon with:" + "\n\nSEPARATOR";

                                foreach(player player in players)
                                {
                                    notification += "\n";
                                    switch(player.playerNumber)
                                    {
                                        case 1:
                                            notification += "Wizard with the abillities: ";
                                            break;
                                        case 2:
                                            notification += "Knight with the abillities: ";
                                            break;
                                        case 3:
                                            notification += "Archer with the abillities: ";
                                            break;
                                        case 4:
                                            notification += "Tank with the abillities: ";
                                            break;
                                        case 5:
                                            notification += "Medic with the abillities: ";
                                            break;
                                    }

                                    notification += player.attacks[0] + " " + player.attacks[1] + " " +     player.attacks[2];

                                }

                                ServerNotification.instance.SendMessageToServer(notification);
                            }
                        }
                    }
                }
            }
        }
    }
}
