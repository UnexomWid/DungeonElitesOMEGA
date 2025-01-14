using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Void : MonoBehaviour
{
    public bool spawnBoss = false;
    public GameObject music;
    public GameObject borders;

    public GameObject boss;

    public float minSize;
    public float maxSize;

    public float growSpeed;

    float t = 0;

    bool growing = true;

    public float maxHealth;

    public float health;

    public float healthDownTillSpawn;

    public float maxHealthDownTillSpawn;

    public GameObject[] enemyTypes;

    int timesSpawned = 0;

    public GameObject[] enemyPositions;

    public GameObject healthBar;

    public int initialEnemyCount;

    public int bonusEnemyCount;

    public GameObject destructionVfx;

    public List<GameObject> enemies;

    public AudioSource shock;

    public AudioSource damage;

    public AudioSource voidBreak;

    public AudioSource bossSpawn;

    public AudioSource destructionSfx;

    public void VoidBreak()
    {
        voidBreak.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        voidBreak.Play();
    }

    public void InvokeBoss()
    {
        music.SetActive(true);
        Invoke("SpawnBoss", 3f);
    }

    void SpawnBoss()
    {
        bossSpawn.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        bossSpawn.Play();
        boss.SetActive(true);
        borders.SetActive(true);
        boss.GetComponent<player>().ableToMove = true;
    }
    SpriteRenderer spriteRenderer;
    CameraFollow cameraFollow;
    InventorySpawn inventorySpawn;
    Animator animator;
    DungeonData dungeonData;
    bool isInTutorial;
    Transform myTransform;
    private void Start()
    {
        myTransform = transform;

        isInTutorial = SceneManager.GetActiveScene().name == "TutorialScene";

        animator = GetComponent<Animator>();

        inventorySpawn = FindObjectOfType<InventorySpawn>();

        dungeonData = FindObjectOfType<DungeonData>();

        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        xp = 50 + UnityEngine.Random.Range(-15, 15);

        enemies = new List<GameObject>();

        float num = ((float)FindObjectOfType<DungeonData>().playerCount)/5f;

        float num2 = (float)FindObjectOfType<DungeonData>().currentMap / 3f - 0.33f;

        health *= (0.8f+num + num2);

        maxHealthDownTillSpawn *= (0.8f+num+num2);

        maxHealth = health;
    }

    void SpawnEnemies()
    {
        int enemyCount = initialEnemyCount + bonusEnemyCount * timesSpawned;
        LockRoom parentScript = parentRoom.GetComponent<LockRoom>();
        for (int i = 0; i < enemyCount && i < enemyPositions.Length; i++)
        {
            GameObject enemy = Instantiate(enemyTypes[Random.Range(1, 99999) % (dungeonData.currentMap - 1 == 0 ? (enemyTypes.Length - 1) : enemyTypes.Length)]);
            enemy.transform.position = enemyPositions[i].transform.position;
            enemy.transform.localScale *= 150;
            enemy.gameObject.SetActive(true);
            player mobScript = enemy.GetComponent<player>();
            mobScript.parentRoom = parentScript;
            mobScript.Stats.SetActive(true);
            enemies.Add(enemy);
        }
    }
    public GameObject coinVfx;
    public GameObject parentRoom;
    public Sprite[] itemSprites;
    public GameObject drop;
    private float maxXp;
    private float xp;

    public void DecreaseHp(float damage, player player)
    {
        try
        {
            if (parentRoom.GetComponent<LockRoom>().locked && health > 0)
            {
                this.damage.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                this.damage.Play();
                health -= damage;
                if (health <= 0)
                {
                    health = 0;
                    healthBar.transform.localScale = new Vector2(0, 1);
                    AudioSource.PlayClipAtPoint(shock.clip, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                    if (Random.Range(1, 99999) % 5 <= 1 && spawnBoss == false)
                    {
                        int itemType = Random.Range(1, 99999) % (FindObjectOfType<ItemShop>().maxNum + 1);

                        GameObject clone = Instantiate(coinVfx);

                        clone.GetComponentInChildren<TextMeshPro>().text = "+";

                        clone.transform.position = new Vector2(myTransform.position.x + 4.52f, myTransform.position.y + 2f);

                        clone.transform.parent = Camera.main.transform;

                        clone.GetComponentInChildren<SpriteRenderer>().sprite = itemSprites[itemType];

                        clone.GetComponentInChildren<SpriteRenderer>().gameObject.transform.localScale *= 100;

                        if (itemType > 7)
                            itemType += 2;

                        inventorySpawn.AddItem(itemType);
                    }

                    if (spawnBoss)
                    {
                        animator.Play("explosion");
                    }
                    else
                    {

                        ServerNotification.instance.SendDungeonData("User defeated the Dungeon Void.");

                        foreach (GameObject enemy in enemies)
                        {
                            if (enemy != null)
                            {
                                enemy.GetComponent<player>().DecreaseHp(999999, new Vector2(0, 0), player, enemies.Count);
                            }
                        }

                        if (player.GetComponent<support>() != null)
                        {
                            player.GetComponent<support>().AddDamage(999999);
                        }
                        player.AddXp(50 + Random.Range(-15, 15));
                        int value = 50 + Random.Range(-10, 15);
                        inventorySpawn.AddCoins(value);
                        GameObject coin = Instantiate(coinVfx);
                        coin.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();

                        coin.transform.position = new Vector2(myTransform.position.x + 4.52f, myTransform.position.y);
                        coin.transform.parent = Camera.main.transform;
                        destroyObj = true;
                        currentSize = myTransform.localScale;

                        DungeonData.instance.AddToQuest(1, 1);

                        destructionSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        destructionSfx.Play();
                    }
                }
                else if (spawnBoss == false)
                {


                    healthDownTillSpawn -= damage;
                    if (healthDownTillSpawn <= 0)
                    {
                        SpawnEnemies();
                        shock.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        shock.Play();
                        cameraFollow.Shake(0.25f, 0.7f);
                        healthDownTillSpawn = maxHealthDownTillSpawn;
                        timesSpawned++;
                    }
                    
                }
                healthBar.transform.localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y);
                if (spawnBoss == false)
                {
                    float xpValue = damage / maxHealth * xp;

                    if (maxXp + xpValue > xp)
                    {
                        xpValue = xp - maxXp;
                        maxXp += xp;
                    }
                    else maxXp += damage / maxHealth * xp;
                    player.AddXp(xpValue);
                }
            }
        }
        catch
        {
            if (spawnBoss && health > 0)
            {
                this.damage.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                this.damage.Play();
                health -= damage;
                if (health <= 0)
                {
                    health = 0;
                    if (Random.Range(1, 99999) % 5 <= 1 && spawnBoss == false)
                    {
                        int itemType = Random.Range(1, 99999) % (FindObjectOfType<ItemShop>().maxNum + 1);

                        GameObject clone = Instantiate(coinVfx);

                        clone.GetComponentInChildren<TextMeshPro>().text = "+";

                        clone.transform.position = new Vector2(myTransform.position.x + 4.52f, myTransform.position.y + 2f);

                        clone.transform.parent = Camera.main.transform;

                        clone.GetComponentInChildren<SpriteRenderer>().sprite = itemSprites[itemType];

                        clone.GetComponentInChildren<SpriteRenderer>().gameObject.transform.localScale *= 100;

                        inventorySpawn.AddItem(itemType);
                    }

                    if (spawnBoss)
                    {
                        animator.Play("explosion");
                    }
                    else
                    {
                        ServerNotification.instance.SendDungeonData("User defeated the Dungeon Void.");

                        foreach (GameObject enemy in enemies)
                        {
                            if (enemy != null)
                            {
                                enemy.GetComponent<player>().DecreaseHp(999999, new Vector2(0, 0), player, enemies.Count);
                            }
                        }
                        player.AddXp(50 + Random.Range(-15, 15));
                        int value = 50 + Random.Range(-10, 15);
                        inventorySpawn.AddCoins(value);
                        GameObject coin = Instantiate(coinVfx);
                        coin.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();

                        coin.transform.position = new Vector2(myTransform.position.x + 4.52f, myTransform.position.y);
                        coin.transform.parent = Camera.main.transform;

                        destroyObj = true;
                        currentSize = myTransform.localScale;

                        destructionSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        destructionSfx.Play();
                    }
                }
                else if (spawnBoss == false)
                {


                    healthDownTillSpawn -= damage;
                    if (healthDownTillSpawn <= 0)
                    {
                        SpawnEnemies();
                        shock.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        shock.Play();
                        cameraFollow.Shake(0.25f, 0.7f);
                        healthDownTillSpawn = maxHealthDownTillSpawn;
                        timesSpawned++;
                    }
                    
                }
                healthBar.transform.localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y);
                if (spawnBoss == false)
                {
                    float xpValue = damage / maxHealth * xp;

                    if (maxXp + xpValue > xp)
                    {
                        xpValue = xp - maxXp;
                        maxXp += xp;
                    }
                    else maxXp += damage / maxHealth * xp;
                    player.AddXp(xpValue);
                }
            }
        }
    }



    bool destroyObj = false;
    Vector2 currentSize;
    float tDestroy = 0;
    // Update is called once per frame
    void Update()
    {
        if (isInTutorial == false)
        {
            myTransform.eulerAngles = new Vector3(0, 0, 0);
            if (destroyObj == false)
            {
                if (growing)
                {
                    t += PublicVariables.deltaTime * growSpeed;
                    if (t > 1)
                        growing = false;
                    else
                    {
                        float currentSize = Mathf.Lerp(minSize, maxSize, t);
                        myTransform.localScale = new Vector3(currentSize, currentSize);

                        spriteRenderer.color = Color32.Lerp(new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255), t);
                    }
                }
                else
                {
                    t -= PublicVariables.deltaTime * growSpeed;
                    if (t < 0)
                        growing = true;
                    else
                    {
                        float currentSize = Mathf.Lerp(minSize, maxSize, t);
                        myTransform.localScale = new Vector3(currentSize, currentSize);

                        spriteRenderer.color = Color32.Lerp(new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255), t);
                    }
                }
            }
            if (destroyObj)
            {
                tDestroy += PublicVariables.deltaTime * 6;
                if (tDestroy > 1)
                {
                    tDestroy = 1;
                    GameObject vfx = Instantiate(destructionVfx);
                    vfx.transform.position = myTransform.position;
                    vfx.transform.localScale *= 1.5f;
                    Destroy(vfx, 1f);
                    Destroy(gameObject);
                }
                myTransform.localScale = Vector2.Lerp(currentSize, new Vector2(0, 0), tDestroy);
            }
        }
    }
}
