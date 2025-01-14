using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestLoader : MonoBehaviour
{
    public static List<Quest> loadedQuests = new List<Quest>();

    [SerializeField] Text questCount;
    [SerializeField] GameObject menu;

    public void LoadMenu()
    {
        menu.SetActive(!menu.activeInHierarchy);
        Time.timeScale = menu.activeInHierarchy ? 0.000001f : 1;
    }

    [SerializeField] Text[] questDescriptionTexts;
    [SerializeField] Text[] questProgressTexts;
    [SerializeField] GameObject[] questProgressObjects;
    [SerializeField] Text[] questRewardTexts;
    [SerializeField] GameObject[] questCheckMarks;
    [SerializeField] GameObject[] questRewardObjects;

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
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightBumper > 0.1f && player.playerNumber != 5) : false) || ((Input.GetKey(KeyCode.X) && player.playerNumber == 5))))
                            {
                                ok = false;
                                if (canDo)
                                {
                                    canDo = false;
                                    LoadMenu();
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

    CameraFollow cameraFollow;
    PlayerData playerData;
    private void Awake()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
        playerData = FindObjectOfType<PlayerData>();

        string lastQuestDate = PlayerPrefs.GetString("LastQuestDate", "");

        if (lastQuestDate == "" || (DateTime.Now - DateTime.ParseExact(lastQuestDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)).TotalDays >= 1)
        {
            List<Quest> questList = quests.ToList();
            Quest selectedQuest;

            for (int i = 0; i <= 2; i++)
            {
                selectedQuest = questList[UnityEngine.Random.Range(1, 99999) % questList.Count];
                loadedQuests.Add(selectedQuest);
                questList.Remove(selectedQuest);

                questDescriptionTexts[i].text = selectedQuest.questDescription;
                questProgressTexts[i].text = selectedQuest.progress + "/" + selectedQuest.threshold;
                questProgressObjects[i].transform.localScale = new Vector3(selectedQuest.progress / (float)selectedQuest.threshold, 1, 1);
                questRewardTexts[i].text = selectedQuest.reward.ToString();

                PlayerPrefs.SetInt("Quest" + i + "Id", selectedQuest.questId);
                PlayerPrefs.SetString("Quest" + i + "Description", selectedQuest.questDescription);
                PlayerPrefs.SetInt("Quest" + i + "Reward", selectedQuest.reward);
                PlayerPrefs.SetInt("Quest" + i + "Threshold", selectedQuest.threshold);
                PlayerPrefs.SetInt("Quest" + i + "Progress", selectedQuest.progress);
            }

            PlayerPrefs.SetString("LastQuestDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        else
        {
            int questNum = 3;

            for (int i = 0; i <= 2; i++)
            {
                Quest quest = new Quest(
                PlayerPrefs.GetInt("Quest" + i + "Id", 0),
                PlayerPrefs.GetString("Quest" + i + "Description", ""),
                PlayerPrefs.GetInt("Quest" + i + "Reward", 0),
                PlayerPrefs.GetInt("Quest" + i + "Threshold", 0),
                PlayerPrefs.GetInt("Quest" + i + "Progress", 0)
                );

                questDescriptionTexts[i].text = quest.questDescription;
                questProgressTexts[i].text = quest.progress + "/" + quest.threshold;
                questProgressObjects[i].transform.localScale = new Vector3(quest.progress / (float)quest.threshold, 1, 1);
                questRewardTexts[i].text = quest.reward.ToString();

                if(quest.progress == quest.threshold)
                {
                    questRewardObjects[i].SetActive(false);
                    questCheckMarks[i].SetActive(true);

                    questNum--;
                }

                loadedQuests.Add(quest);
            }

            questCount.text = questNum.ToString();
        }
    }

    Quest[] quests =
    {
        new Quest(1, "Destroy 3 voids.", 10, 3, 0),
        new Quest(2, "Collect 3 chests.", 10, 3, 0),
        new Quest(3, "Kill 200 monsters.", 10, 200, 0),
        new Quest(4, "Defeat a dungeon.", 10, 1, 0),
        new Quest(5, "Assign 5 status points to HP in a single run.", 10, 1, 0),
        new Quest(6, "Assign 5 status points to Reg in a single run.", 10, 1, 0),
        new Quest(7, "Assign 5 status points to Spd in a single run.", 10, 1, 0),
        new Quest(8, "Assign 5 status points to Dex in a single run.", 10, 1, 0),
        new Quest(9, "Assign 5 status points to Str in a single run.", 10, 1, 0),
        new Quest(10, "Assign 5 status points to Cdr in a single run.", 10, 1, 0),
        //new Quest(11, "Perform a 10 hit combo on a monster.", 10, 1, 0), //cum drq implementez asta
        new Quest(12, "Hold 750 coins at once.", 10, 1, 0),
        new Quest(13, "Collect 2500 coins.", 10, 2500, 0),
        new Quest(14, "Gain 10 levels.", 10, 10, 0),
        new Quest(15, "Perfect any abillity.", 10, 1, 0),
        new Quest(16, "Master 3 abillities.", 10, 3, 0),
        new Quest(17, "Play 5 times as the wizard.", 10, 5, 0),
        new Quest(18, "Play 5 times as the knight.", 10, 5, 0),
        new Quest(19, "Play 5 times as the archer.", 10, 5, 0),
        new Quest(20, "Play 5 times as the tank.", 10, 5, 0),
        new Quest(21, "Play 5 times as the medic.", 10, 5, 0),
        new Quest(22, "Defeat a dungeon room without taking damage.", 10, 1, 0),
        new Quest(23, "Defeat a dungeom room in less than 8 seconds.", 10, 1, 0),
        new Quest(24, "Explore the whole map.", 10, 1, 0),
        new Quest(25, "Heal 1000 health.", 10, 1000, 0),
        new Quest(26, "Defeat 5 dungeon rooms without using basic attacks.", 10, 5, 0),
        new Quest(27, "Defeat 5 dungeon rooms without using abillities.", 10, 5, 0),
        new Quest(28, "Buy 3 items.", 10, 3, 0),
        new Quest(29, "Try your luck. ;)", 10, 1, 0),
        new Quest(30, "Assign a status point to every single stat.", 10, 1, 0),
        new Quest(31, "Defy death.", 10, 1, 0),
    };
}

public class Quest
{
    public int orderId;
    public int questId;
    public string questDescription;
    public int reward;
    public int threshold;
    public int progress;

    public Quest(int questId, string questDescription, int reward, int threshold, int progress)
    {
        this.questId = questId;
        this.questDescription = questDescription;
        this.reward = reward;
        this.threshold = threshold;
        this.progress = progress;
    }
}
