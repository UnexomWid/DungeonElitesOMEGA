using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour
{
    int index = -1;
    public string[] maps;

    public long begin = 0;

    public long GetNistTime()
    {
        try
        {
            DateTime dateTime;

            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            dateTime = DateTime.ParseExact(todaysDates,
                                       "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal);

            if (dateTime.Year < 2020)
            {
                return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            }
            else return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        catch
        {
            return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void NextMap()
    {
        GameObject.Find("Canvas").GetComponent<Animator>().Play("DuelSceneExit");
    }
    public void LoadMap()
    {
        index++;
        try
        {
            foreach (PlayerData player in FindObjectsOfType<PlayerData>())
            {
                if(player.enabled == false)
                    Destroy(player.gameObject);
            }
            SceneManager.LoadScene(maps[index]);

            if(index == 1)
            {
                begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        catch
        {
            foreach (PlayerData player in FindObjectsOfType<PlayerData>())
            {
                Destroy(player.gameObject);
            }
            //Destroy(FindObjectOfType<ControllerReconnect>().gameObject);
            SceneManager.LoadScene("Select");
            Destroy(gameObject);
        }
        
    }
}
