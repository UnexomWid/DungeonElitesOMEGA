using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRoom : MonoBehaviour
{
    public bool keyRoom = false;
    public GameObject chestRoomDoor;
    public GameObject chestRoom;

    public bool parkourRoom = false;
    bool destroyedMobs = false;
    public GameObject[] parkourDoors;
    public GameObject chestObj;





    public GameObject[] hidingSpots;
    public AudioSource openDoor;
    public AudioSource closeDoor;

    public GameObject GetHideSpot(GameObject target, GameObject except = null)
    {
        float maxDist = 0f;
        GameObject hideSpot = null;

        List<GameObject> hidingList = new List<GameObject>(hidingSpots);
        if (except != null)
            hidingList.Remove(except);

        return hidingList[Random.Range(1,999999)%hidingList.Count];
    }

    public GameObject GetHideSpotClosest(Vector2 mobPos, Vector2 playerPos)
    {
        GameObject currHideSpot = null;

        float maxAngle = 0f;
        for(int i=0;i<hidingSpots.Length;i++)
        {
            float angle = Angle360(playerPos, mobPos, hidingSpots[i].transform.position);

            if (angle < 45 && angle > maxAngle)
            {
                currHideSpot = hidingSpots[i];
                maxAngle = angle;
            }
        }

        return currHideSpot;
    }

    public static float Angle360(Vector2 playerPos, Vector2 mobPos, Vector2 hidePos)
    {
        return Vector2.Angle(playerPos - mobPos, hidePos - mobPos);
    }

    public GameObject boxes;
    public AudioSource shock;
    public bool spawnChest = false;
    public GameObject chest;

    public float distanceFromCenter;
    public bool locked = false;
    public GameObject theVoid;
    public GameObject[] door1;
    public GameObject door2;
    public GameObject[] door1pos;
    public GameObject door2pos;
    public Vector2[] door1ogpos;
    public Vector2 door2ogpos;
    public List<GameObject> players;
    public GameObject tankPos;
    public GameObject wizPos;
    public GameObject archPos;
    public GameObject suppPos;
    public GameObject knightPos;
    public GameObject[] mobs;

    public GameObject[] bosses;

    public GameObject boss;
    public List<GameObject> bossMobs;
    public bool bossRoom = false;
    public int mobCountToSpawn;
    public GameObject[] mobPositions;
    public int mobIncrement;

    public bool gotDamaged = false;
    public bool usedBasic = false;
    public bool usedAbil = false;

    player[] detectedPlayers;
    List<player> playerScripts;
    List<Collider2D> playerColliders;
    List<player> bossMobsScripts;

    private void Start()
    {
        playerScripts = new List<player>();
        playerColliders = new List<Collider2D>();
        bossMobsScripts = new List<player>();
        mobCountToSpawn *= 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.GetComponent<player>();
        GameObject collisionObj = collision.gameObject;
        if (player != null && players.Contains(collisionObj) == false)
        {
            if (player.bot == false)
            {
                players.Add(collisionObj);
                playerScripts.Add(player);
                playerColliders.Add(collision);
            }
        }
    }

    SpriteRenderer spriteRenderer;
    BoxCollider2D door2BC;
    LockRoom lockRoom;
    Collider2D collider;
    DungeonData dungeonData;
    List<player> mobScripts;

    bool enabledUpdate = false;

    private void OnEnable()
    {
        if (parkourRoom && chestObj != null)
            chestObj.SetActive(true);
        closeDoor.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        closeDoor.Play();
        collider = GetComponent<Collider2D>();
        bossMobsScripts = new List<player>();
        mobScripts = new List<player>();
        try
        {
            door2BC = door2.GetComponent<BoxCollider2D>();
        }
        catch
        {

        }
        try
        {
            dungeonData = DungeonData.instance;
        }
        catch
        {

        }
        try
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        catch
        {

        }
        try
        {
            detectedPlayers = FindObjectsOfType<player>();
        }
        catch
        {

        }
        lockRoom = GetComponent<LockRoom>();

        door2.SetActive(true);

        for(int i=0;i<door1.Length;i++)
            door1[i].SetActive(true);

        boxes.SetActive(true);

        Fog.instance.BeginFog();

        for(int i=0;i<door1.Length;i++)
        {
            door1ogpos[i] = door1[i].transform.position;
        }

        float maxPlayerSpeed = 0f;
        for(int i=0;i<players.Count;i++)
        {
            player playerScript = players[i].GetComponent<player>();
            switch(playerScript.caracterId)
            {
                case 1:
                    players[i].transform.position = wizPos.transform.position;
                    break;
                case 2:
                    players[i].transform.position = knightPos.transform.position;
                    break;
                case 3:
                    players[i].transform.position = archPos.transform.position;
                    break;
                case 4:
                    players[i].transform.position = tankPos.transform.position;
                    break;
                case 5:
                    players[i].transform.position = suppPos.transform.position;
                    break;
            }
            if (playerScript.originalSpeed > maxPlayerSpeed)
                maxPlayerSpeed = playerScript.originalSpeed;
            playerScript.LockPlayer(collider);
            playerScript.lockRoom = this;
            if (parkourRoom)
            {
                playerScript.SmallHitbox();
                playerScript.ParkourSilence();
            }
        }
        if(theVoid != null)
        {
            mobs = new GameObject[] { theVoid };
        }
        else if (bossRoom == false)
        {
            GameObject[] spawnedMobs = new GameObject[mobPositions.Length];
            for(int i=0;i<mobPositions.Length;i++)
            {
                GameObject mob = Instantiate(mobs[Random.Range(1, 99999) % (dungeonData.currentMap-1 == 0 ? (mobs.Length-1) : mobs.Length)]);
                mob.transform.position = mobPositions[i].transform.position;

                mob.transform.localScale *= 150;

                mob.SetActive(true);
                player mobScript = mob.GetComponent<player>();

                if (parkourRoom)
                {
                    mobScript.parkourRoom = true;
                    mobScript.parkourSpeed = maxPlayerSpeed*4 + 5;
                }

                mobScript.currentBox = collider;
                mobScript.parentRoom = this;

                mobScripts.Add(mobScript);

                spawnedMobs[i] = mob;
            }
            mobs = spawnedMobs;
        }
        else
        {
            boss = bosses[dungeonData.bosses[dungeonData.currentMap-1]-1];
            player bossScript = boss.GetComponent<player>();
            bossScript.boss = true;
            bossScript.bossRoom = gameObject;
            bossScript.parentRoom = this;
            bossScript.currentBox = collider;
            boss.SetActive(true);
        }
        AstarPath astarPath = AstarPath.active;
        astarPath.data.gridGraph.center = transform.position;

        door2ogpos = door2.transform.position;
        if (locked == true)
            lockRoom.enabledUpdate = false;
        else locked = true;

        enabledUpdate = true;
    }

    public void BossSpawnMobs()
    {
        mobCountToSpawn += mobIncrement * 2;

        bossMobs.Clear();
        bossMobsScripts.Clear();

        for (int i = 0; i < mobCountToSpawn; i++)
        {
            GameObject mob = Instantiate(mobs[Random.Range(1, 99999) % (dungeonData.currentMap - 1 == 0 ? (mobs.Length - 1) : mobs.Length)]);
            mob.transform.position = mobPositions[Random.Range(1, 99999) % mobs.Length].transform.position;

            mob.transform.localScale *= 150;

            player mobScript = mob.GetComponent<player>();

            mobScript.speed *= 2;
            mobScript.parentRoom = this;
            mobScript.currentBox = collider;

            mob.SetActive(true);

            bossMobs.Add(mob);
            bossMobsScripts.Add(mobScript);
        }

        boss.GetComponent<player>().ResetAttacks();
        boss.SetActive(false);

        shock.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        shock.Play();
        CameraFollow.instance.Shake(0.25f, 0.7f);
    }

    public void BossKillMobs(player player)
    {
        for(int i=0;i<bossMobsScripts.Count;i++)
        {
            if(bossMobsScripts[i] != null)
                bossMobsScripts[i].DecreaseHp(99999, Vector2.zero, player);
        }
    }

    public float t = 0;
    float t2 = 0;
    public float doorCloseTime;
    public bool allPlayers = false;
    bool roomClosed = false;

    float timeToDefeatDungeon = 0;

    // Update is called once per frame
    void Update()
    {
        if (enabledUpdate)
        {
            if(bossRoom && boss != null)
            {
                if (boss.activeInHierarchy == false)
                {
                    bool ok = false;

                    for(int i=0;i<bossMobs.Count;i++)
                    {
                        if (bossMobs[i] != null)
                        {
                            ok = true;
                            break;
                        }
                    }

                    if (ok == false)
                    {
                        boss.SetActive(true);
                    }
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].activeInHierarchy == false)
                    {
                        players[i] = null;
                        playerScripts[i] = null;
                    }
                }
            }
            bool okPlayers = true;
            while (okPlayers)
            {
                okPlayers = false;
                player currentPlayer = null;
                for(int i=0;i<playerScripts.Count;i++)
                {
                    if (playerScripts[i] != null)
                    {
                        if (spriteRenderer.bounds.Intersects(playerScripts[i].firstLimb.bounds) == false)
                        {
                            currentPlayer = playerScripts[i];
                            okPlayers = true;
                            break;
                        }
                    }
                }
                if (currentPlayer != null)
                {
                    players.Remove(currentPlayer.gameObject);
                    playerScripts.Remove(currentPlayer);
                    playerColliders.Remove(currentPlayer.GetComponent<Collider2D>());
                }
            }
            bool allPlayers = true;
            for (int i = 0; i < playerScripts.Count; i++)
            {
                if (playerScripts[i] != null)
                {
                    if (playerScripts[i].bot == false)
                    {
                        Collider2D playerCollider = playerColliders[i];
                        //Collider2D[] colliders = Physics2D.OverlapBoxAll(playerScripts[i].transform.position, new Vector2(0.01f, 0.01f), 0f);
                        Collider2D[] doorColliders = Physics2D.OverlapBoxAll(door2pos.transform.position, door2BC.size * 50, door2.transform.eulerAngles.z);
                        bool ok2 = true;
                        Quaternion rot = new Quaternion(0, 0, 0, 0);
                        rot.eulerAngles = door2.transform.eulerAngles;

                        for(int j=0;j<doorColliders.Length;j++)
                        {
                            if (doorColliders[j] == playerCollider)
                                ok2 = false;
                        }
                        bool ok = false;
                        if (spriteRenderer.bounds.Intersects(playerScripts[i].firstLimb.bounds))
                            ok = true;

                        if (ok == false || ok2 == false)
                            allPlayers = false;

                    }
                }
            }
            if (allPlayers == true)
                this.allPlayers = true;
            allPlayers = this.allPlayers;
            bool okMob = false;
            if (bossRoom == false)
            {
                for(int i=0;i<mobs.Length;i++)
                {
                    if (mobs[i] != null)
                        okMob = true;
                }
            }
            else if (boss != null)
                okMob = true;
            else okMob = false;

            if (parkourRoom && mobs.Length == 0)
            {
                if (chestObj == null)
                    okMob = false;
                else okMob = true;
            }

            if (allPlayers && okMob && (parkourRoom == false || t<1))
            {
                t += PublicVariables.deltaTime * doorCloseTime;
                if (t > 1)
                {
                    t = 1;
                    if (scanned == false)
                    {
                        AstarPath.active.Scan();
                        scanned = true;
                    }
                }
                for (int i = 0; i < door1.Length; i++)
                {
                    door1[i].transform.position = Vector2.Lerp(door1ogpos[i], door1pos[i].transform.position, t);
                }
            }
            else if (((locked == false || okMob == false) && parkourRoom == false) || (parkourRoom && chestObj == null))
            {

                if (destroyedMobs == false && parkourRoom)
                {
                    for(int i=0;i<mobScripts.Count;i++)
                    {
                        if(mobScripts[i] != null)
                        {
                            mobScripts[i].DecreaseHp(99999, Vector2.zero, null, mobScripts.Count, false);
                        }
                    }

                    foreach(GameObject door in parkourDoors)
                    {
                        door.GetComponent<Animator>().enabled = true;
                        door.GetComponent<Animator>().Play("fade");
                    }

                    if(keyRoom)
                    {
                        chestRoom.GetComponent<PolygonCollider2D>().enabled = true;
                        chestRoomDoor.GetComponent<Animator>().enabled = true;
                        chestRoomDoor.GetComponent<Animator>().Play("fade");
                    }

                    destroyedMobs = true;
                }

                t -= PublicVariables.deltaTime * doorCloseTime;
                if (t < 0)
                    t = 0;
                for (int i = 0; i < door1.Length; i++)
                {
                    door1[i].transform.position = Vector2.Lerp(door1ogpos[i], door1pos[i].transform.position, t);
                }
                door2.transform.position = Vector2.Lerp(door2ogpos, door2pos.transform.position, t);
                if (t == 0)
                {
                    for(int i=0;i<lockRoom.door1.Length;i++)
                        lockRoom.door1[i].SetActive(false);
                    boxes.SetActive(false);
                    lockRoom.door2.SetActive(false);
                    foreach (player player in FindObjectsOfType<player>())
                    {
                        player.UnlockPlayer();
                        if (parkourRoom)
                        {
                            player.NormalHitbox();
                            player.StopSilence();
                            player.ResetOGSpeed();

                            if (spawnChest)
                                ServerNotification.instance.SendDungeonData("User got to the Parkour Chest.");
                            else ServerNotification.instance.SendDungeonData("User got to the Parkour Key.");
                        }
                    }
                    if (spawnChest)
                        chest.SetActive(true);

                    if(bossRoom)
                        ServerNotification.instance.SendDungeonData("User defeated the Dungeon Boss.");

                    if(chest != null && parkourRoom == false)
                    {
                        ServerNotification.instance.SendDungeonData("User defeated the Reward Room.");
                    }

                    openDoor.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                    openDoor.Play();
                    Fog.instance.StopFog();
                    lockRoom.enabledUpdate = false;
                    roomClosed = true;

                    if(gotDamaged == false)
                    {
                        DungeonData.instance.AddToQuest(22, 1);
                    }
                    if (usedBasic == false)
                    {
                        DungeonData.instance.AddToQuest(26, 1);
                    }
                    if (usedAbil == false)
                    {
                        DungeonData.instance.AddToQuest(27, 1);
                    }

                    if (timeToDefeatDungeon <= 8)
                        DungeonData.instance.AddToQuest(23, 1);

                    this.enabled = false;
                }
            }
            if (allPlayers && okMob)
            {
                timeToDefeatDungeon += PublicVariables.deltaTime;

                t2 += PublicVariables.deltaTime * doorCloseTime;
                if (t2 > 1)
                    t2 = 1;
                door2.transform.position = Vector2.Lerp(door2ogpos, door2pos.transform.position, t2);
                if (t2 == 1)
                {

                    locked = true;

                }
            }
        }
    }
    bool scanned = false;
}
