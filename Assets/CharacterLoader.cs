using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLoader : MonoBehaviour
{
    [Header("Phone")]
    [SerializeField] Image phoneImage;
    [SerializeField] Text[] phoneTexts;
    [SerializeField] Text pointText;
    [Header("Pc")]
    [SerializeField] Image[] pcImages;
    [SerializeField] Text[] pcTexts;
    [SerializeField] Text[] pointTexts;
    [SerializeField] GameObject[] unlockScreens;
    [SerializeField] GameObject[] lockScreens;
    [SerializeField] CardController[] cardControllers;
    [Header("Other")]
    [SerializeField] Sprite[] characterPictures;
    CharacterStatus[] characterStatuses = new CharacterStatus[5];
    int[] cardCharacters = new int[5];
    [SerializeField] GameObject menu;
    int[] availablePoints = new int[5];

    public void ResetPoints(int cardId)
    {
        int characterId = 0;

        if (cardId == 5)
            characterId = cardCharacters[0];
        else characterId = cardCharacters[cardId];

        while (characterStatuses[characterId - 1].hp > 0)
        {
            Add(cardId, "hp", true);
        }
        while (characterStatuses[characterId - 1].reg > 0)
        {
            Add(cardId, "reg", true);
        }
        while (characterStatuses[characterId - 1].spd > 0)
        {
            Add(cardId, "spd", true);
        }
        while (characterStatuses[characterId - 1].dex > 0)
        {
            Add(cardId, "dex", true);
        }
        while (characterStatuses[characterId - 1].str > 0)
        {
            Add(cardId, "str", true);
        }
        while (characterStatuses[characterId - 1].cdr > 0)
        {
            Add(cardId, "cdr", true);
        }
    }

    public void PurchasePoint(int cardId)
    {
        int characterId = 0;

        if (cardId == 5)
            characterId = cardCharacters[0];
        else characterId = cardCharacters[cardId];

        if (DungeonData.instance.coins >= 10)
        {
            DungeonData.instance.RemoveCoins(10);

            availablePoints[characterId-1]++;

            ResetPointText();
        }
    }

    void ResetPointText()
    {
        for(int i=0;i<=4;i++)
        {
            PlayerPrefs.SetInt("PermPoints" + i, availablePoints[i]);
        }

        if(onPhone)
        {
            pointText.text = "Available Points: " + availablePoints[cardCharacters[0]-1].ToString();
        }
        else
        {
            for (int i = 0; i <= 4; i++)
            {

                if (cardCharacters[i] == 0)
                    continue;

                pointTexts[i].text = "Available Points: " + availablePoints[cardCharacters[i]-1].ToString();
            }
        }
    }

    public void ActivateMenu()
    {
        menu.SetActive(!menu.activeInHierarchy);
        Time.timeScale = menu.activeInHierarchy ? 0.000001f : 1;
    }

    public void Add(int cardId, string type, bool remove)
    {
        
        int characterId = 0;

        if (cardId == 5)
            characterId = cardCharacters[0];
        else characterId = cardCharacters[cardId];

        if (remove == false)
        {
            if (availablePoints[characterId-1] == 0)
                return;
            else availablePoints[characterId - 1]--;
        }

        switch (type)
        {
            case "hp":

                if (remove && characterStatuses[characterId - 1].hp <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if (characterStatuses[characterId - 1].hp > 0)
                        availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }

                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId-1].hp + (remove? -1f : 1f));
                break;
                case "reg":

                if (remove && characterStatuses[characterId - 1].reg <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if (characterStatuses[characterId - 1].reg > 0)
                        availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }
                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId - 1].reg + (remove? -1f : 1f));
                break;
                case "spd":

                if (remove && characterStatuses[characterId - 1].spd <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if (characterStatuses[characterId - 1].spd > 0)
                        availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }

                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId - 1].spd + (remove? -1f : 1f));
                break;
                case "dex":

                if (remove && characterStatuses[characterId - 1].dex <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if (characterStatuses[characterId - 1].dex > 0)
                        availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }

                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId - 1].dex + (remove? -1f : 1f));
                break;
                case "str":

                if (remove && characterStatuses[characterId - 1].str <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if (characterStatuses[characterId - 1].str > 0)
                        availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }

                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId - 1].str + (remove? -1f : 1f));
                break;
                case "cdr":

                if (remove && characterStatuses[characterId - 1].cdr <= 1.5f)
                {
                    PlayerPrefs.SetFloat("Character" + characterId + type, 0);
                    if(characterStatuses[characterId - 1].cdr > 0)
                    availablePoints[characterId - 1]++;
                    break;
                }
                else if (remove)
                {
                    availablePoints[characterId - 1]++;
                }

                PlayerPrefs.SetFloat("Character" + characterId + type, characterStatuses[characterId - 1].cdr + (remove? -1f : 1f));
                break;
        }

        ResetPointText();
        ResetCards();
    }


    CameraFollow cameraFollow;
    PlayerData playerData;

    private void Start()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
        playerData = FindObjectOfType<PlayerData>();

        for(int i=0;i<=4;i++)
        {
            availablePoints[i] = PlayerPrefs.GetInt("PermPoints" + i, 0);
        }

        ResetCards();
        ResetPointText();
    }
    bool onPhone = false;
    private void ResetCards()
    {
#if UNITY_ANDROID || UNITY_IPHONE
                    onPhone = true;
#endif
        for (int i = 0; i < 5; i++)
        {
            characterStatuses[i] = new CharacterStatus();
            characterStatuses[i].hp = PlayerPrefs.GetFloat("Character" + (i+1) + "hp", 0);
            characterStatuses[i].reg = PlayerPrefs.GetFloat("Character" + (i + 1) + "reg", 0);
            characterStatuses[i].spd = PlayerPrefs.GetFloat("Character" + (i + 1) + "spd", 0);
            characterStatuses[i].dex = PlayerPrefs.GetFloat("Character" + (i + 1) + "dex", 0);
            characterStatuses[i].str = PlayerPrefs.GetFloat("Character" + (i + 1) + "str", 0);
            characterStatuses[i].cdr = PlayerPrefs.GetFloat("Character" + (i + 1) + "cdr", 0);
        }

        PlayerData[] playerDatas = FindObjectsOfType<PlayerData>();

        player[] players = FindObjectsOfType<player>();

        if (onPhone)
        {
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
                                cardCharacters[0] = player.caracterId;

                                phoneImage.sprite = characterPictures[player.caracterId - 1];

                                phoneTexts[0].text = "HP: " + Math.Round(characterStatuses[player.caracterId - 1].hp, 1, MidpointRounding.ToEven) + "%";
                                phoneTexts[1].text = "REG: " + Math.Round(characterStatuses[player.caracterId - 1].reg, 1, MidpointRounding.ToEven) + "%";
                                phoneTexts[2].text = "SPD: " + Math.Round(characterStatuses[player.caracterId - 1].spd, 1, MidpointRounding.ToEven) + "%";
                                phoneTexts[3].text = "DEX: " + Math.Round(characterStatuses[player.caracterId - 1].dex, 1, MidpointRounding.ToEven) + "%";
                                phoneTexts[4].text = "STR: " + Math.Round(characterStatuses[player.caracterId - 1].str, 1, MidpointRounding.ToEven) + "%";
                                phoneTexts[5].text = "CDR: " + Math.Round(characterStatuses[player.caracterId - 1].cdr, 1, MidpointRounding.ToEven) + "%";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
        else
        {
            int cardIndex = 0;

            for(int i=0;i<=4;i++)
            {
                unlockScreens[i].SetActive(false);
                lockScreens[i].SetActive(true);
            }

            foreach (player player in players)
            {
                bool ok = false;

                try
                {
                    if (player.bot == false)
                    {
                        foreach (PlayerData data in playerDatas)
                        {
                            if (data.idCaracter == player.caracterId)
                            {
                                if (player.playerNumber != 5)
                                    cardControllers[cardIndex].player = player;

                                pcImages[cardIndex].sprite = characterPictures[player.caracterId - 1];

                                cardCharacters[cardIndex] = player.caracterId;

                                pcTexts[cardIndex * 6].text = "HP: " + Math.Round(characterStatuses[player.caracterId - 1].hp, 1, MidpointRounding.ToEven) + "%";
                                pcTexts[cardIndex * 6 + 1].text = "REG: " + Math.Round(characterStatuses[player.caracterId - 1].reg, 1, MidpointRounding.ToEven) + "%";
                                pcTexts[cardIndex * 6 + 2].text = "SPD: " + Math.Round(characterStatuses[player.caracterId - 1].spd, 1, MidpointRounding.ToEven) + "%";
                                pcTexts[cardIndex * 6 + 3].text = "DEX: " + Math.Round(characterStatuses[player.caracterId - 1].dex, 1, MidpointRounding.ToEven) + "%";
                                pcTexts[cardIndex * 6 + 4].text = "STR: " + Math.Round(characterStatuses[player.caracterId - 1].str, 1, MidpointRounding.ToEven) + "%";
                                pcTexts[cardIndex * 6 + 5].text = "CDR: " + Math.Round(characterStatuses[player.caracterId - 1].cdr, 1, MidpointRounding.ToEven) + "%";

                                cardIndex++;

                                ok = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }

                if(ok)
                {
                    unlockScreens[cardIndex - 1].SetActive(true);
                    lockScreens[cardIndex - 1].SetActive(false);
                }
            }
        }
    }

    bool canDo = true;
    private void Update()
    {
        bool ok = true;

        if (Time.timeScale > 0.05f || menu.activeInHierarchy)
        foreach (player player in cameraFollow.playerScripts)
        {
            try
            {
                if (player != null)
                {
                    if (player.bot == false)
                    {
                        if (playerData.hasFocus&& ((player.gamePad != null ? (player.gamePad.leftBumper > 0.1f && player.playerNumber != 5) : false) || ((Input.GetKey(KeyCode.C) && player.playerNumber == 5))))
                        {
                            ok = false;
                            if (canDo)
                            {
                                canDo = false;
                                if(((player.gamePad != null ? (player.gamePad.leftBumper > 0.1f && player.playerNumber != 5 && player.interactsAbillity == false) : false) || ((Input.GetKey(KeyCode.C) && player.playerNumber == 5))))
                                    ActivateMenu();
                            }
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
    }
}

public class CharacterStatus
{
    public float hp, reg, spd, dex, str, cdr;
}
