using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public CharacterSlot[] slots;
    public Sprite[] characterPics;
    public Sprite[] abillityBasics;
    public Image basicButton;
    public Image[] abillityButtons;

    PlayerData[] playerDatas;

    public void SetUpCharacters()
    {
        int slotIndex = 0;

        playerDatas = FindObjectsOfType<PlayerData>();

        player[] players = FindObjectsOfType<player>();

        foreach (player player in players)
        {
            try
            {
                if (player.bot == false)
                {
                    foreach (PlayerData data in playerDatas)
                    {
                        if (data.idCaracter == player.caracterId)
                        {
                            basicButton.sprite = abillityBasics[data.idCaracter - 1];

                            CharacterSlot slot = slots[slotIndex++];
                            slot.characterPic.sprite = characterPics[player.caracterId - 1];
                            if (player.caracterId == 1)
                                slot.characterPic.color = new Color32(0, 18, 255, 255);
                            slot.ability1.sprite = player.abilityImages[PlayerPrefs.GetInt("SavedAbility"+player.caracterId+"1",1)-1];

                            abillityButtons[0].sprite = slot.ability1.sprite;

                            slot.ability2.sprite = player.abilityImages[PlayerPrefs.GetInt("SavedAbility" + player.caracterId + "2",2)-1];

                            abillityButtons[1].sprite = slot.ability2.sprite;

                            slot.ability3.sprite = player.abilityImages[PlayerPrefs.GetInt("SavedAbility" + player.caracterId + "3",3)-1];

                            abillityButtons[2].sprite = slot.ability3.sprite;

                            SelectableItem selectableItem1 = slot.ability1.GetComponent<SelectableItem>();
                            SelectableItem selectableItem2 = slot.ability2.GetComponent<SelectableItem>();
                            SelectableItem selectableItem3 = slot.ability3.GetComponent<SelectableItem>();
                            selectableItem1.player = player;
                            selectableItem2.player = player;
                            selectableItem3.player = player;
                            selectableItem1.abilNumber = 1;
                            selectableItem2.abilNumber = 2;
                            selectableItem3.abilNumber = 3;
                            AbilityDesc[] descs = new AbilityDesc[3];
                            descs[0] = selectableItem1.abilityDescription.GetComponentInParent<AbilityDesc>();
                            descs[1] = selectableItem2.abilityDescription.GetComponentInParent<AbilityDesc>();
                            descs[2] = selectableItem3.abilityDescription.GetComponentInParent<AbilityDesc>();
                            player.abilityDescs = descs;
                            slot.healthBar.transform.localScale = new Vector3(1, 1, 1);
                            slot.characterHp.text = player.maxHealth + "/" + player.maxHealth;
                            slot.characterId = player.caracterId;
                            slot.gameObject.SetActive(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        foreach (player player in players)
        {
            try
            {
                if (player.bot == false)
                {
                    player.GetSlots(this);
                }
            }
            catch
            {

            }
        }
    }
    public void Enable()
    {
        int slotIndex = 0;
        List<player> players = FindObjectsOfType<player>().ToList();
        foreach (player player in FindObjectOfType<DeadMates>().players)
            players.Add(player);
        foreach (player player in players)
        {
            try
            {
                if (player.bot == false)
                {
                    bool ok = false;
                    CharacterSlot playerSlot = null;
                    foreach (CharacterSlot slot in slots)
                    {
                        if (slot.characterId == player.caracterId)
                        {
                            playerSlot = slot;
                            ok = true;
                            break;
                        }
                    }
                    if (ok)
                    {
                        if (player.health < 0)
                            player.health = 0;
                        playerSlot.healthBar.transform.localScale = new Vector3(player.health / player.maxHealth, playerSlot.healthBar.transform.localScale.y, 1);
                        playerSlot.characterHp.text = (int)player.health + "/" + player.maxHealth;
                        playerSlot.xpBar.transform.localScale = new Vector3(player.xp / (150 + player.lvl * 50), playerSlot.xpBar.transform.localScale.y, 1);
                        playerSlot.characterXp.text = (int)player.xp + "/" + (150 + player.lvl * 50);
                        playerSlot.characterLevel.text = player.lvl.ToString();
                        playerSlot.APText.text = "AP: " + player.abilityPoints;
                        playerSlot.MPText.text = "MP: " + player.masterPoints;
                    }
                    else
                    {
                        playerSlot.healthBar.transform.localScale = new Vector3(0, playerSlot.healthBar.transform.localScale.y, 1);
                        playerSlot.characterHp.text = playerSlot.characterHp.text.Split('/')[1] + "/" + player.maxHealth;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
}
