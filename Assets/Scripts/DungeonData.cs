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
using UnityEngine.UI;

public class DungeonData : MonoBehaviour
{
    public int mainPlayerId;
    public long begin;
    public int currentMapType = -1;
    public int attempt = 1;
    public Text coinText;
    public int coins;
    public List<int> maps;
    public List<int> bosses;
    public int currentMap = 0;
    public bool restarted = false;
    public int[] items;
    public float[] health;
    public float[] xp;
    public float[] lvl;
    public int[] abil1Lvl;
    public int[] abil2Lvl;
    public int[] abil3Lvl;
    public int dungeonCoins;

    public int killedMobs = 0;

    AudioClip questSounds;

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

    public struct Status
    {
        public Status(int hp, int reg, int str, int cdr, int dex, int spd, int pointsLeft, int abilityPoints, int masterPoints)
        {
            this.hp = hp;
            this.reg = reg;
            this.str = str;
            this.cdr = cdr;
            this.dex = dex;
            this.spd = spd;
            this.pointsLeft = pointsLeft;
            this.abilityPoints = abilityPoints;
            this.masterPoints = masterPoints;
        }

        public int hp;
        public int reg;
        public int str;
        public int cdr;
        public int dex;
        public int spd;
        public int pointsLeft;
        public float abilityPoints;
        public float masterPoints;
    }

    public Status[] statuses;

    public int playerCount;

    public static DungeonData instance;


    List<Quest> quests;

    public void AddToQuest(int id, int count)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if(sceneName == "Fire" || sceneName == "Boss" || sceneName == "Magic" || sceneName == "Plains" || sceneName == "Ice" || sceneName == "Stone")
        for (int i = 0; i < quests.Count; i++)
        {
            if(quests[i].questId == id)
            {
                if (quests[i].progress != quests[i].threshold)
                {
                    quests[i].progress += count;

                    if (quests[i].progress > quests[i].threshold)
                    {
                        quests[i].progress = quests[i].threshold;
                    }

                    if(quests[i].progress == quests[i].threshold)
                    {
                        RemoveCoins(-quests[i].reward);

                        GameObject challengeText = GameObject.Find("ChallengeText");

                        challengeText.GetComponent<Animator>().Play("show", 0, 0);
                        challengeText.transform.GetChild(0).GetComponent<Text>().text = "Reward: " + quests[i].reward;

                        AudioSource.PlayClipAtPoint(questSounds, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                    }

                    PlayerPrefs.SetInt("Quest" + i + "Progress", quests[i].progress);
                }

                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        if (FindObjectsOfType<DungeonData>().Length >= 2)
            Destroy(gameObject);
        else
        {
            quests = QuestLoader.loadedQuests;

            instance = this;

            abil1Lvl = new int[6];
            abil2Lvl = new int[6];
            abil3Lvl = new int[6];
            lvl = new float[6];
            health = new float[6];
            xp = new float[6];
            statuses = new Status[6];
            items = new int[10];
            DontDestroyOnLoad(gameObject);
            coins = PlayerPrefs.GetInt("DungeonCoins", 15);
            if (coins == 15)
                PlayerPrefs.SetInt("DungeonCoins", 15);
            for (int i = 0; i <= 9; i++)
                items[i] = -1;
            foreach(PlayerData player in FindObjectsOfType<PlayerData>())
            {
                if (player.enabled && player.idCaracter > 0 && player.idCaracter <= 5)
                    playerCount++;
            }

            questSounds = Resources.Load("Sounds/quest") as AudioClip;

            coinText.text = coins.ToString();
        }
    }

    public void RemoveCoins(int value)
    {
        coins -= value;
        PlayerPrefs.SetInt("DungeonCoins", coins);
        if(coinText != null)
        coinText.text = coins.ToString();
    }

    public void NextMap()
    {
        if(maps.Count == 0)
        {
            List<int> possibleMaps = new List<int>();
            maps = new List<int>();

            possibleMaps.Add(1);
            possibleMaps.Add(2);
            possibleMaps.Add(3);
            possibleMaps.Add(4);
            possibleMaps.Add(5);

            while(possibleMaps.Count != 0)
            {
                int num = possibleMaps[UnityEngine.Random.Range(1, 99999) % possibleMaps.Count];

                maps.Add(num);
                possibleMaps.Remove(num); 
            }
        }

        if (bosses.Count == 0)
        {
            List<int> possibleBosses = new List<int>();
            bosses = new List<int>();

            possibleBosses.Add(1);
            possibleBosses.Add(2);
            possibleBosses.Add(3);
            possibleBosses.Add(4);
            possibleBosses.Add(5);

            while (possibleBosses.Count != 0)
            {
                int num = possibleBosses[UnityEngine.Random.Range(1, 99999) % possibleBosses.Count];

                bosses.Add(num);
                possibleBosses.Remove(num);
            }
        }

        if (currentMap < 5)
        {

            if (currentMapType == -1)
                begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (maps[currentMap] == 1)
                SceneManager.LoadScene("Fire");
            else if (maps[currentMap] == 2)
                SceneManager.LoadScene("Ice");
            else if (maps[currentMap] == 3)
                SceneManager.LoadScene("Plains");
            else if (maps[currentMap] == 4)
                SceneManager.LoadScene("Stone");
            else if (maps[currentMap] == 5)
                SceneManager.LoadScene("Magic");
        }
        else
        {
            SceneManager.LoadScene("Boss");
        }

        currentMap++;
    }

    public void GenerateMaps(int mapIndex)
    {
        List<int> possibleMaps = new List<int>();
        maps = new List<int>();

        possibleMaps.Add(1);
        possibleMaps.Add(2);
        possibleMaps.Add(3);
        possibleMaps.Add(4);
        possibleMaps.Add(5);

        int index = 0;

        possibleMaps.Remove(mapIndex);

        while (possibleMaps.Count != 0)
        {
            if (index == currentMap)
                maps.Add(mapIndex);
            else
            {
                int num = possibleMaps[UnityEngine.Random.Range(1, 99999) % possibleMaps.Count];

                maps.Add(num);
                possibleMaps.Remove(num);
            }

            index++;
        }
    }
}
