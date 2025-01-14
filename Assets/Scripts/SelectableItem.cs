using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectableItem : MonoBehaviour
{
    public PhoneController phoneController;
    public PhoneAbillities use;
    public PhoneAbillities drop;

    public List<player> characters;

    public SelectableItem up;
    public SelectableItem right;
    public SelectableItem down;
    public SelectableItem left;
    public Image focusSprite;
    public bool active = false;
    public player player;
    public GameObject plus;
    public Text level;
    public int lvl = 1;
    public int abilNumber;

    AudioClip upgrade;

    CameraFollow cameraFollow;
    CharacterSlot characterSlot;
    PlayerData playerData;

    AudioSource soundSource;

    private void Start()
    {
        upgrade = Resources.Load("Sounds\\upgrade") as AudioClip;
        GameObject newObj = new GameObject();
        newObj.transform.parent = Camera.main.transform;
        soundSource = newObj.AddComponent<AudioSource>();
        soundSource.playOnAwake = false;
        soundSource.clip = upgrade;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        characterSlot = GetComponentInParent<CharacterSlot>();
        playerData = FindObjectOfType<PlayerData>();
    }

    public void Activate()
    {
        focusSprite.enabled = true;
        StartCoroutine(ActivateJoy());
    }
    IEnumerator ActivateJoy()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        active = true;
    }
    public void Deactivate()
    {
        active = false;
        focusSprite.enabled = false;
        timeHeld = 0;
        if(abilityDescription != null)
            abilityDescription.SetActive(false);
    }
    private void OnDisable()
    {
        timeHeld = 0;
        if (focusSprite.enabled)
            active = true;
        if (abilityDescription != null)
            abilityDescription.SetActive(false);
    }

    public GameObject abilityDescription;
    public bool isAbility = false;
    float timeHeld = 0;
    bool button1 = false;
    bool button2 = false;
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if ((player.abilityPoints >= 1 && lvl < 6) || (player.masterPoints >= 1 && lvl == 6))
            {
                plus.SetActive(true);
            }
            else plus.SetActive(false);
        }
        if(isAbility && active)
        {
            timeHeld += Time.unscaledDeltaTime;
            if(timeHeld>=1.5f)
            {
                abilityDescription.SetActive(true);
                try
                {
                    abilityDescription.GetComponentInChildren<Image>().raycastTarget = false; abilityDescription.GetComponentInChildren<Text>().raycastTarget = false;
                }
                catch
                {

                }


                if (TutorialScene.instance != null)
                {
                    if (TutorialScene.instance.inventory.gameObject.activeInHierarchy)
                    {
                        Tutorial tutorial = TutorialScene.instance.inventory.GetComponentInChildren<Tutorial>();
                        if (tutorial.textIndex == 2)
                            tutorial.NextQuote();
                    }
                }
            }
            else abilityDescription.SetActive(false);
        }
        if (active)
        {
            bool ok1 = false;
            bool ok2 = false;
            List<player> toAdd = new List<player>();
            foreach (player player in cameraFollow.playerScripts)
            {
                if (player != null)
                {

                    if (player.bot == false)
                    {
                        if (playerData.hasFocus && (player.gamePad != null ? (player.gamePad.A == 1 && player.playerNumber != 5) : false) || ((Input.GetKey(KeyCode.E) && player.playerNumber == 5 && player.keyboard)||(use.attack && player.playerNumber == 5 && player.keyboard == false)))
                        {
                            if (button1 == false)
                            {
                                if (level != null && plus.activeInHierarchy)
                                {
                                    bool mastery = false;
                                    soundSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                                    soundSource.Play();
                                    lvl++;

                                    if (TutorialScene.instance != null)
                                    {
                                        if (TutorialScene.instance.inventory.gameObject.activeInHierarchy)
                                        {
                                            Tutorial tutorial = TutorialScene.instance.inventory.GetComponentInChildren<Tutorial>();

                                                tutorial.SetQuote(3);
                                        }
                                    }

                                    if (lvl == 6)
                                    {
                                        level.text = "M";
                                        this.player.abilityPoints--;

                                        DungeonData.instance.AddToQuest(16, 1);
                                    }
                                    else if (lvl == 7)
                                    {
                                        level.text = "P";
                                        mastery = true;
                                        this.player.masterPoints--;

                                        DungeonData.instance.AddToQuest(15, 1);
                                    }
                                    else
                                    {
                                        level.text = lvl.ToString();
                                        this.player.abilityPoints--;
                                    }
                                    if (this.player.GetComponent<wizard>() != null)
                                    {
                                        this.player.GetComponent<wizard>().LevelAbil(abilNumber - 1, mastery);
                                    }
                                    else if (this.player.GetComponent<knight>() != null)
                                    {
                                        this.player.GetComponent<knight>().LevelAbil(abilNumber - 1, mastery);
                                    }
                                    else if (this.player.GetComponent<archer>() != null)
                                    {
                                        this.player.GetComponent<archer>().LevelAbil(abilNumber - 1, mastery);
                                    }
                                    else if (this.player.GetComponent<Tank>() != null)
                                    {
                                        this.player.GetComponent<Tank>().LevelAbil(abilNumber - 1, mastery);
                                    }
                                    else if (this.player.GetComponent<support>() != null)
                                    {
                                        this.player.GetComponent<support>().LevelAbil(abilNumber - 1, mastery);
                                    }

                                    plus.SetActive(false);
                                    if (this.player != null)
                                    {
                                        if ((this.player.abilityPoints >= 1 && lvl < 6) || (this.player.masterPoints >= 1 && lvl == 6))
                                        {
                                            plus.SetActive(true);
                                        }
                                        else plus.SetActive(false);
                                    }

                                    characterSlot.APText.text = "AP: " + this.player.abilityPoints;
                                    characterSlot.MPText.text = "MP: " + this.player.masterPoints;
                                }
                                else if (containsItem)
                                {
                                    if (itemType == 2)
                                    {
                                        foreach (player character in cameraFollow.playerScripts)
                                        {
                                            if(character != null)
                                            character.masterPoints++;
                                        }
                                        foreach (player dead in FindObjectOfType<DeadMates>().players)
                                        {
                                            if (dead != null)
                                                dead.masterPoints++;
                                        }
                                        FindObjectOfType<Inventory>().Enable();
                                        RemoveItem();
                                    }
                                    else if (itemType == 9)
                                    {
                                        foreach (player character in cameraFollow.playerScripts)
                                        {
                                            if (character != null)
                                            {
                                                character.AddLevels(Random.Range(1, 999999) % 3+1);
                                            }
                                        }
                                        foreach (player dead in FindObjectOfType<DeadMates>().players)
                                        {
                                            if(dead != null)
                                                dead.AddLevels(Random.Range(1, 999999) % 3 + 1);
                                        }
                                        FindObjectOfType<Inventory>().Enable();
                                        RemoveItem();
                                    }
                                    else if (itemType == 10)
                                    {
                                        FindObjectOfType<Inventory>().Enable();
                                        RemoveItem();
                                        int rand = Random.Range(1, 999999)%11;
                                        if (rand > 7)
                                            rand += 2;
                                        FindObjectOfType<InventorySpawn>().AddItem(rand);

                                        DungeonData.instance.AddToQuest(29, 1);
                                    }
                                    else if (itemType == 4)
                                    {
                                        foreach (player character in cameraFollow.playerScripts)
                                        {

                                            if (character != null)
                                            {
                                                if(character.GetAnimation.GetCurrentAnimatorStateInfo(0).IsName("death"))
                                                {
                                                    character.Revive();
                                                    character.SetHpPercent(0);
                                                    character.HealPercent(0.375f);
                                                }
                                                else character.HealPercent(0.75f);
                                            }
                                        }
                                        foreach(PlayerData data in FindObjectsOfType<PlayerData>())
                                        {
                                            bool ok = true;
                                            if(data.enabled)
                                            {
                                                foreach (player character in cameraFollow.playerScripts)
                                                {
                                                    if (character.caracterId == data.idCaracter)
                                                        ok = false;
                                                }
                                                if(ok)
                                                {

                                                    player deadPlayer = null;
                                                    foreach(player dead in FindObjectOfType<DeadMates>().players)
                                                    {
                                                        if (dead.caracterId == data.idCaracter)
                                                            deadPlayer = dead;
                                                    }
                                                    if (deadPlayer != null)
                                                    {
                                                        deadPlayer.gameObject.SetActive(true);
                                                        foreach (player character in cameraFollow.playerScripts)
                                                        {
                                                            if (character != null)
                                                            {
                                                                deadPlayer.transform.position = character.transform.position;
                                                                break;
                                                            }
                                                        }
                                                        deadPlayer.Revive();
                                                        deadPlayer.SetHpPercent(0);
                                                        deadPlayer.HealPercent(0.375f);

                                                        FindObjectOfType<DeadMates>().players.Remove(deadPlayer);

                                                        toAdd.Add(deadPlayer);
                                                    }
                                                }
                                            }
                                        }
                                        FindObjectOfType<Inventory>().Enable();
                                        RemoveItem();
                                    }
                                }
                            }
                            ok1 = true;
                        }
                        if (playerData.hasFocus && (player.gamePad != null ? (player.gamePad.B == 1 && player.playerNumber != 5) : false && player.playerNumber != 5) || (((Input.GetKey(KeyCode.G) && player.playerNumber == 5 && player.keyboard) || (drop.attack && player.playerNumber == 5 && player.keyboard == false))))
                        {
                            if (containsItem)
                            {
                                if (button2 == false)
                                    RemoveItem();
                                ok2 = true;
                            }
                        }
                        if (up != null)
                        {
                            if (up.right != null)
                            {
                                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5)||(Input.GetKey(KeyCode.D) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && ((player.gamePad != null ? (player.gamePad.leftStick.y > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.W) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && up.right.gameObject.activeInHierarchy)
                                {
                                    up.right.Activate();
                                    Deactivate();
                                    break;
                                }
                            }
                            if (up.left != null)
                            {
                                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.A) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && ((player.gamePad != null ? (player.gamePad.leftStick.y > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.W) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && up.left.gameObject.activeInHierarchy)
                                {
                                    up.left.Activate();
                                    Deactivate();
                                    break;
                                }
                            }
                        }

                        if (down != null)
                        {
                            if (down.right != null)
                            {
                                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.D) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && ((player.gamePad != null ? (player.gamePad.leftStick.y < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.S) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && down.right.gameObject.activeInHierarchy)
                                {
                                    down.right.Activate();
                                    Deactivate();
                                    break;
                                }
                            }
                            if (down.left != null)
                            {
                                if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.A) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && ((player.gamePad != null ? (player.gamePad.leftStick.y < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.S) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && down.left.gameObject.activeInHierarchy)
                                {
                                    down.left.Activate();
                                    Deactivate();
                                    break;
                                }
                            }
                        }
                        if (right != null)
                        {
                        if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.D) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && right.gameObject.activeInHierarchy)
                            {
                            right.Activate();
                                Deactivate();
                                break;
                            }
                        }
                        if (left != null)
                        {
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.x < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.A) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.x < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && left.gameObject.activeInHierarchy)
                            {
                                left.Activate();
                                Deactivate();
                                break;
                            }
                        }
                        if (down != null)
                        {
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.y < -0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.S) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y < -0.5 && player.playerNumber == 5 && player.keyboard == false)) && down.gameObject.activeInHierarchy)
                            {
                                down.Activate();
                                Deactivate();
                                break;
                            }
                        }
                        if (up != null)
                        {
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftStick.y > 0.5 && player.playerNumber != 5) : false && player.playerNumber != 5) || (Input.GetKey(KeyCode.W) && player.playerNumber == 5 && player.keyboard)||(phoneController.dir.y > 0.5 && player.playerNumber == 5 && player.keyboard == false)) && up.gameObject.activeInHierarchy)
                            {
                                up.Activate();
                                Deactivate();
                                break;
                            }
                        }
                    }
                }
            }
            button1 = ok1;
            button2 = ok2;
            foreach(player player in toAdd)
            {
                cameraFollow.players.Add(player.gameObject);
                cameraFollow.playerScripts.Add(player);
            }
            }
        }
    public bool containsItem = false;
    public Image item;
    public int itemType = -1;
    public int usesLeft;
    public void RemoveItem()
    {
        item.sprite = null;
        item.gameObject.SetActive(false);
        isAbility = false;
        abilityDescription.SetActive(false);
        abilityDescription.GetComponentInParent<AbilityDesc>().abilityText.text = "";
        if (itemType == 6)
        {
            foreach (player player in cameraFollow.playerScripts)
            {
                if (player != null)
                {
                    if (player.bot == false)
                    {
                        player.RemoveStatBoost();
                    }
                }
            }
        }
        itemType = -1;
        containsItem = false;

    }
}

