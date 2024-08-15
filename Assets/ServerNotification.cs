using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ServerNotification : MonoBehaviour
{
    public static ServerNotification instance;

    void Start()
    {
        if (FindObjectsOfType<ServerNotification>().Length > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);

            instance = this;

            SendMessageToServer("User opened the game.");
        }
    }

    private void OnApplicationQuit()
    {
        SendMessageToServer("User closed the game.");
    }

    private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";

    public static string UrlEncode(string value)
    {
        if (String.IsNullOrEmpty(value))
            return String.Empty;

        var sb = new StringBuilder();

        foreach (char @char in value)
        {
            if (reservedCharacters.IndexOf(@char) == -1)
                sb.Append(@char);
            else
                sb.AppendFormat("%{0:X2}", (int)@char);
        }
        return sb.ToString();
    }

    public void SendMessageToServer(string message)
    {
        /*try
        {
            string URL = "http://dungeonelitesresearch.000webhostapp.com/templates/newTable.php?json={\"device\":\"" + SystemInfo.deviceModel + "\",\"description\":\"" + message + "\"}";

            URL = URL.Replace("\n", "NEWLINE");

            StartCoroutine(GetRequest(URL));
        }
        catch
        {

        }*/
    }

    private void OnLevelWasLoaded(int level)
    {
        /*string name = SceneManager.GetActiveScene().name;

        switch (name)
        {
            case "Start":
                SendMessageToServer("User got into the Main Menu.");
                break;
            case "SelectDungeon":
                SendMessageToServer("User decided to play in the dungeon.");
                break;
            case "Select":
                SendMessageToServer("User decided to duel with his friends.");
                break;
            case "Credits":
                SendMessageToServer("User finished the game.");
                break;
            case "Intro":
                SendMessageToServer("User got into the game intro.");
                break;
            case "Settings":
                SendMessageToServer("User decided to switch some settings.");
                break;
            case "MainMenuCredits":
                SendMessageToServer("User decided to see the credits, how nice.");
                break;
            case "Shops":
                SendMessageToServer("User is practicing his skills or switching his abillities.");
                break;
            case "SampleScene1":
                {
                    int count = 0;
                    foreach(PlayerData data in FindObjectsOfType<PlayerData>())
                    {
                        if (data.locked)
                            count++;
                    }
                    SendMessageToServer("User started dueling with " + count + " players.");
                    break;
                }
        }*/
    }

    public void SendDungeonData(string message)
    {
        /*InventorySpawn inventorySpawn = FindObjectOfType<InventorySpawn>();

        message += "\nDungeon: " + SceneManager.GetActiveScene().name +"\n\nSEPARATOR";

        foreach (player player in FindObjectOfType<CameraFollow>().playerScripts)
        {
            try
            {
                message += "\n\n";
                switch (player.caracterId)
                {
                    case 1:
                        message += "Wizard: ";
                        break;
                    case 2:
                        message += "Knight: ";
                        break;
                    case 3:
                        message += "Archer: ";
                        break;
                    case 4:
                        message += "Tank: ";
                        break;
                    case 5:
                        message += "Medic: ";
                        break;
                }

                message += "Health=" + player.health + " Lvl=" + player.lvl + " Xp=" + player.xp;

                foreach (CharacterSlot slot in inventorySpawn.inventory.slots)
                {
                    if (slot.characterId == player.caracterId)
                    {
                        message += " A1Lvl=" + slot.ability1.GetComponent<SelectableItem>().lvl;
                        message += " A2Lvl=" + slot.ability2.GetComponent<SelectableItem>().lvl;
                        message += " A3Lvl=" + slot.ability3.GetComponent<SelectableItem>().lvl;
                        break;
                    }
                }

                message += " HP=" + player.hp;
                message += " Reg=" + player.reg;
                message += " Spd=" + player.spd;
                message += " Dex=" + player.dex;
                message += " Str=" + player.str;
                message += " Cdr=" + player.cdr;
            }
            catch
            {

            }
        }

        foreach (player player in FindObjectOfType<DeadMates>().players)
        {
            try
            {
                message += "\n";
                switch (player.caracterId)
                {
                    case 1:
                        message += "Wizard: ";
                        break;
                    case 2:
                        message += "Knight: ";
                        break;
                    case 3:
                        message += "Archer: ";
                        break;
                    case 4:
                        message += "Tank: ";
                        break;
                    case 5:
                        message += "Medic: ";
                        break;
                }

                message += "Health=Dead" + " Lvl=" + player.lvl + " Xp=" + player.xp;

                foreach (CharacterSlot slot in inventorySpawn.inventory.slots)
                {
                    if (slot.characterId == player.caracterId)
                    {
                        message += " A1Lvl=" + slot.ability1.GetComponent<SelectableItem>().lvl;
                        message += " A2Lvl=" + slot.ability2.GetComponent<SelectableItem>().lvl;
                        message += " A3Lvl=" + slot.ability3.GetComponent<SelectableItem>().lvl;
                        break;
                    }
                }

                message += " HP=" + player.hp;
                message += " Reg=" + player.reg;
                message += " Spd=" + player.spd;
                message += " Dex=" + player.dex;
                message += " Str=" + player.str;
                message += " Cdr=" + player.cdr;
            }
            catch
            {

            }
        }

        message += "\n\nItems: ";

        try
        {
            for (int i = 0; i < inventorySpawn.itemSlots.Length; i++)
            {
                message += inventorySpawn.itemSlots[i].itemType + " ";
            }

            message += "\n\nCoins=" + inventorySpawn.coins;
        }
        catch
        {

        }

        SendMessageToServer(message);*/
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            
        }
        else
        {

        }
    }
}
