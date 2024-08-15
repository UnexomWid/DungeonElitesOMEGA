using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Buttons : MonoBehaviour
{
    public void LocalGame()
    {
        SceneManager.LoadScene("Select");
    }
    public void OnlineGame()
    {
        SceneManager.LoadScene("OnlineSelect");
    }
    public void HostGame()
    {
        GameObject host = new GameObject();
        host.AddComponent<GameHost>();
        host.name = "GameHost";
        DontDestroyOnLoad(host);
        host.GetComponent<GameHost>().Begin();
        GameObject client = new GameObject();
        client.AddComponent<GameClient>();
        var localHost = Dns.GetHostEntry(Dns.GetHostName());
        string ownIp = "";
        foreach(var ip in localHost.AddressList)
        {
            if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ownIp = ip.ToString();
            }
        }
        client.GetComponent<GameClient>().ip = ownIp;
        client.name = "GameClient";
        DontDestroyOnLoad(client);
        client.GetComponent<GameClient>().Begin();
    }
    public InputField ipAdress;
    public void JoinGame()
    {
        GameObject client = new GameObject();
        client.AddComponent<GameClient>();
        client.name = "GameClient";
        DontDestroyOnLoad(client);
        client.GetComponent<GameClient>().ip = ipAdress.text;
        client.GetComponent<GameClient>().Begin();
    }
    public void Dungeon()
    {
        SceneManager.LoadScene("SelectDungeon");
    }
}
