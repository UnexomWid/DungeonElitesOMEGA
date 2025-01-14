using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lidgren.Network;
using System.Linq;

public class GameHost : MonoBehaviour
{
    public List<NetConnection> clientConnections;
    public int players = 1;
    NetPeerConfiguration config;
    public NetServer server;
    public List<int> playerCharacterIds;
    public List<int> playerFirstAbility;
    public List<int> playerSecondAbility;
    public List<int> playerThirdAbility;
    public List<string> playerNames;
    public List<int> playerTeams;
    // Start is called before the first frame update
    public void Disconnect()
    {
        var toAll = server.CreateMessage();
        toAll.Write("Disconnect");
        server.SendMessage(toAll, recipients: clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
        foreach (NetConnection client in clientConnections)
            client.Disconnect("exited");
    }
    public bool upnp = false;
    public void Begin()
    {
        if (FindObjectsOfType<GameHost>().Length >= 2)
            Destroy(gameObject);
        else
        {
            SceneManager.LoadScene("OnlineLobby");
            playerCharacterIds = new List<int>();
            playerFirstAbility = new List<int>();
            playerSecondAbility = new List<int>();
            playerThirdAbility = new List<int>();
            playerNames = new List<string>();
            playerTeams = new List<int>();
            clientConnections = new List<NetConnection>();
            config = new NetPeerConfiguration("RpgGame")
            { Port = 25566, ConnectionTimeout = 16, EnableUPnP = true};
            server = new NetServer(config);
            server.Start();
            int attempts = 0;
            Invoke("ActivateMessage", 0.05f);
            /*while(true)
            {
                bool response = server.UPnP.ForwardPort(25566, "RpgGame");
                attempts++;
                upnp = response;
                if (attempts == 6 || response)
                    break;
            }
            */
        }
    }

    bool ableToSend = false;

    void ActivateMessage()
    {
        ableToSend = true;
        Invoke("ActivateMessage", 0.05f);
    }

    string mesaj = "";
    bool canSendMsg = false;
    void SendMessage()
    {
        canSendMsg = true;
        try
        {
            var message = server.CreateMessage();
            message.Write(mesaj);
            server.SendMessage(message, clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
            mesaj = "";
            ableToSend = false;
        }
        catch
        {
            mesaj = "";
        }
    }
    private void OnApplicationQuit()
    {
        server.Shutdown("Server ShutDown");
    }
    bool detected = false;
    bool sentPlayers = false;
    // Update is called once per frame
    public void StartGame()
    {
        if (players >= 3)
        {
            mesaj += "StartGame " + "1\n";
            sentPlayers = false;
        }
    }
    string currentScene = "OnlineFight";
    void Update()
    {
        if (sentPlayers)
        {
            List<int> teams = new List<int>();
            foreach (player player in FindObjectsOfType<player>())
            {
                if (player.online)
                {
                    if (teams.Contains(player.team) == false && player.gameObject != gameObject)
                        teams.Add(player.team);
                }

            }
            if (teams.Count == 1)
            {
                if (SceneManager.GetActiveScene().name == "OnlineFight1")
                {
                    mesaj+="StartGame " + 2 + "\n";
                    
                }
                else if (SceneManager.GetActiveScene().name == "OnlineFight2")
                {
                    mesaj += "StartGame " + 3 + "\n";
                }
                else if (SceneManager.GetActiveScene().name == "OnlineFight3")
                {

                    mesaj += "StartGame " + 4 + "\n";
                }
                else if (SceneManager.GetActiveScene().name == "OnlineFight4")
                {
                    mesaj += "StartGame " + "OnlineLobby" + "\n";
                }
                sentPlayers = false;
            }
        }
        if (clientConnections.Count != 0)
        {
            if(ableToSend)
            mesaj += "ServerAlive" + "\n";
        }
        try
        {
            if (SceneManager.GetActiveScene().name == "OnlineLobby")
            {
                if (ableToSend)
                    mesaj += "PlayerCount " + (players - 1) + "\n";
            }
        }
        catch
        {

        }
        if(SceneManager.GetActiveScene().name.Contains("OnlineFight") && sentPlayers == false && currentScene != SceneManager.GetActiveScene().name)
        {
            for (int i = 1; i < players; i++)
            {
                try
                {
                    mesaj += "SpawnPlayer " + i + " " + playerCharacterIds[i - 1] + " " + playerTeams[i - 1] + " " + Random.Range(-26f, 26f) + " " + Random.Range(-14f, 14f) + " " + playerNames[i - 1] + " " + playerFirstAbility[i - 1] + " " + playerSecondAbility[i - 1] + " " + playerThirdAbility[i - 1]+"\n";
                }
                catch
                {

                }
            }
            currentScene = SceneManager.GetActiveScene().name;
            sentPlayers = true;
        }
        foreach(player player in FindObjectsOfType<player>())
        {
            if (player.online)
            {
                if (ableToSend)
                {
                    Vector2 movementDirection = (player.onlineNewPos - player.onlineOldPos) / PublicVariables.deltaTime;

                    mesaj += "PlayerPos" + " " + player.playerNumber + " " + player.transform.position.x + " " + player.transform.position.y + " " + player.transform.localEulerAngles.z + " " + player.health + " " + player.canDoAbils[0] + " " + player.canDoAbils[1] + " " + player.canDoAbils[2] + " " + player.canAttack + " " + player.networkCanAttack + " " + player.detectAttack + " " + player.networkDetect + " " + player.canAttackButWithMovement + " " + player.spamDirection + " " + player.health + " " + player.iceParticles.activeInHierarchy + " " + player.fireParticles.activeInHierarchy + " " + player.stunObj.activeInHierarchy + " " + movementDirection.x + " " + movementDirection.y + "\n";
                }
            }
        }
        List<NetIncomingMessage> messages = new List<NetIncomingMessage>();
        server.ReadMessages(messages);
        if (messages.Count != 0)
        {
            foreach (NetIncomingMessage message in messages)
            {
                string[] lines = message.ReadString().Split('\n');
                foreach (string line in lines)
                {
                    string data = line;
                    if (data.Contains("Conn"))
                    {
                        clientConnections.Add(message.SenderConnection);
                        var response = server.CreateMessage();
                        response.Write("ReceiveId " + players);
                        server.SendMessage(response, recipient: message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        var response2 = server.CreateMessage();
                        response2.Write("LoadScene " + SceneManager.GetActiveScene().name);
                        server.SendMessage(response2, recipient: message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        if (SceneManager.GetActiveScene().name.Contains("OnlineFight"))
                        {
                            foreach (player player in FindObjectsOfType<player>())
                            {
                                var response3 = server.CreateMessage();
                                response3.Write("SpawnPlayer " + player.playerNumber + " " + playerCharacterIds[player.playerNumber - 1] + " " + playerTeams[player.playerNumber - 1] + " " + player.transform.position.x + " " + player.transform.position.y + " " + playerNames[player.playerNumber - 1] + " " + playerFirstAbility[player.playerNumber - 1] + " " + playerSecondAbility[player.playerNumber - 1] + " " + playerThirdAbility[player.playerNumber - 1]);
                                server.SendMessage(response3, recipient: message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            }
                        }
                        playerCharacterIds.Add(1);
                        playerFirstAbility.Add(1);
                        playerSecondAbility.Add(1);
                        playerThirdAbility.Add(1);
                        playerNames.Add("");
                        playerTeams.Add(1);
                        players++;
                    }
                    if (message.MessageType == NetIncomingMessageType.Data)
                    {
                        if (data == "Disconnect")
                        {
                            DisconnectClient(message);
                        }
                        else
                        {
                            string type = data.Split(' ')[0];
                            if (type == "NewCharacterId")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {
                                    playerCharacterIds[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "PlayerTarget")
                            {
                                string[] parameters = data.Split(' ');
                                player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<player>();
                                Destroy(player.cursorTarget);
                                player.cursorTarget = null;
                                GameObject target = new GameObject();
                                target.transform.position = new Vector2(float.Parse(parameters[2]), float.Parse(parameters[3]));
                                player.cursorTarget = target;
                            }
                            else if (type == "NewAbility1")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {


                                    playerFirstAbility[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "NewAbility2")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {


                                    playerSecondAbility[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "NewAbility3")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {

                                    playerThirdAbility[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "NewCharacterName")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {

                                    playerNames[id] = data.Split(' ')[2];
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "NewCharacterTeam")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {

                                    playerTeams[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "SetVelocity")
                            {
                                string[] parameters = data.Split(' ');
                                float speed = 0;
                                player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<player>();
                                if ((player.canAttack || player.networkCanAttack) && player.ice.activeInHierarchy == false && player.stunned == false && player.hurt == false)
                                {
                                    if (player.speedObj != null)
                                    {

                                        if (player.slowed)
                                            speed = player.originalSpeed / 6;
                                        else if (player.poisonParticles.activeInHierarchy)
                                            speed = player.originalSpeed / 3;
                                        else if (player.speedObj.activeInHierarchy == true)
                                            speed = player.bonusSpeedValue;
                                        else speed = player.originalSpeed;
                                    }
                                    else
                                    {
                                        if (player.slowed)
                                            speed = player.originalSpeed / 6;
                                        else if (player.poisonParticles.activeInHierarchy)
                                            speed = player.originalSpeed / 3;
                                        else speed = player.originalSpeed;
                                    }
                                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[2]) * speed, float.Parse(parameters[3]) * speed);
                                    if (player.GetComponent<Rigidbody2D>().velocity != Vector2.zero && player.canAttackButWithMovement == false)
                                    {
                                        float rotZ = player.transform.rotation.z;
                                        player.transform.rotation = Quaternion.LookRotation(player.GetComponent<Rigidbody2D>().velocity * 100, new Vector3(0, 0, 1));
                                        player.transform.rotation = new Quaternion(0, 0, player.transform.rotation.z, player.transform.rotation.w);
                                        if (player.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
                                            player.transform.rotation = new Quaternion(0, 0, rotZ, player.transform.rotation.w);
                                    }
                                }


                            }
                            else if (type == "PlayerAnim")
                            {
                                string[] parameters = data.Split(' ');
                                player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<player>();
                                if (data.Contains("hurt"))
                                {
                                    mesaj += "PlayerAnim " + parameters[1] + " " + parameters[2] + " " + parameters[3] + " " + parameters[4] + "\n";

                                }
                                else if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(parameters[2]) == false || parameters[2] == "idle" || parameters[2] == "run")
                                {
                                    if (((parameters[2] == "idle" || parameters[2] == "run") && player.detectAttack) == false)
                                    {
                                        if (parameters.Length > 3)
                                        {
                                            if (player.playerNumber == 1)
                                            {
                                                if (player.ice.activeInHierarchy == false && player.stunned == false)
                                                {
                                                    mesaj += "PlayerAnim " + parameters[1] + " " + parameters[2] + " " + parameters[3] + " " + parameters[4] + "\n";
                                                }
                                            }
                                            else if (((player.canAttack || player.detectAttack) && player.ice.activeInHierarchy == false && player.stunned == false))
                                            {
                                                player.detectAttack = false;
                                                mesaj += "PlayerAnim " + parameters[1] + " " + parameters[2] + " " + parameters[3] + " " + parameters[4] + "\n";


                                            }

                                        }
                                        else
                                        {
                                            if (player.playerNumber == 1)
                                            {
                                                if (player.ice.activeInHierarchy == false && player.stunned == false)
                                                {
                                                    mesaj += "PlayerAnim " + parameters[1] + " " + parameters[2] + "\n";

                                                }
                                            }
                                            else if (((player.canAttack || player.detectAttack) && player.ice.activeInHierarchy == false && player.stunned == false))
                                            {
                                                player.detectAttack = false;
                                                mesaj += "PlayerAnim " + parameters[1] + " " + parameters[2] + "\n";

                                            }
                                        }
                                    }
                                }
                            }
                            else if (type == "PlayerRot")
                            {
                                string[] parameters = data.Split(' ');
                                player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<player>();
                                player.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[2]));
                                Destroy(player.cursorTarget);
                                player.cursorTarget = null;
                            }
                            else if (type == "UpdateAttack")
                            {
                                mesaj += data + "\n";
                            }
                            else if (type == "PlayerAbil")
                            {
                                mesaj += data + "\n";
                            }
                        }
                    }
                    if (message.MessageType == NetIncomingMessageType.ConnectionApproval)
                    {
                        message.SenderConnection.Approve();
                    }
                    try
                    {
                        if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            DisconnectClient(message);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        if (ableToSend)
            SendMessage();
    }

    private void DisconnectClient(NetIncomingMessage message)
    {
        bool breaking = false;
        for (int i = 0; i < clientConnections.Count; i++)
        {
            if (clientConnections[i] == message.SenderConnection)
            {
                breaking = true;
                for (int j = i; j < clientConnections.Count; j++)
                {
                    var response = server.CreateMessage();
                    response.Write("ReceiveId " + j);
                    try
                    {
                        playerCharacterIds[j] = playerCharacterIds[j + 1];
                        playerFirstAbility[j] = playerFirstAbility[j + 1];
                        playerSecondAbility[j] = playerSecondAbility[j + 1];
                        playerThirdAbility[j] = playerThirdAbility[j + 1];
                        playerNames[j] = playerNames[j + 1];
                        playerTeams[j] = playerTeams[j + 1];
                    }
                    catch
                    {
                        playerCharacterIds.RemoveAt(j);
                        playerFirstAbility.RemoveAt(j);
                        playerSecondAbility.RemoveAt(j);
                        playerThirdAbility.RemoveAt(j);
                        playerNames.RemoveAt(j);
                        playerTeams.RemoveAt(j);
                    }
                }
                if (breaking)
                    break;
            }
        }
        if (breaking)
        {
            players--;
            clientConnections.Remove(message.SenderConnection);
        }
    }
    public void SendAnim(string anim, int playerNum)
    {
        mesaj += "PlayerAnim " + playerNum + " " + anim+"\n";
    }
}
