using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lidgren.Network;
using System.Linq;

public class GameClient : MonoBehaviour
{
    public string ip;
    NetPeerConfiguration config;
    public NetClient client;

    public int playerId;

    public InputField idCaracterField;
    public InputField abilitate1Field;
    public InputField abilitate2Field;
    public InputField abilitate3Field;
    public InputField nameField;
    public InputField teamField;

    public GameObject waitingForServer;
    public Text playerCount;
    public GameObject startGame;

    public GameObject[] players;

    public bool upnp = false;
    // Start is called before the first frame update
    public void Disconnect()
    {
        var message3 = client.CreateMessage();
        message3.Write("Disconnect");
        
        client.SendMessage(message3,
        NetDeliveryMethod.ReliableOrdered);
    }
    public void Begin()
    {
        if (FindObjectsOfType<GameClient>().Length >= 2)
            Destroy(gameObject);
        else
        {
       
            players = new GameObject[999];
            config = new NetPeerConfiguration("RpgGame");
            config.ConnectionTimeout = 16;
            config.EnableUPnP = true;
            client = new NetClient(config);
            client.Start();
            int attempts = 0;
            while (true)
            {
                bool response = client.UPnP.ForwardPort(25566, "RpgGame");
                upnp = response;
                attempts++;
                if (attempts == 6 || response)
                    break;
            }
            client.Connect(host: ip, port: 25566);
            Invoke("TestConnection", 3.5f);
            Invoke("SendMessage", 0.25f);
            Invoke("ActivateMessage", 0.05f);
        }
    }

    bool canSendMsg = false;
    void ActivateMessage()
    {
        canSendMsg = true;
        Invoke("ActivateMessage", 0.05f);
    }
    void TestConnection()
    {
        if (playerId == 0)
        {
            var message = client.CreateMessage();
            message.Write("Disconnect");
            client.SendMessage(message,
            NetDeliveryMethod.ReliableOrdered);
            Destroy(gameObject);
        }
    }

    string mesaj = "";

    void SendFullMessage()
    {
        try
        {
            mesaj += currentPos + "\n";
            mesaj += currentRot + "\n";
            mesaj += currentAnimation + "\n";
            var message = client.CreateMessage();
            message.Write(mesaj);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            mesaj = "";
            currentAnimation = "";
            currentPos = "";
            currentRot = "";
            canSendMsg = false;
        }
        catch
        {
            mesaj = "";
        }
    }

    void SendMessage()
    {

        mesaj += "NewPlayer" + "\n";
    }

