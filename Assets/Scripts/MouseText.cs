using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MouseText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip press;
    public Vector2 targetSize;
    public float speed;
    public AudioClip hover;


    bool focus = false;

    public void Focus()
    {
        focus = true;

        AudioSource.PlayClipAtPoint(hover, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }
    public void Unfocus()
    {
        focus = false;
    }

    Vector2 startSize;

    void Start()
    {
        Time.timeScale = 1;

        startSize = transform.localScale;

        if (option == "Intro")
        {
            if (PlayerPrefs.GetInt("Intro", 1) == 0)
            {
                GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
            }
        }
        else if (option == "Tutorial")
        {
            if (PlayerPrefs.GetInt("Tutorial", 1) == 0)
            {
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
            }
        }
        else if (option == "JoystickDir")
        {
            if (PlayerPrefs.GetInt("HelperLine", 1) == 1)
            {
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
            }
        }
        else if (option == "Difficulty")
        {
            switch(PlayerPrefs.GetInt("Difficulty", 0))
            {
                case 0:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        break;
                    }
                                  case 1:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        break;
                    }
                                  case 2:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        break;
                    }
                                  case 3:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        break;
                    }
                                  case 4:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        break;
                    }

            }
        }
        else if (option == "EnemyBar")
        {
            if (PlayerPrefs.GetInt("EnemyBar", 1) == 1)
            {
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
            }
        }
        else if (option == "BarFade")
        {
            if (PlayerPrefs.GetInt("BarFade", 1) == 1)
            {
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
            }
        }
        else if (option == "AbillitySel")
        {
            if (PlayerPrefs.GetInt("ShopsTutorial", 1) == 1)
            {
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
            }
        }
        else if (option == "FPS")
        {
            if (PlayerPrefs.GetInt("FPS", 1) == 1)
            {
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";
            }
        }
        else if (option == "ToggleMovement")
        {
            if (PlayerPrefs.GetInt("ToggleMovement", 1) == 0)
            {
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
            }
        }
    }

    bool hovered = false;

    float t = 0;
    private void Update()
    {
        if (option == "Shops")
        {
            bool ok = false;
            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if (data.enabled && data.locked)
                {
                    ok = true;
                }
                if (data.enabled && data.locked == false)
                {
                    ok = false;
                    break;
                }
            }
            GetComponent<TextMeshProUGUI>().enabled = ok;
            foreach (Transform child in transform)
            {
                child.GetComponent<TextMeshProUGUI>().enabled = ok;
            }
        }
        else if(option == "StartDuel")
        {
            bool ok = true;
            int count = 0;
            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if (data.enabled && data.locked)
                {
                    count++;
                }
                if (data.enabled && data.locked == false)
                {
                    ok = false;
                    break;
                }
            }
            if (count < 2)
                ok = false;

            List<int> teams = new List<int>();
            foreach(PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if(teams.Contains(data.echipa) == false && data.echipa != 0)
                {
                    teams.Add(data.echipa);
                }
            }
            if (teams.Count < 2)
                ok = false;

            GetComponent<TextMeshProUGUI>().enabled = ok;
            foreach (Transform child in transform)
            {
                child.GetComponent<TextMeshProUGUI>().enabled = ok;
            }
        }

        if (hovered || focus)
        {
            t += PublicVariables.deltaTime * speed;
        }
        else t -= PublicVariables.deltaTime * speed;

        if (t >= 1)
            t = 1;
        if (t <= 0)
            t = 0;

        transform.localScale = Vector2.Lerp(startSize, targetSize, t);
    }
    public string option;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (option == "Dungeon")
        { 
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDungeon", 1.5f);
        }
        else if(option == "Database")
        {
            Application.OpenURL("http://dungeonelitesresearch.000webhostapp.com/templates/index.php");
        }
        else if(option == "Settings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadSettings", 1.5f);
        }
        else if (option == "Credits")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadCredits", 1.5f);
        }
        else if (option == "GameplaySettings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadGameplaySettings", 1.5f);
        }
        else if(option == "Intro")
        {
            if(PlayerPrefs.GetInt("Intro", 1) == 1)
            {
                PlayerPrefs.SetInt("Intro", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
            }
            else
            {
                PlayerPrefs.SetInt("Intro", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
            }
        }
        else if (option == "Tutorial")
        {
            if (PlayerPrefs.GetInt("Tutorial", 1) == 1)
            {
                PlayerPrefs.SetInt("Tutorial", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
            }
            else
            {
                PlayerPrefs.SetInt("Tutorial", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
            }
        }
        else if (option == "JoystickDir")
        {
            if (PlayerPrefs.GetInt("HelperLine", 1) == 1)
            {
                PlayerPrefs.SetInt("HelperLine", 0);
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
            }
            else
            {
                PlayerPrefs.SetInt("HelperLine", 1);
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
            }
        }
        else if (option == "Difficulty")
        {
            int num = PlayerPrefs.GetInt("Difficulty") + 1;

            if (num > 4)
                num = 0;

            PlayerPrefs.SetInt("Difficulty", num);

            switch (PlayerPrefs.GetInt("Difficulty", 0))
            {
                case 0:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        break;
                    }
                case 1:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        break;
                    }
                case 2:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        break;
                    }
                case 3:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        break;
                    }
                case 4:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        break;
                    }

            }
        }
        else if (option == "EnemyBar")
        {
            if (PlayerPrefs.GetInt("EnemyBar", 1) == 0)
            {
                PlayerPrefs.SetInt("EnemyBar", 1);
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
            }
            else
            {
                PlayerPrefs.SetInt("EnemyBar", 0);
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
            }
        }
        else if (option == "BarFade")
        {
            if (PlayerPrefs.GetInt("BarFade", 1) == 0)
            {
                PlayerPrefs.SetInt("BarFade", 1);
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
            }
            else
            {
                PlayerPrefs.SetInt("BarFade", 0);
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
            }
        }
        else if (option == "AbillitySel")
        {
            if (PlayerPrefs.GetInt("ShopsTutorial", 1) == 1)
            {
                PlayerPrefs.SetInt("ShopsTutorial", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
            }
            else
            {
                PlayerPrefs.SetInt("ShopsTutorial", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
            }
        }
        else if(option == "Duel")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDuel", 1.5f);
        }
        else if (option == "Settings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("settingsTransition");
            Invoke("LoadSettings", 1.5f);
        }
        else if (option == "Exit")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadExit", 1.5f);
        }
        else if (option == "MainMenu")
        {
            foreach(PlayerData data in FindObjectsOfType<PlayerData>())
            {
                Destroy(data.gameObject);
            }
            Destroy(FindObjectOfType<DungeonData>());
            Destroy(FindObjectOfType<ControllerReconnect>());
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadMainMenu", 1.5f);
        }
        else if (option == "Shops" && GetComponent<TextMeshProUGUI>().enabled)
        {

            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if(data.locked == false)
                {
                    data.idCaracter = 0;
                }
            }
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadShops", 1.5f);
        }
        else if (option == "StartDuel" && GetComponent<TextMeshProUGUI>().enabled)
        {

            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if (data.locked == false)
                {
                    data.idCaracter = 0;
                }
            }
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDuelLvl", 1.5f);
        }
        else if (option == "MainMenu2")
        {

            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadMainMenu", 1.5f);
        }
        else if (option == "FPS")
        {
            if (PlayerPrefs.GetInt("FPS", 1) == 1)
            {
                PlayerPrefs.SetInt("FPS", 0);
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";

                FPSCounter.instance.ShowFPS();
            }
            else
            {
                PlayerPrefs.SetInt("FPS", 1);
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";

                FPSCounter.instance.ShowFPS();
            }
        }
        else if (option == "ToggleMovement")
        {
            if (PlayerPrefs.GetInt("ToggleMovement", 1) == 1)
            {
                PlayerPrefs.SetInt("ToggleMovement", 0);
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
            }
            else
            {
                PlayerPrefs.SetInt("ToggleMovement", 1);
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
            }
        }
        if (option != "Intro" && option != "Tutorial" && option != "JoystickDir" && option != "AbillitySel" && option != "EnemyBar" && option != "BarFade" && option != "FPS" && option != "ToggleMovement" && option != "Difficulty")
        {
            if (option == "Dungeon" || option == "Duel" || option == "MainMenu" || option == "Exit" || option == "StartDuel" || option == "Shops")
            Destroy(FindObjectOfType<Music>().gameObject);
            foreach (MouseText text in FindObjectsOfType<MouseText>())
            {
                text.enabled = false;
            }
            if (FindObjectOfType<CreditsControls>() != null)
                Destroy(FindObjectOfType<CreditsControls>());
        }
        AudioSource.PlayClipAtPoint(press, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }

    public void Activate()
    {
        if (option == "Dungeon")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDungeon", 1.5f);
        }
        else if (option == "Settings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadSettings", 1.5f);
        }
        else if (option == "Credits")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadCredits", 1.5f);
        }
        else if (option == "GameplaySettings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadGameplaySettings", 1.5f);
        }
        else if (option == "Intro")
        {
            if (PlayerPrefs.GetInt("Intro", 1) == 1)
            {
                PlayerPrefs.SetInt("Intro", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: Off";
            }
            else
            {
                PlayerPrefs.SetInt("Intro", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Intro: On";
            }
        }
        else if (option == "Tutorial")
        {
            if (PlayerPrefs.GetInt("Tutorial", 1) == 1)
            {
                PlayerPrefs.SetInt("Tutorial", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: Off";
            }
            else
            {
                PlayerPrefs.SetInt("Tutorial", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Tutorial: On";
            }
        }
        else if (option == "JoystickDir")
        {
            if (PlayerPrefs.GetInt("HelperLine", 1) == 1)
            {
                PlayerPrefs.SetInt("HelperLine", 0);
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: On";
            }
            else
            {
                PlayerPrefs.SetInt("HelperLine", 1);
                GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joystick Direction on Character: Off";
            }
        }
        else if (option == "Difficulty")
        {
            int num = PlayerPrefs.GetInt("Difficulty") + 1;

            if (num > 4)
                num = 0;

            PlayerPrefs.SetInt("Difficulty", num);

            switch (PlayerPrefs.GetInt("Difficulty", 0))
            {
                case 0:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Easy";
                        break;
                    }
                case 1:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Normal";
                        break;
                    }
                case 2:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Hard";
                        break;
                    }
                case 3:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Insane";
                        break;
                    }
                case 4:
                    {
                        GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Difficulty: Impossible";
                        break;
                    }

            }
        }
        else if (option == "FPS")
        {
            if (PlayerPrefs.GetInt("FPS", 1) == 1)
            {
                PlayerPrefs.SetInt("FPS", 0);
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: On";

                FPSCounter.instance.ShowFPS();
            }
            else
            {
                PlayerPrefs.SetInt("FPS", 1);
                GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FPS Counter: Off";

                FPSCounter.instance.ShowFPS();
            }
        }
        else if(option == "ToggleMovement")
        {
            if (PlayerPrefs.GetInt("ToggleMovement", 1) == 1)
            {
                PlayerPrefs.SetInt("ToggleMovement", 0);
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Hold";
            }
            else
            {
                PlayerPrefs.SetInt("ToggleMovement", 1);
                GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock Movement: Toggle";
            }
        }
        else if (option == "EnemyBar")
        {
            if (PlayerPrefs.GetInt("EnemyBar", 1) == 0)
            {
                PlayerPrefs.SetInt("EnemyBar", 1);
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: On";
            }
            else
            {
                PlayerPrefs.SetInt("EnemyBar", 0);
                GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Healthbar: Off";
            }
        }
        else if (option == "BarFade")
        {
            if (PlayerPrefs.GetInt("BarFade", 1) == 0)
            {
                PlayerPrefs.SetInt("BarFade", 1);
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: Off";
            }
            else
            {
                PlayerPrefs.SetInt("BarFade", 0);
                GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Healthbar Fade: On";
            }
        }
        else if (option == "AbillitySel")
        {
            if (PlayerPrefs.GetInt("ShopsTutorial", 1) == 1)
            {
                PlayerPrefs.SetInt("ShopsTutorial", 0);
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: Off";
            }
            else
            {
                PlayerPrefs.SetInt("ShopsTutorial", 1);
                GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Abillity Selection Tutorial: On";
            }
        }
        else if (option == "Duel")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDuel", 1.5f);
        }
        else if (option == "Settings")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("settingsTransition");
            Invoke("LoadSettings", 1.5f);
        }
        else if (option == "Exit")
        {
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadExit", 1.5f);
        }
        else if (option == "MainMenu")
        {
            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                Destroy(data.gameObject);
            }
            Destroy(FindObjectOfType<DungeonData>());
            Destroy(FindObjectOfType<ControllerReconnect>());
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadMainMenu", 1.5f);
        }
        else if (option == "MainMenu2")
        {
           
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadMainMenu", 1.5f);
        }
        else if (option == "Shops" && GetComponent<TextMeshProUGUI>().enabled)
        {

            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if (data.locked == false)
                {
                    data.idCaracter = 0;
                }
            }
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadShops", 1.5f);
        }
        else if (option == "StartDuel" && GetComponent<TextMeshProUGUI>().enabled)
        {

            foreach (PlayerData data in FindObjectsOfType<PlayerData>())
            {
                if (data.locked == false)
                {
                    data.idCaracter = 0;
                }
            }
            FindObjectOfType<Canvas>().GetComponent<Animator>().Play("transition");
            Invoke("LoadDuelLvl", 1.5f);
        }
        if (option != "Intro" && option != "Tutorial" && option != "JoystickDir" && option != "AbillitySel" && option != "EnemyBar" && option != "BarFade" && option != "FPS" && option != "ToggleMovement" && option != "Difficulty")
        {
            if (option == "Dungeon" || option == "Duel" || option == "MainMenu" || option == "Exit" || option == "StartDuel" || option == "Shops")
                Destroy(FindObjectOfType<Music>().gameObject);
            foreach (MouseText text in FindObjectsOfType<MouseText>())
            {
                text.enabled = false;
            }
            if (FindObjectOfType<CreditsControls>() != null)
                Destroy(FindObjectOfType<CreditsControls>());
            
        }
        AudioSource.PlayClipAtPoint(press, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }

    void LoadDungeon()
    {
        SceneManager.LoadScene("SelectDungeon");
    }

    void LoadDuel()
    {
        SceneManager.LoadScene("Select");
    }

    void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    void LoadCredits()
    {
        SceneManager.LoadScene("MainMenuCredits");
    }
    void LoadGameplaySettings()
    {
        SceneManager.LoadScene("Gameplay Settings");
    }

    void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
    }

    void LoadShops()
    {
        Time.timeScale = 1;
        if (PlayerPrefs.GetInt("Intro", 1) == 1)
            SceneManager.LoadScene("Intro");
        else if (PlayerPrefs.GetInt("Tutorial", 1) == 1)
        {
            SceneManager.LoadScene("TutorialScene");
        }
        else SceneManager.LoadScene("Shops");
    }

    void LoadDuelLvl()
    {
        SceneManager.LoadScene("SampleScene 1");
    }

    void LoadExit()
    {
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioSource.PlayClipAtPoint(hover, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
            hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
