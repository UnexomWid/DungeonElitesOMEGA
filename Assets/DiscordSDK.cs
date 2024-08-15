using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using UnityEngine.SceneManagement;

public class DiscordSDK : MonoBehaviour
{
    Discord.Discord discord;

    void Start()
    {
        if (FindObjectsOfType<DiscordSDK>().Length > 1)
            Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);


        discord = new Discord.Discord(809399704474484757, (UInt64)Discord.CreateFlags.NoRequireDiscord);

        UpdatePresence();
    }

    void FakeCallback()
    {

    }

    void UpdatePresence()
    {
        if (discord == null)
            return;
           try
        {
            string SceneName = SceneManager.GetActiveScene().name;
            DungeonData dungeonData = FindObjectOfType<DungeonData>();
            MapSelect mapSelect = FindObjectOfType<MapSelect>();

            Activity discordActivity = new Activity();
            if (SceneName == "Start" || SceneName == "Settings" || SceneName == "MainMenuCredits")
                discordActivity.State = "In Lobby";
            else if (SceneName == "Select")
                discordActivity.State = "In Duel Lobby";
            else if (SceneName == "SelectDungeon")
                discordActivity.State = "In Dungeon Lobby";
            else if (dungeonData != null)
            {
                string state = "";

                if (dungeonData.currentMap == 6)
                {
                    state = "Black Dungeon(Final Boss)";
                }
                else
                {
                    switch (dungeonData.currentMapType)
                    {
                        case -1:
                            state = "Shops";
                            break;
                        case 1:
                            state = "Fire Dungeon";
                            break;
                        case 2:
                            state = "Ice Dungeon";
                            break;
                        case 3:
                            state = "Plains Dungeon";
                            break;
                        case 4:
                            state = "Stone Dungeon";
                            break;
                        case 5:
                            state = "Magic Dungeon";
                            break;
                    }

                    if (dungeonData.currentMapType != -1)
                        state += "(Level " + dungeonData.currentMap + ")";
                }

                PlayerData[] datas = FindObjectsOfType<PlayerData>();

                string mainCharacter = "";
                int mainChId = -1;

                foreach (PlayerData data in datas)
                {
                    if (data.playerNumber == 5 && data.locked)
                    {
                        mainChId = data.idCaracter;

                        switch (data.idCaracter)
                        {
                            case 1:
                                mainCharacter = "Wizard";
                                break;
                            case 2:
                                mainCharacter = "Knight";
                                break;
                            case 3:
                                mainCharacter = "Archer";
                                break;
                            case 4:
                                mainCharacter = "Tank";
                                break;
                            case 5:
                                mainCharacter = "Medic";
                                break;
                        }
                    }
                }

                int id = 1;

                while (id < 5 && string.IsNullOrWhiteSpace(mainCharacter))
                { 
                    foreach (PlayerData data in datas)
                    {
                        if (data.locked && data.playerNumber == id)
                        {
                            mainChId = data.idCaracter;

                            switch (data.idCaracter)
                            {
                                case 1:
                                    mainCharacter = "Wizard";
                                    break;
                                case 2:
                                    mainCharacter = "Knight";
                                    break;
                                case 3:
                                    mainCharacter = "Archer";
                                    break;
                                case 4:
                                    mainCharacter = "Tank";
                                    break;
                                case 5:
                                    mainCharacter = "Medic";
                                    break;
                            }
                        }
                    }
                    id++;
                }

                int mainLevel = 0;
                CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
                if (cameraFollow != null)
                {
                    foreach(player player in cameraFollow.playerScripts)
                    {
                        if(player.caracterId == mainChId)
                        {
                            mainLevel = (int)player.lvl;
                            break;
                        }
                    }
                }

                if(mainLevel == 0)
                {
                    mainLevel = (int)dungeonData.lvl[mainChId];
                }

                if (mainLevel == 0)
                    mainLevel = 1;

                discordActivity.State = state;
                if (dungeonData.currentMapType != -1)
                    discordActivity.Details = "Level " + mainLevel + " " + mainCharacter;
                else discordActivity.Details = mainCharacter;
                discordActivity.Assets.LargeImage = "small" + mainCharacter.ToLower();
                discordActivity.Timestamps.Start = dungeonData.begin;
            }
            else if (mapSelect != null)
            {
                PlayerData[] datas = FindObjectsOfType<PlayerData>();

                string mainCharacter = "";
                int mainLevel = -1;
                int count = 0;


                foreach (PlayerData data in datas)
                {
                    if (data.playerNumber == 5 && data.locked)
                    {
                        switch (data.idCaracter)
                        {
                            case 1:
                                mainCharacter = "Wizard";
                                break;
                            case 2:
                                mainCharacter = "Knight";
                                break;
                            case 3:
                                mainCharacter = "Archer";
                                break;
                            case 4:
                                mainCharacter = "Tank";
                                break;
                            case 5:
                                mainCharacter = "Medic";
                                break;
                        }
                    }
                }

                foreach(PlayerData data in datas)
                {
                    if (data.locked)
                    {
                        count++;
                    }
                }

                int id = 1;

                while (id < 5 && string.IsNullOrWhiteSpace(mainCharacter))
                {
                    foreach (PlayerData data in datas)
                    {
                        if (data.locked && data.playerNumber == id)
                        {
                            switch (data.idCaracter)
                            {
                                case 1:
                                    mainCharacter = "Wizard";
                                    break;
                                case 2:
                                    mainCharacter = "Knight";
                                    break;
                                case 3:
                                    mainCharacter = "Archer";
                                    break;
                                case 4:
                                    mainCharacter = "Tank";
                                    break;
                                case 5:
                                    mainCharacter = "Medic";
                                    break;
                            }
                        }
                    }
                    id++;
                }

                discordActivity.State = count + " Player Duel";
                discordActivity.Details = mainCharacter;
                discordActivity.Assets.LargeImage = "small" + mainCharacter.ToLower();
                if(mapSelect.begin == 0)
                    discordActivity.Timestamps.Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                else discordActivity.Timestamps.Start = mapSelect.begin;
            }

            discord.GetActivityManager().UpdateActivity(discordActivity, FakeCallback);
        }
        catch
        {

        }

        Invoke("UpdatePresence", 15f);
    }

    private void FakeCallback(Result result)
    {

    }

    private void Update()
    {
        if(discord != null)
        discord.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        if (discord != null)
            discord.Dispose();
    }

    private void OnLevelWasLoaded(int level)
    {
        CancelInvoke("UpdatePresence");
        UpdatePresence();
    }
}