    void SendIdChange()
    {
        mesaj += "NewCharacterId " + playerId + ' ' + idCaracterField.text + "\n";
    }
    void SendAbility1Change()
    {
        mesaj += "NewAbility1 " + playerId + ' ' + abilitate1Field.text + "\n";
    }
    void SendAbility2Change()
    {
        mesaj += "NewAbility2 " + playerId + ' ' + abilitate2Field.text + "\n";
    }
    void SendAbility3Change()
    {
        mesaj += "NewAbility3 " + playerId + ' ' + abilitate3Field.text + "\n";
    }
    void SendNameChange()
    {
        mesaj += "NewCharacterName " + playerId + ' ' + nameField.text + "\n";
    }
    void SendTeamChange()
    {
        mesaj += "NewCharacterTeam " + playerId + ' ' + teamField.text + "\n";
    }
    bool detected = false;
    public bool canDo = false;
    float timeBetweenResponses = 0;
    string currentAnimation = "";
    string currentPos = "";
    string currentRot = "";
    public void Send(string message)
    {
        if(message.Contains("PlayerTarget"))
        {
            currentPos = message;
        }
        else if (message.Contains("PlayerRot"))
        {
            currentRot = message;
        }
        else if (message.Contains("PlayerAnim"))
        {
            if (currentAnimation != "")
            {
                if (message.Contains("idle") == false && message.Contains("run") == false)
                {
                    currentAnimation = message;
                }
                else
                {
                    if(currentAnimation.Contains("idle") == true || currentAnimation.Contains("run") == true)
                    {
                        currentAnimation = message;
                    }
                }
            }
            else currentAnimation = message;
        }
        else if (canSendMsg)
            mesaj += message + "\n";
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            idCaracterField = GameObject.Find("IdField").GetComponent<InputField>();
            abilitate1Field = GameObject.Find("Ability1Field").GetComponent<InputField>();
            abilitate2Field = GameObject.Find("Ability2Field").GetComponent<InputField>();
            abilitate3Field = GameObject.Find("Ability3Field").GetComponent<InputField>();
            nameField = GameObject.Find("NameField").GetComponent<InputField>();
            teamField = GameObject.Find("TeamField").GetComponent<InputField>();

                idCaracterField.onValueChanged.AddListener(delegate { SendIdChange(); });
                abilitate1Field.onValueChanged.AddListener(delegate { SendAbility1Change(); });
                abilitate2Field.onValueChanged.AddListener(delegate { SendAbility2Change(); });
                abilitate3Field.onValueChanged.AddListener(delegate { SendAbility3Change(); });
                nameField.onValueChanged.AddListener(delegate { SendNameChange(); });
                teamField.onValueChanged.AddListener(delegate { SendTeamChange(); });
            
        }
        catch
        {

        }
        if (client != null)
        {
            try
            {
                if (FindObjectOfType<GameHost>() != null)
                {
                    GameObject.Find("Start").SetActive(true);
                    GameObject.Find("Waiting...").SetActive(false);
                }
                else
                {
                    GameObject.Find("Start").SetActive(false);
                    GameObject.Find("Waiting...").SetActive(true);
                }
            }
            catch
            {

            }
            timeBetweenResponses += PublicVariables.deltaTime;
            if (timeBetweenResponses >= 10f)
            {
                SceneManager.LoadScene("OnlineSelect");
                mesaj += "Disconnect" + "\n";
                Destroy(gameObject);
            }
            List<NetIncomingMessage> messages = new List<NetIncomingMessage>();
            client.ReadMessages(messages);
            foreach (NetIncomingMessage message in messages)
            {
                string[] lines = message.ReadString().Split('\n');

                foreach (string line in lines)
                {

                    try
                    {
                        string data = line;
                        string type = data.Split(' ')[0];
                        if (type == "ReceiveId")
                        {
                            playerId = int.Parse(data.Split(' ')[1]);
                        }
                        else if (type == "ServerAlive")
                        {
                            timeBetweenResponses = 0;
                        }
                        else if (type == "PlayerCount")
                        {
                            playerCount = GameObject.Find("PlayerCount").GetComponent<Text>();
                            playerCount.text = "Player Count: " + data.Split(' ')[1];
                        }
                        else if (type == "StartGame")
                        {
                            if (data.Split(' ')[1] == "OnlineLobby")
                                SceneManager.LoadScene("OnlineLobby");
                            else SceneManager.LoadScene("OnlineFight" + data.Split(' ')[1]);
                        }
                        else if (type == "LoadScene")
                        {
                            SceneManager.LoadScene(data.Split(' ')[1]);
                        }
                        else if (type == "Disconnect")
                        {
                            SceneManager.LoadScene("OnlineSelect");

                            Destroy(gameObject);
                        }
                        else if (type == "SpawnPlayer")
                        {
                            try
                            {

                                GameObject player = null;
                                int character = int.Parse(data.Split(' ')[2]) - 1;
                                GameObject characterToInstantiate = null;
                                if (character == 0)
                                    player = Instantiate(GameObject.Find("wizard"));
                                if (character == 1)
                                    player = Instantiate(GameObject.Find("knight"));
                                if (character == 2)
                                    player = Instantiate(GameObject.Find("archer"));
                                if (character == 3)
                                    player = Instantiate(GameObject.Find("Tank"));
                                if (character == 4)
                                    player = Instantiate(GameObject.Find("Support"));

                                player.transform.position = new Vector3(float.Parse(data.Split(' ')[4]), float.Parse(data.Split(' ')[5]));
                                player.GetComponent<player>().team = int.Parse(data.Split(' ')[3]);
                                player.GetComponent<player>().name.text = data.Split(' ')[6];
                                player.GetComponent<player>().playerNumber = int.Parse(data.Split(' ')[1]);
                                player.GetComponent<player>().attacks[0] = "attack" + data.Split(' ')[7];
                                player.GetComponent<player>().attacks[1] = "attack" + data.Split(' ')[8];
                                player.GetComponent<player>().attacks[2] = "attack" + data.Split(' ')[9];
                                player.GetComponent<player>().online = true;
                                if (int.Parse(data.Split(' ')[1]) != playerId)
                                    player.GetComponent<player>().controlling = false;
                                player.GetComponent<player>().Awake();
                                players[int.Parse(data.Split(' ')[1])] = player;
                            }
                            catch
                            {

                            }
                        }
                        else if (type == "SetVelocity")
                        {
                            string[] parameters = data.Split(' ');

                            players[int.Parse(parameters[1])].GetComponent<player>().GetVelocity(float.Parse(parameters[2]), float.Parse(parameters[3]));
                        }

                        else if (type == "PlayerAbil")
                        {
                            string[] parameters = data.Split(' ');

                            int id = int.Parse(parameters[1]);

                            players[id].GetComponent<player>().abilImages[0].gameObject.SetActive(bool.Parse(parameters[2]));
                            players[id].GetComponent<player>().abilImages[1].gameObject.SetActive(bool.Parse(parameters[3]));
                            players[id].GetComponent<player>().abilImages[2].gameObject.SetActive(bool.Parse(parameters[4]));

                        }
                        else if (type == "PlayerAnim")
                        {
                            string[] parameters = data.Split(' ');
                            int id = int.Parse(parameters[1]);
                            if (data.Contains("hurt"))
                            {
                                Vector2 direction = new Vector2(float.Parse(parameters[3]), float.Parse(parameters[4]));
                                players[id].GetComponent<Rigidbody2D>().velocity = direction;

                                players[id].transform.rotation = Quaternion.LookRotation(-direction * 100, new Vector3(0, 0, 1));
                                players[id].transform.rotation = new Quaternion(0, 0, players[id].transform.rotation.z, players[id].transform.rotation.w);
                            }
                            players[id].GetComponent<player>().PlayAnim(parameters[2]);

                        }
                        else if (type == "PlayerPos" && FindObjectOfType<GameHost>() == null)
                        {
                            string[] parameters = data.Split(' ');

                            int id = int.Parse(parameters[1]);

                            Vector2 playerPos = new Vector2(float.Parse(parameters[2]), float.Parse(parameters[3]));

                            if (Vector2.Distance(players[id].transform.position, playerPos) > 0.1f)
                            {
                                players[id].GetComponent<player>().interpolationCurrentPos = players[id].transform.position;
                                players[id].GetComponent<player>().interpolationTime = 0;
                                players[id].GetComponent<player>().interpolationTarget = playerPos;
                            }
                            players[id].transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[4]));
                            players[id].GetComponent<player>().health = float.Parse(parameters[5]);
                            players[id].GetComponent<player>().hpBar.transform.localScale = new Vector3(players[id].GetComponent<player>().health / players[id].GetComponent<player>().maxHealth, 1, 1);
                            players[id].GetComponent<player>().abilImages[0].gameObject.SetActive(bool.Parse(parameters[6]));
                            players[id].GetComponent<player>().abilImages[1].gameObject.SetActive(bool.Parse(parameters[7]));
                            players[id].GetComponent<player>().abilImages[2].gameObject.SetActive(bool.Parse(parameters[8]));
                            if (players[id].GetComponent<player>().timeTillStop >= 1f && bool.Parse(parameters[9]))
                                players[id].GetComponent<player>().canAttack = bool.Parse(parameters[9]);
                            else players[id].GetComponent<player>().canAttack = bool.Parse(parameters[9]);
                            players[id].GetComponent<player>().networkCanAttack = bool.Parse(parameters[10]);
                            players[id].GetComponent<player>().detectAttack = bool.Parse(parameters[11]);
                            players[id].GetComponent<player>().networkDetect = bool.Parse(parameters[12]);
                            players[id].GetComponent<player>().canAttackButWithMovement = bool.Parse(parameters[13]);
                            players[id].GetComponent<player>().spamDirection = bool.Parse(parameters[14]);
                            players[id].GetComponent<player>().health = int.Parse(parameters[15]);
                            players[id].GetComponent<player>().hpBar.transform.localScale = new Vector3(players[id].GetComponent<player>().health / players[id].GetComponent<player>().maxHealth, players[id].GetComponent<player>().hpBar.transform.localScale.y, 1);
                            players[id].GetComponent<player>().iceParticles.SetActive(bool.Parse(parameters[16]));
                            players[id].GetComponent<player>().fireParticles.SetActive(bool.Parse(parameters[17]));
                            players[id].GetComponent<player>().stunObj.SetActive(bool.Parse(parameters[18]));

                            players[id].GetComponent<player>().movementDirection = new Vector2(float.Parse(parameters[19]), float.Parse(parameters[20]));
                        }
                    }
                    catch
                    {

                    }

                }


            }
        }
        if(canSendMsg)
            SendFullMessage();
        
    }
}
