using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public GameObject nameText;

    public char[] characters;
    public Text prevChar;
    public Text nextChar;
    public Text currentChar;
    new public Text name;

    public int charIndex = 0;

    public AudioClip playerEnter;
    public AudioClip playerLock;

    public bool hasFocus = true;

    public int playerNumber;
    public int idCaracter;
    public int abilitate1;
    public int abilitate2;
    public int abilitate3;
    public InputField idCaracterField;
    public InputField abilitate1Field;
    public InputField abilitate2Field;
    public InputField abilitate3Field;
    public string playerName;
    public InputField nameField;
    public int echipa;
    public InputField teamField;
    public GameObject objs;
    new public bool enabled = false;
    //public SharpDX.DirectInput.Joystick joystick;

    int buttonIndex = 0;
    public MouseText[] buttons;
    bool focusCharacter = true;

    Escape escape;

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
    }

    ControllerReconnect controllerReconnect;

    Canvas canvas;

    public ControllerInput gamepad;

    bool onPhone = false;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        try
        {
            escape = FindObjectOfType<Escape>();
        }
        catch
        {

        }
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        controllerReconnect = FindObjectOfType<ControllerReconnect>();
        idCaracter = 1;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        /*List<SharpDX.DirectInput.Joystick> controllers = new List<SharpDX.DirectInput.Joystick>();
        var joystickGuid = Guid.Empty;
        var di = new DirectInput();*/
        if (playerNumber != 5)
        {
            abils = new Sprite[5][];
            abils[0] = wizardAbils;
            abils[1] = knightAbils;
            abils[2] = archerAbils;
            abils[3] = tankAbils;
            abils[4] = supportAbils;

            abilitate1 = abilitate2 = abilitate3 = 1;

            /*IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
            for (int device = 0; device < gamepads.Count; device++)
            {
                joystickGuid = gamepads[device].InstanceGuid;
                controllers.Add(new Joystick(di, joystickGuid));
            }
            IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);
            for (int device = 0; device < joysticks.Count; device++)
            {
                joystickGuid = joysticks[device].InstanceGuid;
                controllers.Add(new Joystick(di, joystickGuid));
            }
            joystick = controllers[playerNumber - 1];*/
            hasController = true;
        }
        else
        {
            abils = new Sprite[5][];
            abils[0] = wizardAbils;
            abils[1] = knightAbils;
            abils[2] = archerAbils;
            abils[3] = tankAbils;
            abils[4] = supportAbils;

            abilitate1 = abilitate2 = abilitate3 = 1;

            /*IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);
            for (int device = 0; device < keyboards.Count; device++)
            {
                joystickGuid = keyboards[device].InstanceGuid;
                controllers.Add(new Joystick(di, joystickGuid));
            }*/
            //joystick = controllers[0];
        }
        //joystick.Acquire();
        //controllerReconnect.controllers[playerNumber] = joystick;

        if(onPhone)
        {
            enabled = true;
            if (objs != null)
                objs.SetActive(true);
        }
    }
    bool pressed = false;
    bool pressed2 = false;

    public bool duel = false;
    public GameObject abil1;
    public GameObject abil2;
    public GameObject abil3;
    public GameObject abil1Pos;
    public GameObject abil2Pos;
    public GameObject abil3Pos;
    public GameObject teams;

    public Sprite[] wizardAbils;
    public Sprite[] knightAbils;
    public Sprite[] archerAbils;
    public Sprite[] tankAbils;
    public Sprite[] supportAbils;

    Sprite[][] abils;

    int stage = 0;
    bool hasController = false;
    bool ret = false;
    // Update is called once per frame
    void Update()
    {
        if (ret)
            return;

        if(gamepad == null && onPhone == false && playerNumber != 5 && SceneManager.GetActiveScene().name.Contains("Select"))
        {
            try
            {
                ControllerInput[] controllers = FindObjectsOfType<ControllerInput>();
                if(controllers.Length>= playerNumber)
                {
                    foreach(ControllerInput controller in controllers)
                    {
                        if (playerNumber == controller.playerNumber)
                            gamepad = controller;
                    }
                }
            }
            catch
            {

            }
        }
        /*if (controllerReconnect != null && ((hasController == false && SceneManager.GetActiveScene().name.Contains("Select")) || hasController))
        {
            if ((hasController && (locked || SceneManager.GetActiveScene().name.Contains("Select"))) || hasController == false)
            {
                /*if (joystick != null)
                {
                    bool ret = false;
                    /*gamepad.
                    try
                    {
                        if (gamepad.A == 1)
                            Debug.Log("hasController");
                        joystick.Poll();
                    }
                    catch
                    {
                        enabled = false;
                        locked = false;
                        if (keyObj != null)
                            keyObj.SetActive(true);
                        stage = 0;
                        if (objs != null)
                            objs.SetActive(enabled);
                        joystick = controllerReconnect.controllers[playerNumber];
                        try
                        {
                            joystick.Acquire();
                            
                            joystick.Poll();
                        }
                        catch
                        {
                            controllerReconnect.pendingControllers[playerNumber] = true;
                        }
                        ret = true;
                        return;
                    }


                    if (ret)
                        return;
                }
                else
                {
                    Debug.Log("works3");
                    if (controllerReconnect.controllers[playerNumber] != null)
                    {
                        joystick = controllerReconnect.controllers[playerNumber];
                        joystick.Acquire();
                        try
                        {

                            joystick.Poll();
                        }
                        catch
                        {
                            controllerReconnect.pendingControllers[playerNumber] = true;
                        }
                    }
                    else controllerReconnect.pendingControllers[playerNumber] = true;
                }
            }
        }
        else if(hasController == false && SceneManager.GetActiveScene().name.Contains("Select") == false)
        {
            controllerReconnect.pendingControllers[playerNumber] = false;
        }*/
        
        if (gamepad != null || playerNumber == 5)
        {
            if (duel)
            {
                if (SceneManager.GetActiveScene().name.Contains("Select"))
                {
                    if (FindObjectOfType<Canvas>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("transition") == false)
                    {
                        if (((gamepad != null ? (gamepad.A == 1 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && playerNumber == 5)) && (SceneManager.GetActiveScene().name == "Select" || SceneManager.GetActiveScene().name == "SelectDungeon"))
                        {
                            if (pressed == false)
                            {
                                if (focusCharacter == false && playerNumber != 5)
                                {
                                    if (buttons[buttonIndex].GetComponent<TextMeshProUGUI>().enabled)
                                    {
                                        buttons[buttonIndex].Activate();
                                    }
                                }
                                else
                                {
                                    if (stage == 0)
                                    {
                                        AudioSource.PlayClipAtPoint(playerEnter, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        stage = 1;
                                        escape.enabled = false;
                                        enabled = true;
                                        if (objs != null)
                                            objs.SetActive(true);
                                    }
                                    else if (stage == 1)
                                    {
                                        AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        stage = 2;
                                        characterPos.SetActive(false);
                                        characterPos.transform.localPosition = new Vector2(characterXPos[this.index], characterPos.transform.localPosition.y);
                                        abil1.SetActive(true);
                                        int index = 0;
                                        foreach (Image image in abil1Pos.GetComponentsInChildren<Image>())
                                        {
                                            image.sprite = abils[idCaracter - 1][index++];
                                        }
                                        objs.GetComponent<Animator>().Play("lock");
                                    }
                                    else if (stage == 2)
                                    {
                                        AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        stage = 3;
                                        abil1Pos.transform.localPosition = new Vector2(abilPositions[abil1Index], abil1Pos.transform.localPosition.y);
                                        abil2.SetActive(true);
                                        int index = 0;
                                        foreach (Image image in abil2Pos.GetComponentsInChildren<Image>())
                                        {
                                            image.sprite = abils[idCaracter - 1][index++];
                                        }
                                        objs.GetComponent<Animator>().Play("lock");
                                    }
                                    else if (stage == 3 && abilitate2 != abilitate1)
                                    {
                                        AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        abil2Pos.transform.localPosition = new Vector2(abilPositions[abil2Index], abil2Pos.transform.localPosition.y);
                                        stage = 4;
                                        abil3.SetActive(true);
                                        int index = 0;
                                        foreach (Image image in abil3Pos.GetComponentsInChildren<Image>())
                                        {
                                            image.sprite = abils[idCaracter - 1][index++];
                                        }
                                        objs.GetComponent<Animator>().Play("lock");
                                    }
                                    else if (stage == 4 && abilitate3 != abilitate1 && abilitate3 != abilitate2)
                                    {
                                        AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        abil3Pos.transform.localPosition = new Vector2(abilPositions[abil3Index], abil3Pos.transform.localPosition.y);
                                        stage = 5;
                                        abil1.SetActive(false);
                                        abil2.SetActive(false);
                                        abil3.SetActive(false);
                                        teams.SetActive(true);
                                        objs.GetComponent<Animator>().Play("lock");
                                        if (echipa == 0)
                                            echipa = 1;
                                    }
                                    else if (stage == 5)
                                    {
                                        stage = 6;
                                        locked = true;
                                        teamPos.transform.localPosition = new Vector2(abilPositions[teamIndex], teamPos.transform.localPosition.y);
                                        teams.SetActive(false);
                                        nameText.SetActive(true);
                                        AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                                        objs.GetComponent<Animator>().Play("lock", 0, 0f);
                                        keyObj.SetActive(false);
                                    }
                                    else if (stage == 6)
                                    {
                                        if (name.text.Length <= 9)
                                        {
                                            name.text += characters[charIndex];
                                            playerName = name.text;
                                        }
                                    }
                                }
                                pressed = true;
                            }
                        }
                        else pressed = false;

                        if (((gamepad != null ? (gamepad.B == 1 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.Escape) && playerNumber == 5)) && (SceneManager.GetActiveScene().name == "Select" || SceneManager.GetActiveScene().name == "SelectDungeon"))
                        {
                            if (pressed2 == false)
                            {
                                if (enabled == true)
                                {
                                    if (stage == 1)
                                    {
                                        enabled = false;
                                        stage = 0;
                                        escape.enabled = true;
                                        objs.SetActive(false);
                                    }
                                    else if (stage == 2)
                                    {
                                        stage = 1;
                                        characterPos.SetActive(true);
                                        abil1.SetActive(false);
                                    }
                                    else if (stage == 3)
                                    {
                                        stage = 2;
                                        abil2.SetActive(false);
                                    }
                                    else if (stage == 4)
                                    {
                                        stage = 3;
                                        abil3.SetActive(false);
                                    }
                                    else if (stage == 5)
                                    {
                                        stage = 4;
                                        abil1.SetActive(true);
                                        abil2.SetActive(true);
                                        abil3.SetActive(true);
                                        teams.SetActive(false);
                                    }
                                    else if (stage == 6)
                                    {
                                        stage = 5;
                                        name.text = playerName = "";
                                        nameText.SetActive(false);
                                        teams.SetActive(true);
                                        locked = false;
                                        keyObj.SetActive(true);
                                    }
                                }
                                pressed2 = true;
                            }
                        }
                        else pressed2 = false;



                        if (hasFocus && gamepad != null ? (gamepad.leftStick.y > 0.5) : false)
                        {
                            if (focusCharacter == false)
                            {
                                bool ok = true;
                                foreach (PlayerData data in FindObjectsOfType<PlayerData>())
                                {
                                    if (data.gameObject != gameObject && data.focusCharacter == false && data.buttonIndex == buttonIndex)
                                        ok = false;
                                }
                                if (ok)
                                {
                                    buttons[buttonIndex].Unfocus();
                                }
                                focusCharacter = true;
                                characterFocus.color = new Color32(0, 0, 0, 0);
                            }
                        }
                        else if (hasFocus && gamepad != null ? (gamepad.leftStick.y < -0.5) : false)
                        {
                            if (focusCharacter == true)
                            {
                                buttonIndex = 0;
                                buttons[0].Focus();
                                focusCharacter = false;
                                characterFocus.color = new Color32(0, 0, 0, 128);
                            }
                        }

                        if (hasFocus && ((index != 4 && stage == 1) || focusCharacter == false || (abil1Index != 7 && stage == 2) || stage == 6 || (teamIndex != 7 && stage == 5) || (abil2Index != 7 && stage == 3) || (abil3Index != 7 && stage == 4)) && ((gamepad != null ? (gamepad.leftStick.x > 0.5 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.D) && playerNumber == 5)) && canDo)
                        {
                            if (focusCharacter == false)
                            {
                                if (buttons[buttonIndex + 1].GetComponent<TextMeshProUGUI>().enabled && buttonIndex != 1)
                                {
                                    buttons[buttonIndex].Unfocus();
                                    buttonIndex++;
                                    if (buttonIndex > 1)
                                        buttonIndex = 1;
                                    buttons[buttonIndex].Focus();
                                }
                            }
                            else if (enabled)
                            {
                                t = 0;
                                if (stage == 1)
                                {
                                    prevIndex = index;
                                    index++;

                                    if (index > 4)
                                        index = 4;

                                    idCaracter = index + 1;
                                }
                                else if (stage == 2)
                                {
                                    prevAbil1Index = abil1Index;
                                    abil1Index++;

                                    if (abil1Index > 7)
                                        abil1Index = 7;

                                    abilitate1 = abil1Index + 1;
                                }
                                else if (stage == 3)
                                {
                                    prevAbil2Index = abil2Index;
                                    abil2Index++;

                                    if (abil2Index > 7)
                                        abil2Index = 7;

                                    abilitate2 = abil2Index + 1;
                                }
                                else if (stage == 4)
                                {
                                    prevAbil3Index = abil3Index;
                                    abil3Index++;

                                    if (abil3Index > 7)
                                        abil3Index = 7;

                                    abilitate3 = abil3Index + 1;
                                }
                                else if (stage == 5)
                                {
                                    prevTeamIndex = teamIndex;
                                    teamIndex++;

                                    if (teamIndex > 7)
                                        teamIndex = 7;

                                    echipa = teamIndex + 1;

                                    keyObj.SetActive(false);
                                }
                                else if (stage == 6)
                                {
                                    charIndex++;
                                    if (charIndex >= characters.Length)
                                        charIndex = 0;
                                    if (charIndex != 0)
                                        prevChar.text = characters[charIndex - 1].ToString();
                                    else prevChar.text = "Z";
                                    currentChar.text = characters[charIndex].ToString();
                                    if (charIndex != characters.Length - 1)
                                        nextChar.text = characters[charIndex + 1].ToString();
                                    else nextChar.text = "A";

                                    playerName = name.text;
                                }



                                moveForward = true;

                                canDo = false;
                                if (stage == 6)
                                    Invoke("CanDo", 0.2f);
                                else Invoke("CanDo", 0.4f);
                            }
                        }
                        else if (hasFocus && ((index != 0 && stage == 1) || focusCharacter == false || (abil1Index != 0 && stage == 2) || stage == 6 || (teamIndex != 0 && stage == 5) || (abil2Index != 0 && stage == 3) || (abil3Index != 0 && stage == 4)) && ((gamepad != null ? (gamepad.leftStick.x < -0.5 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.A) && playerNumber == 5)) && canDo)
                        {

                            if (playerNumber != 5 && focusCharacter == false)
                            {
                                if (buttonIndex != 0)
                                {
                                    buttons[buttonIndex].Unfocus();
                                    buttonIndex--;
                                    if (buttonIndex < 0)
                                        buttonIndex = 0;
                                    buttons[buttonIndex].Focus();
                                }
                            }
                            else if (enabled)
                            {
                                t = 0;
                                if (stage == 1)
                                {
                                    prevIndex = index;
                                    index--;

                                    if (index < 0)
                                        index = 0;

                                    idCaracter = index + 1;
                                }
                                else if (stage == 2)
                                {
                                    prevAbil1Index = abil1Index;
                                    abil1Index--;

                                    if (abil1Index < 0)
                                        abil1Index = 0;

                                    abilitate1 = abil1Index + 1;
                                }
                                else if (stage == 3)
                                {
                                    prevAbil2Index = abil2Index;
                                    abil2Index--;

                                    if (abil2Index < 0)
                                        abil2Index = 0;

                                    abilitate2 = abil2Index + 1;
                                }
                                else if (stage == 4)
                                {
                                    prevAbil3Index = abil3Index;
                                    abil3Index--;

                                    if (abil3Index < 0)
                                        abil3Index = 0;

                                    abilitate3 = abil3Index + 1;
                                }
                                else if (stage == 5)
                                {
                                    prevTeamIndex = teamIndex;
                                    teamIndex--;

                                    if (teamIndex < 0)
                                        teamIndex = 0;

                                    echipa = teamIndex + 1;
                                }
                                else if (stage == 6)
                                {
                                    charIndex--;

                                    if (charIndex < 0)
                                        charIndex = characters.Length - 1;
                                    if (charIndex != 0)
                                        prevChar.text = characters[charIndex - 1].ToString();
                                    else prevChar.text = "Z";
                                    currentChar.text = characters[charIndex].ToString();
                                    if (charIndex != characters.Length - 1)
                                        nextChar.text = characters[charIndex + 1].ToString();
                                    else nextChar.text = "A";

                                    playerName = name.text;
                                }


                                moveBackward = true;

                                canDo = false;
                                if (stage == 6)
                                    Invoke("CanDo", 0.2f);
                                else Invoke("CanDo", 0.4f);
                            }
                        }


                        if (moveForward)
                        {
                            t += PublicVariables.deltaTime * 3;
                            if (t >= 1)
                            {
                                t = 1;
                                moveForward = false;
                            }
                            if (stage == 1)
                                characterPos.transform.localPosition = new Vector2(Mathf.Lerp(characterXPos[prevIndex], characterXPos[index], t), characterPos.transform.localPosition.y);
                            else if (stage == 2)
                                abil1Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil1Index], abilPositions[abil1Index], t), abil1Pos.transform.localPosition.y);
                            else if (stage == 3)
                                abil2Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil2Index], abilPositions[abil2Index], t), abil2Pos.transform.localPosition.y);
                            else if (stage == 4)
                                abil3Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil3Index], abilPositions[abil3Index], t), abil3Pos.transform.localPosition.y);
                            else if (stage == 5)
                                teamPos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevTeamIndex], abilPositions[teamIndex], t), teamPos.transform.localPosition.y);
                        }
                        if (moveBackward)
                        {
                            t += PublicVariables.deltaTime * 3;
                            if (t >= 1)
                            {
                                t = 1;
                                moveBackward = false;
                            }
                            if (stage == 1)
                                characterPos.transform.localPosition = new Vector2(Mathf.Lerp(characterXPos[prevIndex], characterXPos[index], t), characterPos.transform.localPosition.y);
                            else if (stage == 2)
                                abil1Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil1Index], abilPositions[abil1Index], t), abil1Pos.transform.localPosition.y);
                            else if (stage == 3)
                                abil2Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil2Index], abilPositions[abil2Index], t), abil2Pos.transform.localPosition.y);
                            else if (stage == 4)
                                abil3Pos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevAbil3Index], abilPositions[abil3Index], t), abil3Pos.transform.localPosition.y);
                            else if (stage == 5)
                                teamPos.transform.localPosition = new Vector2(Mathf.Lerp(abilPositions[prevTeamIndex], abilPositions[teamIndex], t), teamPos.transform.localPosition.y);
                        }
                    }
                }
                else if (onPhone)
                    ret = false;
            }
            else
            {
                if (enabled)
                {
                    try
                    {

                    }
                    catch
                    {

                    }
                }
                if (SceneManager.GetActiveScene().name.Contains("Select"))
                {
                    if (canvas != null)
                    {
                        if (canvas.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("transition") == false)
                        {
                            if (((gamepad != null ? (gamepad.A == 1 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.E) && playerNumber == 5)) && (SceneManager.GetActiveScene().name == "Select" || SceneManager.GetActiveScene().name == "SelectDungeon"))
                            {
                                if (pressed == false)
                                {
                                    Enter();
                                    pressed = true;
                                }
                            }
                            else pressed = false;

                            if (((gamepad != null ? (gamepad.B == 1 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.Escape) && playerNumber == 5)) && (SceneManager.GetActiveScene().name == "Select" || SceneManager.GetActiveScene().name == "SelectDungeon"))
                            {
                                if (pressed2 == false)
                                {
                                    Return();
                                    pressed2 = true;
                                }
                            }
                            else pressed2 = false;

                            if (hasFocus && (index != 4 || focusCharacter == false) && ((gamepad != null ? (gamepad.leftStick.x > 0.5 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.D) && playerNumber == 5)) && canDo)
                            {
                                if (playerNumber != 5 && focusCharacter == false)
                                {
                                    if (buttons[buttonIndex + 1].GetComponent<TextMeshProUGUI>().enabled && buttonIndex != 1)
                                    {
                                        buttons[buttonIndex].Unfocus();
                                        buttonIndex++;
                                        if (buttonIndex > 1)
                                            buttonIndex = 1;
                                        buttons[buttonIndex].Focus();
                                    }
                                }
                                else Right();
                            }
                            else if (hasFocus && (index != 0 || focusCharacter == false) && ((gamepad != null ? (gamepad.leftStick.x < -0.5 && playerNumber != 5) : false) || (Input.GetKey(KeyCode.A) && playerNumber == 5)) && canDo)
                            {

                                if (playerNumber != 5 && focusCharacter == false)
                                {
                                    if (buttonIndex != 0)
                                    {
                                        buttons[buttonIndex].Unfocus();
                                        buttonIndex--;
                                        if (buttonIndex < 0)
                                            buttonIndex = 0;
                                        buttons[buttonIndex].Focus();
                                    }
                                }
                                else Left();
                            }
                            if (hasFocus && gamepad != null ? (gamepad.leftStick.y > 0.5) : false)
                            {
                                if (focusCharacter == false)
                                {
                                    bool ok = true;
                                    foreach (PlayerData data in FindObjectsOfType<PlayerData>())
                                    {
                                        if (data.gameObject != gameObject && data.focusCharacter == false && data.buttonIndex == buttonIndex)
                                            ok = false;
                                    }
                                    if (ok)
                                    {
                                        buttons[buttonIndex].Unfocus();
                                    }
                                    focusCharacter = true;
                                    characterFocus.color = new Color32(0, 0, 0, 0);
                                }
                            }
                            else if (hasFocus && gamepad != null ? (gamepad.leftStick.y < -0.5) : false)
                            {
                                if (focusCharacter == true)
                                {
                                    buttonIndex = 0;
                                    buttons[0].Focus();
                                    focusCharacter = false;
                                    characterFocus.color = new Color32(0, 0, 0, 128);
                                }
                            }
                        }
                        if (moveForward)
                        {
                            t += PublicVariables.deltaTime * 3;
                            if (t >= 1)
                            {
                                t = 1;
                                moveForward = false;
                            }
                            characterPos.transform.localPosition = new Vector2(Mathf.Lerp(characterXPos[prevIndex], characterXPos[index], t), characterPos.transform.localPosition.y);
                        }
                        if (moveBackward)
                        {
                            t += PublicVariables.deltaTime * 3;
                            if (t >= 1)
                            {
                                t = 1;
                                moveBackward = false;
                            }
                            characterPos.transform.localPosition = new Vector2(Mathf.Lerp(characterXPos[prevIndex], characterXPos[index], t), characterPos.transform.localPosition.y);
                        }
                    }

                }
                else if (onPhone) ret = false;
            }
        }
        if (SceneManager.GetActiveScene().name != "Select" && SceneManager.GetActiveScene().name != "SelectDungeon" && onPhone == false)
            enabled = true;
    }

    public void Left()
    {
        if (enabled && locked == false && canDo)
        {

            t = 0;
            prevIndex = index;
            index--;

            if (index < 0)
                index = 0;

            idCaracter = index + 1;

            moveBackward = true;

                canDo = false;
            Invoke("CanDo", 0.4f);
        }
    }

    public void Right()
    {
        if (enabled && locked == false && canDo)
        {
            t = 0;
            prevIndex = index;
            index++;

            if (index > 4)
                index = 4;

            idCaracter = index + 1;

            moveForward = true;

                canDo = false;
            Invoke("CanDo", 0.4f);
        }
    }

    public void Return()
    {
        if (enabled == true)
        {
            if (locked)
            {
                locked = false;
                keyObj.SetActive(true);
            }
            else
            {
                enabled = false;
                if (objs != null)
                    objs.SetActive(false);
            }
        }
    }

    public void Enter()
    {
        if (focusCharacter == false && playerNumber != 5)
        {
            if (buttons[buttonIndex].GetComponent<TextMeshProUGUI>().enabled)
            {
                buttons[buttonIndex].Activate();
            }
        }
        else
        {
            if (enabled == false)
            {
                AudioSource.PlayClipAtPoint(playerEnter, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                enabled = true;
                if (objs != null)
                    objs.SetActive(true);
            }
            else if (locked == false)
            {
                bool ok = true;
                foreach (PlayerData data in FindObjectsOfType<PlayerData>())
                {
                    if ((data.enabled && data.locked && data.idCaracter == idCaracter))
                    {
                        ok = false;
                    }
                }
                if (ok)
                {
                    AudioSource.PlayClipAtPoint(playerLock, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                    keyObj.SetActive(false);
                    locked = true;
                    objs.GetComponent<Animator>().Play("lock", 0, 0f);
                }
            }
        }
    }

    public Image characterFocus;
    public GameObject keyObj;
    bool moveBackward;
    bool moveForward;
    public GameObject characterPos;
    public GameObject teamPos;
    public int[] characterXPos;
    int prevIndex = 0;
    int index = 0;

    public int[] abilPositions;
    int prevAbil1Index = 0;
    int abil1Index = 0;

    int prevAbil2Index = 0;
    int abil2Index = 0;

    int prevAbil3Index = 0;
    int abil3Index = 0;

    int prevTeamIndex = 0;
    int teamIndex = 0;

    float t = 0;

    public bool locked = false;

    bool canDo = true;

    void CanDo()
    {
        canDo = true;
    }
}
