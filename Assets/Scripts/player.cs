using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Pathfinding;
using System.Diagnostics;
using System.Linq;

public class player : MonoBehaviour
{
    public bool interactsAbillity = false;
    bool movementToggled = true;
    private bool togglePressed;
    bool movementToggle = false;

    public bool animState_IsRunning; //IsName("run");

    public float bonusStrPoints;

    bool HumanPlayerAbleToMove()
    {
        return ableToMove && !OMEGA.DebugMaster.freecam;
    }

    public void ResetPermPoints()
    {    
        float permHp = PlayerPrefs.GetFloat("Character" + caracterId + "hp", 0)/10;
        float permReg = PlayerPrefs.GetFloat("Character" + caracterId + "reg", 0)/10;
        float permSpd = PlayerPrefs.GetFloat("Character" + caracterId + "spd", 0)/10;
        float permDex = PlayerPrefs.GetFloat("Character" + caracterId + "dex", 0)/10;
        float permStr = PlayerPrefs.GetFloat("Character" + caracterId + "str", 0)/10;
        float permCdr = PlayerPrefs.GetFloat("Character" + caracterId + "cdr", 0)/10;

        OMEGA.Events.OnResetPermPoints(ref permHp, ref permReg, ref permSpd, ref permDex, ref permStr, ref permCdr);

        health += baseHealth * 0.1f * permHp;
        maxHealth += baseHealth * 0.1f * permHp;
        hpBarTransform.localScale = new Vector3(health / maxHealth, hpBarTransform.localScale.y, 1);

        extraHealthRegen += newReg * permReg;

        originalSpeed += 0.75f * permSpd;

        attackSpeed += newDex * permDex;

        bonusStrPoints = permStr;

        if (caracterId == 1)
        {
            wizard.staffDamage += baseDamage * 0.3f * permStr;
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.weaponDamage += baseDamage * 0.3f * permStr;
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.arrowDamage += baseDamage * 0.3f * permStr;
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.weaponDamage += baseDamage * 0.3f * permStr;
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.weaponDamage += baseDamage * 0.33f * permStr;
            supportClass.SetUpText();
        }

        cooldownReduction += newCdr * permCdr;

        if (caracterId == 4)
        {
            tankCharge -= 300 * permCdr;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack6")
                    abilMax[i] -= 300 * permCdr;
            }

            tank.AddDamage(0);
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            suppCharge -= 400 * permCdr;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack3")
                    abilMax[i] -= 400 * permCdr;
            }

            supportClass.AddDamage(0);
            supportClass.SetUpText();
        }

        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.SetUpText();
        }
    }

    public void UpdateState() //Metoda apelata doar de abillityShop.cs pentru ca este primul script care executa Update();
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animState_IsRunning = stateInfo.IsName("run");
    }

    public string SceneName;

    public GameObject helperLine;
    public PhoneAbillities basic;
    public PhoneAbillities abillity1;
    public PhoneAbillities abillity2;
    public PhoneAbillities abillity3;
    public PhoneInteraction phoneInteraction;
    public PhoneInteraction phoneReturn;
    public PhoneInteraction phoneToggle;
    public GameObject lockObj;

    public void ActivateHitbox()
    {
        if (bot == false && isBoss == false && finalBoss == false)
            myObj.layer = 8;
        else myObj.layer = 16;
    }

    itemMenu currentItemMenu = null;
    public void HighlightInteraction(itemMenu menu = null)
    {
        if (menu != null)
        {
            currentItemMenu = menu;
        }

        if (phoneInteraction == null)
        {
            return;
        }

        phoneInteraction.Highlight();
    }

    public void UnhighlightInteraction(itemMenu menu = null)
    {
        if (phoneInteraction == null)
        {
            return;
        }

        if (menu != null)
        {
            if (menu == currentItemMenu)
            {
                phoneInteraction.Unhighlight();
            }
        }
        else phoneInteraction.Unhighlight();
    }

    public void ActivateReturn()
    {
        if (phoneInteraction == null)
        {
            return;
        }

        phoneReturn.gameObject.SetActive(true);
    }

    public void StopReturn()
    {
        if (phoneReturn == null)
        {
            return;
        }

        phoneReturn.gameObject.SetActive(false);
    }

    public bool isInteracting()
    {
        return phoneInteraction != null && phoneInteraction.isInteracting;
    }

    public bool isReturning()
    {
        return phoneReturn && phoneReturn.isInteracting;
    }


    public PhoneController phoneController;

    public GameObject healthBarObj;

    public float CurrentSpeed
    {
        get
        {
            return speed * (timeTillAttacked * 2 + 1);
        }
    }

    public bool bossScene = false;

    RandomTarget randomAroundTarget;

    public bool noAI = false;

    public TextMeshPro healthText;

    public ShopSpawner shopSpawn;

    public bool botInShops = false;

    public AudioClip footstep;

    public Material spriteUnlit;

    public Vector2 myVelocity;

    public float baseHealth;
    public float baseSpeed;
    public float baseDamage;

    float ogRadius;
    public void SmallHitbox()
    {
        ogRadius = circleCollider.radius;
        circleCollider.radius = 0.04f;
    }

    public void NormalHitbox()
    {
        circleCollider.radius = ogRadius;
    }

    void MakeChildrenUnlit(Transform playerTransform)
    {
        foreach (Transform child in playerTransform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.material = spriteUnlit;
            MakeChildrenUnlit(child);
        }
    }
    public IEnumerator Knockback(Vector2 direction, float time)
    {
        int index = 0;
        for (int i = 0; i < limbSprites.Count; i++)
        {
            if (spriteForDamage)
                limbSprites[i].sprite = sprites[index++];
            else limbSprites[i].color = new Color32(255, 0, 0, 255);
        }

        float knockDur = 0f;

        Quaternion rotation = Quaternion.LookRotation(-direction * 100, new Vector3(0, 0, 1)); ;

        rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        playerTransform.rotation = rotation;

        rb.velocity = myVelocity = direction;

        while (knockDur < time)
        {
            if (PublicVariables.TimeScale > 0.05f)
            {
                knockDur += PublicVariables.deltaTime;
            }

            yield return null;
        }

        StopDamage(true);
    }

    float[] attackDistances;
    int bossCurrentAttackIndex = -1;

    public int bossCurrentAttack = 0;
    public Animator Animator
    {
        get
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            return animator;
        }
    }

    public LockRoom parentRoom;

    public AudioSource weaken;
    public AudioSource silence;
    public GameObject weakenObj;
    public GameObject silenceObj;
    public GameObject mobAura;
    public AudioSource blind;
    public enum MobType
    {
        Fire,
        Ice,
        Plains,
        Magic,
        Stone,
        None
    }
    MobType mobType;
    public MobType ApplyEffect()
    {
        return mobType;
    }
    public GameObject slowParticles;
    public GameObject purchaseVfx;
    public AudioSource purchaseSfx;
    public GameObject damageEffect;
    public ControllerInput gamePad;
    public float tankCharge = 5000;
    public float suppCharge = 8000;

    public AudioSource breaking;
    public AudioSource rise;

    public AudioSource impactRise;
    public AudioSource impact;

    public void SpawnPurchaseVfx()
    {
        GameObject vfx = Instantiate(purchaseVfx);
        vfx.transform.position = playerTransform.position;
        vfx.transform.parent = playerTransform;
        vfx.transform.localScale = new Vector2(1, 1);
        Destroy(vfx, 2f);
        purchaseSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        purchaseSfx.Play();
    }

    public void ShakeCamera(float amp)
    {
        if (cameraFollow != null)
            cameraFollow.Shake(0.25f, amp);

    }

    bool GetKeyboardButton(int button)
    {
        if (playerNumber == 5 && silenceObjActive == false && HumanPlayerAbleToMove())
        {
            if (button == buttons[4] && Input.GetKey(KeyCode.Q))
                return true;
            else if (button == buttons[5] && Input.GetKey(KeyCode.W))
                return true;
            else if (button == buttons[6] && Input.GetKey(KeyCode.E))
                return true;
            else return false;
        }
        else return false;
    }

    bool GetPhoneButton(int button)
    {
        if (playerNumber == 5 && silenceObjActive == false && HumanPlayerAbleToMove())
        {
            if (button == buttons[4] && abillity1.attack)
                return true;
            else if (button == buttons[5] && abillity2.attack)
                return true;
            else if (button == buttons[6] && abillity3.attack)
                return true;
            else if (button == buttons[7] && basic.attack)
                return true;
            else return false;
        }
        else return false;
    }

    bool GetGamePadButton(int button)
    {
        if (gamePad != null && playerNumber != 5 && silenceObjActive == false && HumanPlayerAbleToMove())
        {
            if (button == buttons[4] && gamePad.A == 1)
                return true;
            else if (button == buttons[5] && gamePad.B == 1)
                return true;
            else if (button == buttons[6] && gamePad.Y == 1)
                return true;
            else if (button == buttons[7] && gamePad.X == 1)
                return true;
            else return false;
        }
        else if (playerNumber == 5 && HumanPlayerAbleToMove() && keyboard == false)
        {
            return GetPhoneButton(button);
        }
        else if (playerNumber == 5 && HumanPlayerAbleToMove())
        {
            return GetKeyboardButton(button);
        }
        else return false;
    }

    public bool Weakened
    {
        get
        {
            return weakenObj.activeInHierarchy;
        }
    }

    public void PlayBreak()
    {
        breaking.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        breaking.Play();
    }

    public void Rise()
    {
        rise.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        rise.Play();
    }

    public void PlayImpactRise()
    {
        impactRise.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        impactRise.Play();
    }

    public void PlayImpact()
    {
        impact.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        impact.Play();
    }

    public bool spriteForDamage;

    public Sprite[] sprites;
    public Sprite[] oldSprites;

    void FinalBossCredits()
    {
        if (isInShops == false)
            SceneManager.LoadScene("Credits");
    }

    public bool followTarget = false;
    public void TargetReached()
    {
        followTarget = false;
    }

    public float xpDrop;

    bool damaged = false;

    public AudioSource fireballSFX;
    public AudioSource teleport;
    public void PlayFireball2()
    {
        fireballSFX.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        fireballSFX.Play();
    }
    public void PlayTeleporty2()
    {
        teleport.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        teleport.Play();
    }
    public GameObject explosion;

    void Expl()
    {
        CancelInvoke("Expl");

        GameObject expl = Instantiate(explosion);
        expl.GetComponent<explosion>().active = true;

        GameObject target = null;

        List<GameObject> targets = new List<GameObject>();

        for (int i = 0; i < cameraFollow.playerScripts.Count; i++)
        {
            if (cameraFollow.playerScripts[i] != null)
            {
                GameObject obj = cameraFollow.playerScripts[i].gameObject;
                if (obj != myObj)
                {
                    targets.Add(obj);
                }
            }
        }

        target = targets[UnityEngine.Random.Range(1, 99999) % targets.Count];

        expl.transform.position = target.transform.position;

        if (health <= maxHealth / 8)
            Invoke("Expl", 1f);
        else if (health <= maxHealth / 2)
            Invoke("Expl", 2f);
    }


    public GameObject bar;
    public List<GameObject> mobs;
    public GameObject bossCopy;

    void KillMobs()
    {
        CancelInvoke("KillMobs");
        for (int i = 0; i < mobs.Count; i++)
        {
            Destroy(mobs[i]);
        }
        bar.SetActive(true);
        currentAttack = 0;
    }

    public bool finalBoss = false;
    public int currentAttack = 0;
    public int maxFireballCount;
    public int currentFireballCount;
    public int consecutiveAttacks = 0;

    public GameObject bossFireball;
    public GameObject fbPos;
    public float fireballSpeed;

    public int timesAttacked;
    public int maxTimesAttacked;
    public Vector2 targetLocation;

    bool tilted = false;


    public void FireballCircle()
    {
        tilted = !tilted;

        float degree = 0;

        if (tilted)
            degree += 11.25f;

        for (int i = 0; i <= 15; i++)
        {
            GameObject fb = Instantiate(bossFireball);
            Transform fbTransform = fb.transform;
            fbTransform.position = playerTransform.position;
            fbTransform.eulerAngles = new Vector3(0, 0, playerTransform.eulerAngles.z + degree);
            fb.GetComponent<Rigidbody2D>().velocity = fireballSpeed * (-fbTransform.up) / 3f;
            hitbox fbHitbox = fb.GetComponent<hitbox>();
            fbHitbox.parent = myObj;
            fbHitbox.damage = 100;
            fbHitbox.knockBackSpeed = 3;
            fbTransform.localScale *= 2;
            degree += 22.5f;
        }
        PlayFireball2();
    }

    public void BossFireball()
    {
        GameObject fb = Instantiate(bossFireball);
        Transform fbTransform = fb.transform;
        fbTransform.position = fbPos.transform.position;
        fb.GetComponent<Rigidbody2D>().velocity = fireballSpeed * (-playerTransform.up);
        fbTransform.eulerAngles = playerTransform.eulerAngles;
        hitbox fbHitbox = fb.GetComponent<hitbox>();
        fbHitbox.parent = myObj;
        fbHitbox.damage = 150;
        fbHitbox.knockBackSpeed = 6;
        fbTransform.localScale *= 3;
    }

    public GameObject maxTpLoc;
    public GameObject minTpLoc;

    public Vector2 teleportLocation;

    public void BossTeleport()
    {
        playerTransform.position = teleportLocation;
    }
    bool fireball = false;
    public void BossTeleportEnd()
    {

        GameObject target = null;

        List<GameObject> targets = new List<GameObject>();

        for (int i = 0; i < cameraFollow.playerScripts.Count; i++)
        {
            if (cameraFollow.playerScripts[i] != null)
            {

                GameObject obj = cameraFollow.playerScripts[i].gameObject;
                if (obj != myObj)
                {
                    targets.Add(obj);
                }
            }
        }

        target = targets[UnityEngine.Random.Range(1, 99999) % targets.Count];

        Vector2 dif = (Vector2)target.transform.position - (Vector2)playerTransform.position;
        dif.Normalize();
        float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        playerTransform.localEulerAngles = new Vector3(0, 0, rot + 90);

        animator.Play("boss1");

        currentFireballCount++;
        if (currentFireballCount >= maxFireballCount)
        {
            currentAttack = 0;
        }

    }







    public bool isBoss = false;

    public GameObject levelVfx;
    public AudioSource levelup;
    public float distFromCenter;

    public float[] cooldownReduce;
    public float[] cooldownReductions;

    public float xp;
    public float lvl = 1;
    public float abilityPoints;
    public float masterPoints;
    public GameObject bomb;

    public float[] abilityCooldowns;

    public float[] currentCooldowns;

    public float newHp;
    public float newReg;
    public float newStr;
    public float newDex;
    public float newCdr;
    public float newSpd;

    public float extraHealthRegen = 1;

    public float cooldownReduction = 0f;

    public void StatBoost()
    {
        health += baseHealth * 0.1f;
        maxHealth += baseHealth * 0.1f;
        hpBarTransform.localScale = new Vector3(health / maxHealth, hpBarTransform.localScale.y, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        extraHealthRegen += 0.1f;
        originalSpeed += 0.75f;
        attackSpeed += 0.05f;
        cooldownReduction += 2.5f;

        if (caracterId == 1)
        {
            wizard.staffDamage += baseDamage * 0.3f;
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.weaponDamage += baseDamage * 0.3f;
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.arrowDamage += baseDamage * 0.3f;
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tankCharge -= 150;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack6")
                    abilMax[i] -= 150;
            }

            tank.AddDamage(0);
            tank.weaponDamage += baseDamage * 0.3f;
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            suppCharge -= 200;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack3")
                    abilMax[i] -= 200;
            }

            supportClass.AddDamage(0);
            supportClass.weaponDamage += baseDamage * 0.3f;
            supportClass.SetUpText();
        }
    }
    public void RemoveStatBoost()
    {
        health -= baseHealth * 0.1f;
        maxHealth -= baseHealth * 0.1f;
        hpBarTransform.localScale = new Vector3(health / maxHealth, hpBarTransform.localScale.y, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        extraHealthRegen -= 0.1f;
        originalSpeed -= 0.75f;
        attackSpeed -= 0.05f;
        cooldownReduction -= 2.5f;
        if (caracterId == 1)
        {
            wizard.staffDamage -= baseDamage * 0.3f;
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.weaponDamage -= baseDamage * 0.3f;
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.arrowDamage -= baseDamage * 0.3f;
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tankCharge += 150;
            for(int i=0;i<=2;i++)
            {
                if (attacks[i] == "attack6")
                    abilMax[i] += 150;
            }
            tank.AddDamage(0);
            tank.weaponDamage -= baseDamage * 0.3f;
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            suppCharge += 200;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack3")
                    abilMax[i] += 200;
            }

            supportClass.AddDamage(0);
            supportClass.weaponDamage -= baseDamage * 0.3f;
            supportClass.SetUpText();
        }
        foreach (player player in cameraFollow.playerScripts)
        {
            if (player != null)
            {
                if (player.bot == false)
                {
                    bool ok = false;
                    CharacterSlot playerSlot = null;
                    foreach (CharacterSlot slot in inventory.slots)
                    {
                        if (slot.characterId == player.caracterId)
                        {
                            playerSlot = slot;
                            ok = true;
                            break;
                        }
                    }
                    if (ok)
                    {
                        playerSlot.healthBar.transform.localScale = new Vector3(player.health / player.maxHealth, playerSlot.healthBar.transform.localScale.y, 1);
                        playerSlot.characterHp.text = player.health + "/" + player.maxHealth;
                    }
                    else
                    {
                        playerSlot.healthBar.transform.localScale = new Vector3(0, playerSlot.healthBar.transform.localScale.y, 1);
                        playerSlot.characterHp.text = playerSlot.characterHp.text.Split('/')[1] + "/" + player.maxHealth;
                    }
                }
            }
        }
    }

    public void SetNewHp(bool set)
    {
        health += baseHealth * 0.1f;
        maxHealth += baseHealth * 0.1f;
        hpBarTransform.localScale = new Vector3(health / maxHealth, hpBarTransform.localScale.y, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.SetUpText();
        }
        hp++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].hp = hp;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (hp == 5)
            DungeonData.instance.AddToQuest(5, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }
    public void SetNewReg(bool set)
    {
        extraHealthRegen += newReg;
        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.SetUpText();
        }
        reg++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].reg = reg;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (reg == 5)
            DungeonData.instance.AddToQuest(6, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }
    public void SetNewSPeed(bool set)
    {
        originalSpeed += 0.75f;
        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.SetUpText();
        }
        spd++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].spd = spd;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (spd == 5)
            DungeonData.instance.AddToQuest(7, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }
    public void SetNewDex(bool set)
    {
        attackSpeed += newDex;
        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.SetUpText();
        }
        dex++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].dex = dex;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (dex == 5)
            DungeonData.instance.AddToQuest(8, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }
    public void SetNewCdr(bool set)
    {
        cooldownReduction += newCdr;
        if (caracterId == 1)
        {
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tankCharge -= 300;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack6")
                    abilMax[i] -= 300;
            }

            tank.AddDamage(0);
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            suppCharge -= 400;

            for (int i = 0; i <= 2; i++)
            {
                if (attacks[i] == "attack3")
                    abilMax[i] -= 400;
            }

            supportClass.AddDamage(0);
            supportClass.SetUpText();
        }
        cdr++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].cdr = cdr;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (cdr == 5)
            DungeonData.instance.AddToQuest(10, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }
    public void SetNewStr(bool set)
    {
        if (caracterId == 1)
        {
            wizard.staffDamage += baseDamage * 0.3f;
            wizard.SetUpText();
        }
        else if (caracterId == 2)
        {
            knight.weaponDamage += baseDamage * 0.3f;
            knight.SetUpText();
        }
        else if (caracterId == 3)
        {
            archer.arrowDamage += baseDamage * 0.3f;
            archer.SetUpText();
        }
        else if (caracterId == 4)
        {
            tank.weaponDamage += baseDamage * 0.3f;
            tank.SetUpText();
        }
        else if (caracterId == 5)
        {
            supportClass.weaponDamage += baseDamage * 0.33f;
            supportClass.SetUpText();
        }
        str++;
        if (set)
        {
            pointsLeft--;
            dungeonData.statuses[caracterId].str = str;
            dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
        }

        if (str == 5)
            DungeonData.instance.AddToQuest(9, 1);

        if (hp > 0 && reg > 0 && spd > 0 && dex > 0 && str > 0 && cdr > 0)
            DungeonData.instance.AddToQuest(30, 1);
    }

    Dictionary<GameObject, int> comboList = new Dictionary<GameObject, int>();
    public void Combo(GameObject mob)
    {
        if (bot == false && inDungeon)
        {
            if (comboList.ContainsKey(mob) == false)
                comboList.Add(mob, 0);

            comboList[mob]++;

            UnityEngine.Debug.Log(comboList[mob]);

            if (comboList[mob] == 10)
                DungeonData.instance.AddToQuest(11, 1);
        }
    }

    public void ClearComboList(List<GameObject> hitMobs)
    {
        UnityEngine.Debug.Log("Clear");

        List<GameObject> badMobs = new List<GameObject>();

        foreach(GameObject mob in comboList.Keys)
        {
            if(hitMobs.Contains(mob) == false)
            {
                badMobs.Add(mob);
            }
        }

        foreach (GameObject mob in badMobs)
            comboList.Remove(mob);
    }

    public bool ableToMove = true;

    public int hp;
    public int reg;
    public int str;
    public int dex;
    public int cdr;
    public int spd;
    public int pointsLeft = 0;
    public int pointsAvailable = 5;

    public AbilityDesc[] abilityDescs;

    public Sprite[] abilityImages;

    public bool noKnockback = false;

    public bool online = false;
    public bool controlling = true;
    public float timeTillStop = 0f;


    public GameObject[] limbs;
    public string[] attacks;
    public float health;

    public float speed;


    public int[] buttons;


    public GameObject weapon;
    public GameObject[] hitboxes;
    public bool hurt = false;
    public GameObject fireParticles;
    public GameObject ice;
    public GameObject iceParticles;
    int fireCombo = 0;
    public GameObject centerOfMass;
    public float originalSpeed;
    public bool stunned = false;
    public GameObject stunObj;
    bool nonStop = false;
    public GameObject nonStopObj;
    public float nonStopTime;

    public int caracterId;

    public int playerNumber;

    public SpriteRenderer[] abilImages;
    public bool[] canDoAbils;

    public GameObject hpBar;

    new public TextMeshPro name;
    public AudioSource hit;

    public AudioSource healSfx;
    public float healValue;

    public SelectableItem abilSlot1;
    public SelectableItem abilSlot2;
    public SelectableItem abilSlot3;

    float t = 0;
    public void HealPlayer()
    {
        Heal(healValue);
    }

    public void LockPlayer(Collider2D collider)
    {
        if (locked == false)
        {
            currentBox = collider;
            locked = true;
            oldPos = newPos = playerTransform.position;
        }
    }
    public void UnlockPlayer()
    {
        locked = false;
        circleCollider.enabled = false;
        circleCollider.enabled = true;
    }
    public bool locked = false;
    public bool parkourRoom = false;
    public float parkourSpeed;
    public Collider2D currentBox;
    public void Heal(float hpAmount)
    {
        if (bot == false && this.inDungeon)
        {
            for (int i = 0; i < inventorySpawn.itemSlots.Length; i++)
            {
                if (inventorySpawn.itemSlots[i].containsItem)
                {
                    if (inventorySpawn.itemSlots[i].itemType == 3)
                    {
                        health += 0.2f * hpAmount;
                    }
                }
            }
        }
        health += hpAmount * (float)reg / 10f;
        health += hpAmount;

        DungeonData.instance.AddToQuest(25, (int)hpAmount);

        if (health > maxHealth)
            health = maxHealth;
        hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        healthObjVfx.Play();
        healSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        healSfx.Play();
        cameraFollow.RecalculateDamage();
    }

    public void HealPercent(float hpAmount)
    {
        health += hpAmount * maxHealth;

        DungeonData.instance.AddToQuest(25, (int)(hpAmount * maxHealth));

        if (health > maxHealth)
            health = maxHealth;
        hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        healthObjVfx.Play();
        healSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        healSfx.Play();
        cameraFollow.RecalculateDamage();
    }

    public GameObject shield;
    public bool shielded = false;
    public GameObject shieldBreak;
    public AudioSource shieldSfx;
    public void Shield()
    {
        shield.SetActive(true);
        shielded = true;
    }
    public void BreakShield()
    {
        shield.SetActive(false);
        shieldBreakVfx.Play();
        shielded = false;
        shieldSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        shieldSfx.Play();
    }

    void EnableAbil1()
    {
        ActivateAbillity(1);
    }
    void EnableAbil2()
    {
        ActivateAbillity(2);
    }
    void EnableAbil3()
    {
        ActivateAbillity(3);
    }

    public void StartNonStop(float time)
    {
        CancelInvoke("StopNonStop");
        nonStop = true;
        nonStopObj.SetActive(true);
        Invoke("StopNonStop", time);
    }
    void StopNonStop()
    {
        nonStopObj.SetActive(false);
        nonStop = false;
    }
    public bool slowed = false;
    public void Slow(float slowTime)
    {
        if (health > 0)
        {
            CancelInvoke("StopSlow");
            slowed = true;
            slowParticles.SetActive(true);
            Invoke("StopSlow", slowTime);
        }
    }
    void StopSlow()
    {
        slowParticles.SetActive(false);
        slowed = false;
    }
    public AudioSource stun;

    public void Weaken()
    {
        if (health > 0)
        {
            CancelInvoke("StopWeaken");
            weaken.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            weaken.Play();
            weakenObj.SetActive(true);
            Invoke("StopWeaken", 1.5f);
        }
    }

    public void StopWeaken()
    {
        weakenObj.SetActive(false);
    }

    bool roomSilence = false;
    float ogogSpeed;

    public void ResetOGSpeed()
    {
        originalSpeed = ogogSpeed;
    }

    public void ParkourSilence()
    {
        if (health > 0)
        {
            ogogSpeed = originalSpeed;
            originalSpeed = 3;
            roomSilence = true;
            CancelInvoke("StopSilence");
            silence.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            silence.Play();
            silenceObj.SetActive(true);
            Invoke("StopSilence", 9999999f);
        }
    }

    public void Silence()
    {
        if (health > 0 && roomSilence == false)
        {
            CancelInvoke("StopSilence");
            silence.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            silence.Play();
            silenceObj.SetActive(true);
            Invoke("StopSilence", 1.5f);
        }
    }

    public void StopSilence()
    {
        if (silenceObjNotNull)
        {
            CancelInvoke("StopSilence");
            roomSilence = false;
            silenceObj.SetActive(false);
        }
    }

    public void Blind()
    {
        blind.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        blind.Play();
        blindness.Enable();
    }

    public void StopBlind()
    {

    }

    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //

    public void Stun(float stunTime)
    {
        bool isInvulnerable = false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (bot == false && inDungeon)
            isInvulnerable = (stateInfo.IsName(attacks[0]) && abilSlot1.lvl >= 6) || (stateInfo.IsName(attacks[1]) && abilSlot2.lvl >= 6) || (stateInfo.IsName(attacks[2]) && abilSlot3.lvl >= 6);
        if (((noKnockback == false && isInvulnerable == false && finalBoss == false) || (finalBoss && currentAttack >= 4 && stunned == false) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks() && stunned == false && finalBoss == false) || (isBoss & bossScene && stunned == false && finalBoss == false)) && health > 0)
        {
            if (currentAttack >= 4)
            {
                if (currentAttack == 6)
                    KillMobs();
                stunTime = 5;
            }
            if (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks())
            {
                stunTime = 5;
                CancelInvoke("ResetBossAttacks");
            }
            stun.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            stun.Play();
            CancelInvoke("StopStun");
            stunned = true;
            stunObj.SetActive(true);
            CancelInvoke("RbStop");
            Invoke("RbStop", 0.25f);
            Invoke("StopStun", stunTime);
        }
    }

    void RbStop()
    {
        if (stunned)
            rb.velocity = myVelocity = Vector2.zero;
    }
    void StopStun()
    {
        stunned = false;
        stunObj.SetActive(false);
        if (currentAttack >= 4)
        {
            currentAttack = 0;
        }
        if (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks())
        {
            bossCurrentAttack = 0;
        }
    }
    int poisonCombo = 0;
    public GameObject poisonParticles;
    public void Poison(player player, float str)
    {
        if (health > 0)
        {
            bonusPoisonDamage = str * 14;

            speed = originalSpeed / 3;
            poisonCombo = 0;
            poisonParticles.SetActive(true);
            lastHit = player;
            CancelInvoke("PoisonDamage");
            Invoke("PoisonDamage", 0.5f);
        }
    }

    public void ResetAttacks()
    {
        bossCurrentAttack = 0;
    }

    public float maxHealth;
    void PoisonDamage()
    {
        if (health > 0)
        {
            poisonCombo++;
            if (poisonCombo == 4)
            {
                poisonCombo = 0;
                poisonParticles.SetActive(false);
                speed = originalSpeed;
            }
            else
            {
                if ((botInShops && bot) || botInShops == false)
                {
                    //if ((isBoss || finalBoss) && stunned)
                    //    health -= 50;
                    //else
                    health -= 25 + bonusPoisonDamage;
                    if (botInShops && bot)
                        healthTextObj.SetActive(true);
                    else if (botInShops == false && (enemyBarActive || isBoss || finalBoss) && bot) { healthBarObj.SetActive(true); if (barFadeActive && isBoss == false && finalBoss == false) { CancelInvoke("DeactivateBar"); Invoke("DeactivateBar", 5); } }
                }
                if (bot && isBoss && health > 0)
                {
                    healthTillSpawnMobs -= 25 + bonusPoisonDamage;
                    if (healthTillSpawnMobs <= 0)
                    {
                        bossRoomScript.BossSpawnMobs();
                        healthTillSpawnMobs = maxHealthTillSpawnMobs;
                    }
                }
                if (health <= 0)
                {
                    if (bot == false && inDungeon)
                    {
                        for (int i = 0; i < inventorySpawn.itemSlots.Length; i++)
                        {
                            SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                            if (itemSlot.containsItem)
                            {
                                if (itemSlot.itemType == 7)
                                {
                                    Heal(350);
                                    itemSlot.usesLeft--;
                                    if (itemSlot.usesLeft <= 0)
                                        itemSlot.RemoveItem();
                                    else itemSlot.abilityDescription.GetComponentInParent<AbilityDesc>().abilityText.text = inventorySpawn.itemDescs[7] + " Remaining uses: <color=red><b>" + itemSlot.usesLeft + "</b></color>.";

                                    DungeonData.instance.AddToQuest(31, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
                hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
                if (botInShops && bot)
                    healthText.text = ((int)health).ToString();
                if (health <= 0)
                {
                    StopIce();
                    CancelInvoke("FireParticles");
                    fireParticles.SetActive(false);
                    CancelInvoke("PoisonParticles");
                    poisonParticles.SetActive(false);
                    CancelInvoke("StopStun");
                    StopStun();
                    if (speedObjNotNull)
                    {
                        CancelInvoke("StopSpeed");
                        StopSpeed();
                    }
                    CancelInvoke("StopSlow");
                    StopSlow();
                    if (mobAuraNotNull)
                        mobAura.SetActive(false);
                    if (silenceObjNotNull)
                    {
                        CancelInvoke("StopSilence");
                        StopSilence();
                    }
                    if (weakenObjNotNull)
                    {
                        CancelInvoke("StopWeaken");
                        StopWeaken();
                    }
                    health = 0;
                    hpBarTransform.localScale = new Vector2(0, 1);
                    if (botInShops && bot)
                        healthText.text = ((int)health).ToString();

                    animState_death = animator.GetCurrentAnimatorStateInfo(0).IsName("death");

                    if (bot && animState_death == false)
                    {
                        if (isBoss && bossScene == false)
                        {
                            TimeController.instance.SlowMotion();
                            bossRoomScript.BossKillMobs(lastHit);
                        }
                    }
                    if (bot && animState_death == false && finalBoss == false)
                    {
                        if (revived == false && botInShops == false && bossScene == false)
                        {
                            if (UnityEngine.Random.value > 0.9 && revived == false && botInShops == false)
                            {
                                dungeonData.RemoveCoins(-1);
                            }
                            int value = 7 + UnityEngine.Random.Range(-3, 3);
                            if (isBoss)
                                value *= 10;
                            if (SceneName == "TutorialScene")
                                value *= 5;
                            inventorySpawn.AddCoins(value);
                            GameObject vfx = Instantiate(coinVfx); vfx.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                            vfx.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();
                            if (caracterId == 4)
                            {
                                vfx.transform.position = new Vector2(playerTransform.position.x + 4.52f, playerTransform.position.y);
                            }
                            else vfx.transform.position = new Vector2(playerTransform.position.x + 4.12f, playerTransform.position.y);
                            vfx.transform.parent = Camera.main.transform;
                        }
                        aiPath.canMove = false;
                        aiDestinationSetter.target = null;
                    }
                    if (bot == false && inDungeon)
                        TimeController.instance.SlowMotion();
                    Stats.SetActive(false);
                    hpBarObj.SetActive(false);
                    healthTextObj.SetActive(false);
                    rb.velocity = myVelocity = new Vector2(0, 0);
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    if (finalBoss)
                    {
                        animator.Play("death");
                        FindObjectOfType<FinalBossScene>().InvokeNextFaze();
                        CancelInvoke("Expl");
                        foreach (borderExpl border in FindObjectsOfType<borderExpl>())
                        {
                            border.StopBorder();
                        }
                        GameObject.Find("Borders").SetActive(false);
                    }
                    else
                    {

                        animator.Play("death");
                        circleCollider.enabled = false;
                    }
                    CancelInvoke("PoisonDamage");
                    return;
                }
                if (bot)
                {
                    float xpValue = (25f + bonusPoisonDamage) / maxHealth * xpDrop;

                    if (maxXp + xpValue > xpDrop)
                    {
                        xpValue = xpDrop - maxXp;
                        maxXp = xpDrop;
                    }
                    else maxXp += damage / maxHealth * xpDrop;
                    lastHit.AddXp(xpValue);
                }
                int index = 0;
                for (int i = 0; i < limbSprites.Count; i++)
                {
                    if (spriteForDamage)
                        limbSprites[i].sprite = sprites[index++];
                    else limbSprites[i].color = new Color32(255, 0, 0, 255);
                }
                hit.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                hit.Play();
                Invoke("StopDamage", 0.25f);
                Invoke("PoisonDamage", 0.5f);
            }
        }
    }
    float bonusFireDamage;
    float bonusPoisonDamage;
    public void SetOnFire(float str)
    {
        if (finalBoss == false && health > 0)
        {
            bonusFireDamage = str * 14;

            fireCombo = 0;
            fireParticles.SetActive(true);
            CancelInvoke("FireDamage");
            Invoke("FireDamage", 0.5f);
        }
    }
    void FireDamage()
    {
        if (health > 0)
        {
            fireCombo++;
            if (fireCombo == 4)
            {
                fireCombo = 0;
                fireParticles.SetActive(false);
            }
            else
            {
                if ((botInShops && bot) || botInShops == false)
                {
                    //if ((isBoss || finalBoss) && stunned)
                    //    health -= 50;
                    //else
                    health -= 25 + bonusFireDamage;
                    if (botInShops && bot)
                        healthTextObj.SetActive(true);
                    //else if(botInShops==false) healthBarObj.SetActive(true);
                }
                if (bot && isBoss && health > 0)
                {
                    healthTillSpawnMobs -= 25 + bonusFireDamage;
                    if (healthTillSpawnMobs <= 0)
                    {
                        bossRoomScript.BossSpawnMobs();
                        healthTillSpawnMobs = maxHealthTillSpawnMobs;
                    }
                }
                if (health <= 0)
                {
                    if (bot == false && inDungeon)
                    {
                        for (int i = 0; i < inventorySpawn.itemSlots.Length; i++)
                        {
                            SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                            if (itemSlot.containsItem)
                            {
                                if (itemSlot.itemType == 7)
                                {
                                    Heal(350);
                                    itemSlot.usesLeft--;
                                    if (itemSlot.usesLeft <= 0)
                                        itemSlot.RemoveItem();
                                    else itemSlot.abilityDescription.GetComponentInParent<AbilityDesc>().abilityText.text = inventorySpawn.itemDescs[7] + " Remaining uses: <color=red><b>" + itemSlot.usesLeft + "</b></color>.";

                                    DungeonData.instance.AddToQuest(31, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
                hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
                if (botInShops && bot)
                    healthText.text = ((int)health).ToString();
                if (health <= 0)
                {
                    StopIce();
                    CancelInvoke("FireParticles");
                    fireParticles.SetActive(false);
                    CancelInvoke("PoisonParticles");
                    poisonParticles.SetActive(false);
                    CancelInvoke("StopStun");
                    StopStun();
                    if (speedObjNotNull)
                    {
                        CancelInvoke("StopSpeed");
                        StopSpeed();
                    }
                    CancelInvoke("StopSlow");
                    StopSlow();
                    if (mobAuraNotNull)
                        mobAura.SetActive(false);
                    if (silenceObjNotNull)
                    {
                        CancelInvoke("StopSilence");
                        StopSilence();
                    }
                    if (weakenObjNotNull)
                    {
                        CancelInvoke("StopWeaken");
                        StopWeaken();
                    }
                    health = 0;
                    hpBarTransform.localScale = new Vector2(0, 1);
                    if (botInShops && bot)
                        healthText.text = ((int)health).ToString();

                    animState_death = animator.GetCurrentAnimatorStateInfo(0).IsName("death");

                    if (bot && animState_death == false)
                    {
                        if (isBoss && bossScene == false)
                        {
                            TimeController.instance.SlowMotion();
                            bossRoomScript.BossKillMobs(lastHit);
                        }
                    }
                    if (bot && animState_death == false && finalBoss == false)
                    {
                        if (revived == false && botInShops == false && bossScene == false)
                        {
                            if (UnityEngine.Random.value > 0.9 && revived == false && botInShops == false)
                            {
                                dungeonData.RemoveCoins(-1);
                            }
                            int value = 7 + UnityEngine.Random.Range(-3, 3);
                            if (isBoss)
                                value *= 10;
                            if (SceneName == "TutorialScene")
                                value *= 5;
                            inventorySpawn.AddCoins(value);
                            GameObject vfx = Instantiate(coinVfx); vfx.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                            vfx.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();
                            if (caracterId == 4)
                            {
                                vfx.transform.position = new Vector2(playerTransform.position.x + 4.52f, playerTransform.position.y);
                            }
                            else vfx.transform.position = new Vector2(playerTransform.position.x + 4.12f, playerTransform.position.y);
                            vfx.transform.parent = Camera.main.transform;
                        }
                        aiPath.canMove = false;
                        aiDestinationSetter.target = null;
                    }
                    if (bot == false && inDungeon)
                        TimeController.instance.SlowMotion();
                    Stats.SetActive(false);
                    hpBarObj.SetActive(false);
                    healthTextObj.SetActive(false);
                    rb.velocity = myVelocity = new Vector2(0, 0);
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    if (finalBoss)
                    {
                        animator.Play("death");
                        FindObjectOfType<FinalBossScene>().InvokeNextFaze();
                        CancelInvoke("Expl");
                        foreach (borderExpl border in FindObjectsOfType<borderExpl>())
                        {
                            border.StopBorder();
                        }
                        GameObject.Find("Borders").SetActive(false);
                    }
                    else
                    {

                        animator.Play("death");
                        circleCollider.enabled = false;
                    }
                    CancelInvoke("FireDamage");
                    return;
                }
                if (bot)
                {
                    float xpValue = damage / maxHealth * xpDrop;

                    if (maxXp + xpValue > xpDrop)
                    {
                        xpValue = xpDrop - maxXp;
                        maxXp = xpDrop;
                    }
                    else maxXp += (25f + bonusFireDamage) / maxHealth * xpDrop;
                    lastHit.AddXp(xpValue);
                }
                int index = 0;
                for (int i = 0; i < limbSprites.Count; i++)
                {
                    if (spriteForDamage)
                        limbSprites[i].sprite = sprites[index++];
                    else limbSprites[i].color = new Color32(255, 0, 0, 255);
                }
                hit.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                hit.Play();
                Invoke("StopDamage", 0.25f);
                Invoke("FireDamage", 0.5f);
            }
        }
    }
    public AudioSource iceHit;
    public AudioSource iceBreak;

    public void Ice(float iceSpeed = -1)
    {
        bool isInvulnerable = false;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        if (bot == false && inDungeon)
            isInvulnerable = (animState.IsName(attacks[0]) && abilSlot1.lvl >= 6) || (animState.IsName(attacks[1]) && abilSlot2.lvl >= 6) || (animState.IsName(attacks[2]) && abilSlot3.lvl >= 6);
        if (((noKnockback == false && isInvulnerable == false && finalBoss == false) || (finalBoss && currentAttack >= 4) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks() && finalBoss == false)) && health > 0)
        {
            bool activeIce = ice.activeInHierarchy;
            if (activeIce)
            {
                StopIce(activeIce, animState.IsName("death"));
            }
            else
            {
                if ((finalBoss && currentAttack >= 4) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks()))
                    Stun(1.5f);
                else
                {
                    if (iceSpeed != -1)
                    {
                        myVelocity = rb.velocity.normalized * iceSpeed;
                    }
                    myVelocity *= 3;

                    rb.velocity = myVelocity;

                    iceHit.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                    iceHit.Play();
                    ice.SetActive(true);
                    canAttack = false;
                    CancelInvoke("FireDamage");
                    CancelInvoke("StopIce");
                    rb.drag = 1.5f;
                    fireParticles.SetActive(false);

                    Invoke("StopIce", 1.5f);
                }
            }
        }
    }
    void StopIce()
    {
        rb.drag = 0;
        if (ice.activeInHierarchy == true)
        {
            if (bot && animState_death == false)
                aiPath.canMove = true;

            iceBreak.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            iceBreak.Play();
            canAttack = true;
            canAttackButWithMovement = false;
            ice.SetActive(false);
            GameObject iceVfx = Instantiate(iceParticles);
            Destroy(iceVfx, 1f);
            iceVfx.transform.position = playerTransform.position;
        }
    }
    void StopIce(bool iceActive, bool dead)
    {
        rb.drag = 0;
        if (iceActive == true)
        {
            if (bot && dead == false)
                aiPath.canMove = true;

            iceBreak.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            iceBreak.Play();
            canAttack = true;
            canAttackButWithMovement = false;
            ice.SetActive(false);
            GameObject iceVfx = Instantiate(iceParticles);
            Destroy(iceVfx, 1f);
            iceVfx.transform.position = playerTransform.position;
        }
    }
    public GameObject limbsObj;
    public void SetNormalLimbRot()
    {
        limbsObj.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    public void SpamDirection()
    {
        spamDirection = true;
    }
    public GameObject Stats;
    public void AddXp(float xp)
    {
        if (botInShops == false)
        {
            this.xp += xp;
            if (this.xp >= 150 + lvl * 50)
            {
                this.xp -= (150 + lvl * 50);
                lvl++;
                DungeonData.instance.AddToQuest(14, 1);
                abilityPoints++;
                GameObject vfx = Instantiate(levelVfx);
                vfx.transform.position = new Vector2(playerTransform.position.x + distFromCenter, playerTransform.position.y);
                levelup.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                levelup.Play();
            }
        }
    }
    public void AddLevels(int levels)
    {

        lvl += levels;
        abilityPoints += levels;

        DungeonData.instance.AddToQuest(14, levels);

        GameObject vfx = Instantiate(levelVfx);
        vfx.transform.position = new Vector2(playerTransform.position.x + distFromCenter, playerTransform.position.y);
        levelup.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        levelup.Play();

    }
    player lastHit;
    public GameObject bossRoom;
    public bool boss = false;
    public float healthTillSpawnMobs = 0;
    public float maxHealthTillSpawnMobs;

    void DeactivateBar()
    {
        healthBarObj.SetActive(false);
    }

    public LockRoom lockRoom;
    public void DecreaseHp(float damage, Vector2 direction, player player, int cutSound = 1, bool giveCoins = true)
    {
        if (!gameObject.scene.isLoaded)
        {
            // UW: Game is exiting; don't create any new particles
            // (for some reason, this function is called from other event handles while objects are being destroyed)
            return;
        }

        if ((finalBoss || isBoss) && stunned == false)
            damage /= 4;
        //if ((isBoss || finalBoss) && (stunned || (finalBoss && currentAttack >= 4) || (isBoss && bossCurrentAttack > 7)))
        //    damage *= 2;

        bool isInvulnerable = false;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        if (bot == false && inDungeon)
        {
            if (lockRoom != null)
                lockRoom.gotDamaged = true;

            isInvulnerable = (animState.IsName(attacks[0]) && abilSlot1.lvl >= 6) || (animState.IsName(attacks[1]) && abilSlot2.lvl >= 6) || (animState.IsName(attacks[2]) && abilSlot3.lvl >= 6);
        }

        if (bot && t < 1)
        {
            t = 1;


            try
            {
                for (int i = 0; i < childrenSprites.Length; i++)
                {
                    SpriteRenderer sprite = childrenSprites[i];

                    sprite.color = new Color32((byte)(sprite.color.r * 255), (byte)(sprite.color.g * 255), (byte)(sprite.color.b * 255), 255);
                }
            }
            catch
            {

            }

            try
            {
                for(int i=0;i<limbColors.Count;i++)
                {
                    limbColors[i].Start();
                }
            }
            catch
            {

            }

            //randomAroundTarget.target = null;
        }

        if (maxHealth == 0)
            maxHealth = health;
        if (health > 0)
        {
            if ((finalBoss && currentAttack >= 4) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks()))
            {
                Stun(1.5f);
            }
            try
            {
                GameObject damageClone = Instantiate(damageEffect);
                damageClone.transform.position = playerTransform.position;
                if (boss || finalBoss)
                    damageClone.transform.localScale = new Vector2(25, 25) / 15;
                Destroy(damageClone, 2f);
                if (health <= 0)
                {
                    if (isBoss)
                    {
                        for (int i = 1; i <= 2; i++)
                        {
                            GameObject damageClone2 = Instantiate(damageEffect);
                            damageClone2.transform.position = playerTransform.position;
                            damageClone2.transform.localScale = new Vector2(25, 25) / 15;
                            Destroy(damageClone2, 2f);
                        }
                    }
                    else if (finalBoss)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            GameObject damageClone2 = Instantiate(damageEffect);
                            damageClone2.transform.position = playerTransform.position;
                            damageClone2.transform.localScale = new Vector2(25, 25) / 15;
                            Destroy(damageClone2, 2f);
                        }
                    }
                }
            }
            catch
            {

            }
            if (noKnockback == false && isInvulnerable == false)
            {
                if (poisonParticles.activeInHierarchy == false)
                    speed = originalSpeed;
                spamDirection = false;
            }

            hit.volume = PlayerPrefs.GetFloat("SFXVolume", 1) / cutSound;
            hit.Play();

            if (weaponHitboxNotNull && noKnockback == false && isInvulnerable == false)
            {
                weaponHitbox.playerDirection = true;
                weaponHitbox.limbDirection = false;
            }
            if ((botInShops && bot) || botInShops == false)
            {
                health -= damage;
                if (botInShops && bot)
                    healthTextObj.SetActive(true);
                else if (botInShops == false && (enemyBarActive || isBoss || finalBoss) && bot) { healthBarObj.SetActive(true); if (barFadeActive && isBoss == false && finalBoss == false) { CancelInvoke("DeactivateBar"); Invoke("DeactivateBar", 5); } }
            }
            if (finalBoss)
            {
                if (isInShops)
                {
                    health = maxHealth;
                }
                if (health <= maxHealth / 2)
                    Invoke("Expl", 2f);
                if (health <= maxHealth / 4)
                    Invoke("Expl", 1.5f);
                if (health <= maxHealth / 8)
                    Invoke("Expl", 1f);
                if (health <= maxHealth / 16)
                    Invoke("Expl", 0.5f);
            }
            if (health <= 0)
            {
                if (bot == false && inDungeon && finalBoss == false)
                {
                    for(int i=0;i< inventorySpawn.itemSlots.Length;i++)
                    {
                        SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                        if (itemSlot.containsItem)
                        {
                            if (itemSlot.itemType == 7)
                            {
                                Heal(350);
                                itemSlot.usesLeft--;
                                if (itemSlot.usesLeft <= 0)
                                    itemSlot.RemoveItem();
                                else itemSlot.abilityDescription.GetComponentInParent<AbilityDesc>().abilityText.text = inventorySpawn.itemDescs[7] + " Remaining uses: <color=red><b>" + itemSlot.usesLeft + "</b></color>.";

                                DungeonData.instance.AddToQuest(31, 1);
                                break;
                            }
                        }
                    }
                }
            }
            try
            {
                hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
                if (botInShops && bot)
                    healthText.text = ((int)health).ToString();
            }
            catch
            {

            }
            if (bot && parkourRoom == false && bossScene == false)
            {
                float xpValue = damage / maxHealth * xpDrop;

                if (maxXp + xpValue > xpDrop)
                {
                    xpValue = xpDrop - maxXp;
                    maxXp = xpDrop;
                }
                else maxXp += damage / maxHealth * xpDrop;
                player.AddXp(xpValue);
                {
                    foreach (player p in FindObjectsOfType<player>())
                    {

                        if (p.finalBoss && finalBoss == false)
                        {
                            Destroy(myObj);
                            return;
                        }

                    }
                }
                lastHit = player;
                if (isBoss && health > 0)
                {
                    healthTillSpawnMobs -= damage;
                    if (healthTillSpawnMobs <= 0)
                    {
                        bossRoomScript.BossSpawnMobs();
                        healthTillSpawnMobs = maxHealthTillSpawnMobs;
                    }
                }
            }
            if (bot == false && inDungeon && online == false && finalBoss == false)
            {
                for(int i=0;i< inventorySpawn.itemSlots.Length;i++)
                {
                    SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                    if (itemSlot.containsItem)
                    {
                        if (itemSlot.itemType == 1)
                        {
                            if (UnityEngine.Random.Range(1, 999999) % 3 == 0)
                            {
                                GameObject cloneBomb = Instantiate(bomb);
                                cloneBomb.transform.position = playerTransform.position;
                                bomb bombScript = cloneBomb.GetComponent<bomb>();
                                bombScript.active = true;
                                bombScript.parent = this;
                            }
                        }
                    }
                }
            }
            StopIce();
            if (health <= 0)
            {
                CancelInvoke("FireParticles");
                fireParticles.SetActive(false);
                CancelInvoke("PoisonParticles");
                poisonParticles.SetActive(false);
                CancelInvoke("StopStun");
                StopStun();
                if (speedObjNotNull)
                {
                    CancelInvoke("StopSpeed");
                    StopSpeed();
                }
                CancelInvoke("StopSlow");
                StopSlow();
                if (mobAuraNotNull)
                    mobAura.SetActive(false);
                if (silenceObjNotNull)
                {
                    CancelInvoke("StopSilence");
                    StopSilence();
                }
                if (weakenObjNotNull)
                {
                    CancelInvoke("StopWeaken");
                    StopWeaken();
                }
                health = 0;
                hpBarTransform.localScale = new Vector2(0, 1);
                if (botInShops && bot)
                    healthText.text = ((int)health).ToString();


                if (bot && finalBoss == false)
                {
                    if (isBoss && bossScene == false)
                    {
                        TimeController.instance.SlowMotion();
                        bossRoomScript.BossKillMobs(player);
                    }
                    if (revived == false && botInShops == false && parkourRoom == false && bossScene == false)
                    {
                        int value = 7 + UnityEngine.Random.Range(-3, 3);
                        if (isBoss)
                            value *= 10;
                        if (SceneName == "TutorialScene")
                            value *= 5;
                        inventorySpawn.AddCoins(value);
                        GameObject vfx = Instantiate(coinVfx);
                        vfx.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume", 1) / cutSound;
                        vfx.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();
                        if (caracterId == 4)
                        {
                            vfx.transform.position = new Vector2(playerTransform.position.x + 4.52f, playerTransform.position.y);
                        }
                        else vfx.transform.position = new Vector2(playerTransform.position.x + 4.12f, playerTransform.position.y);
                        vfx.transform.parent = Camera.main.transform;
                    }
                    aiPath.canMove = false;
                    aiDestinationSetter.target = null;
                }
                else if (inDungeon && finalBoss == false)
                {
                    TimeController.instance.SlowMotion();
                    bool ok = false;
                    CharacterSlot playerSlot = null;
                    foreach (CharacterSlot slot in inventorySpawn.inventory.GetComponent<Inventory>().slots)
                    {
                        if (slot.characterId == caracterId)
                        {
                            playerSlot = slot;
                            ok = true;
                            break;
                        }
                    }
                    if (ok)
                    {
                        playerSlot.xpBar.transform.localScale = new Vector3(xp / (100 + lvl * 25), playerSlot.xpBar.transform.localScale.y, 1);
                        playerSlot.characterXp.text = xp + "/" + 100 + lvl * 25;
                        playerSlot.characterLevel.text = lvl.ToString();
                        playerSlot.APText.text = "AP: " + abilityPoints;
                        playerSlot.MPText.text = "MP: " + masterPoints;
                    }
                }
                Stats.SetActive(false);
                rb.velocity = myVelocity = new Vector2(0, 0);
                rb.bodyType = RigidbodyType2D.Kinematic;
                if (bot && finalBoss == false && parkourRoom == false)
                {
                    if (UnityEngine.Random.value > 0.9 && revived == false && botInShops == false)
                    {
                        dungeonData.RemoveCoins(-1);
                    }
                    if (damage < 5000)
                    {
                        foreach (support Player in FindObjectsOfType<support>())
                        {
                            if (Player.player.bot && Player.player.mobToRevive == null && Player.gameObject != myObj)
                            {
                                Player.player.mobToRevive = myObj;
                                break;
                            }
                        }
                    }
                }
                if (finalBoss)
                {
                    animator.Play("death");
                    FindObjectOfType<FinalBossScene>().InvokeNextFaze();
                    CancelInvoke("Expl");
                    foreach (borderExpl border in FindObjectsOfType<borderExpl>())
                    {
                        border.StopBorder();
                    }
                    GameObject.Find("Borders").SetActive(false);
                }
                else
                {

                    animator.Play("death");
                    circleCollider.enabled = false;
                }
            }
            else if ((noKnockback == false && isInvulnerable == false && finalBoss == false) || (finalBoss && stunned) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks() && finalBoss == false) || (isBoss && bossScene && stunObj.activeInHierarchy && finalBoss == false))
            {


                if (online == false)
                {
                    CancelInvoke("StopDamage");
                    StopAttack(false);
                    hurt = true;
                    if (animState.IsName("hurt2"))
                        animator.Play("hurt");
                    else animator.Play("hurt2");
                    if ((noKnockback == false && isInvulnerable == false) || (finalBoss && stunned) || (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks()) || (isBoss && bossScene && stunObj.activeInHierarchy))
                    {
                        /*Vector2 newestPos = (Vector2)playerTransform.position + direction;

                        Collider2D[] boxesX = Physics2D.OverlapBoxAll(new Vector2(newestPos.x, playerTransform.position.y), new Vector2(0.01f, 0.01f), 0f);
                        Collider2D[] boxesY = Physics2D.OverlapBoxAll(new Vector2(playerTransform.position.x, newestPos.y), new Vector2(0.01f, 0.01f), 0f);
                        bool okX = false;
                        bool okY = false;

                        foreach (Collider2D box in boxesX)
                        {
                            if (box.GetComponent<room>() != null)
                            {
                                okX = true;
                            }
                        }
                        foreach (Collider2D box in boxesY)
                        {
                            if (box.GetComponent<room>() != null)
                            {
                                okY = true;
                            }
                        }

                        if (okX || inDungeon == false)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, direction.y);
                        }
                        if (okY || inDungeon == false)
                        {
                            rb.velocity = new Vector2(direction.x, rb.velocity.y);
                        }*/


                        if (lastKnockbackCoroutine != null)
                            StopCoroutine(lastKnockbackCoroutine);
                        lastKnockbackCoroutine = StartCoroutine(Knockback(direction, 0.367f));
                    }

                    canAttack = false;
                    int index = 0;
                    rb.sharedMaterial.bounciness = 1f;
                    circleCollider.enabled = false;
                    circleCollider.enabled = true;
                    hurt1 = !hurt1;
                    if (noKnockback == false && isInvulnerable == false)
                    {
                        if (bot)
                        {
                            foreach (support Player in FindObjectsOfType<support>())
                            {
                                
                                if (Player.player.bot && Player.player.mobToHeal == null && Player.gameObject != myObj)
                                {
                                    Player.player.mobToHeal = myObj;
                                    Player.player.MedicHeal();
                                    break;
                                }
                            }
                        }
                    }

                }
                if (online)
                {
                    FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "hurt" + " " + direction.x + " " + direction.y);
                }
            }
            damaged = true;
            if (bot == false && inDungeon && health > 0 && botInShops == false)
                cameraFollow.Damage(0.25f);
        }

    }



    public GameObject coinVfx;
    bool hurt1 = true;
    int damage = 0;

    public void StopDamage(bool force)
    {
        if (damaged == false || force)
        {
            CancelInvoke("StopDamage");

            if (stunned)
                rb.velocity = myVelocity = Vector2.zero;

            int index = 0;

            for (int i = 0; i < limbSprites.Count; i++)
            {
                if (spriteForDamage)
                    limbSprites[i].sprite = oldSprites[index++];
                else limbSprites[i].color = limbColors[i].Color;
            }

            hurt = false;

            rb.sharedMaterial.bounciness = 0f;
            circleCollider.enabled = false;
            circleCollider.enabled = true;
        }
    }

    public void StopDamage()
    {
        StopDamage(false);
    }

    IEnumerator NormalizeColors()
    {
        float t = 0;
        int index = 0;
        while (t <= 1)
        {
            if (PublicVariables.TimeScale > 0.05f)
            {
                t += PublicVariables.deltaTime;
                for (int i = 0; i < limbSprites.Count; i++)
                {
                    if (spriteForDamage)
                        limbSprites[i].sprite = oldSprites[index++];
                    else limbSprites[i].color = limbColors[i].Color;
                }
            }
            yield break;
        }
        yield return null;
    }

    public Animator GetAnimation
    {
        get
        {
            return animator;
        }
    }

    public void EliminatePlayer()
    {
        if (bot && SceneName == "TutorialScene")
        {
            if (TutorialScene.instance.activatedInventory == false)
            {
                bool ok = false;
                bool ok2 = false;
                foreach (player player in FindObjectsOfType<player>())
                {
                    if (player.ableToMove && player.bot && player.gameObject != myObj)
                        ok = true;

                    if (player.lvl == 2 && player.bot == false)
                        ok2 = true;
                }

                if (ok == false && ok2)
                {
                    TutorialScene.instance.movement.GetComponentInChildren<Tutorial>().Disable();
                    TutorialScene.instance.EnableInventoryIcon();
                    TutorialScene.instance.inventory.Begin();
                    TutorialScene.instance.activatedInventory = true;
                }
            }
        }
        if (online == false)
        {
            List<int> teams = new List<int>();
            if (inDungeon)
            {
                foreach (player player in cameraFollow.playerScripts)
                {
                    if (player != null)
                    {
                        if (teams.Contains(player.team) == false)
                            teams.Add(player.team);
                    }

                }
            }
            else
            {
                foreach (player player in FindObjectsOfType<player>())
                {
                    if (player != null)
                    {
                        if (teams.Contains(player.team) == false && player.gameObject != myObj && player.animState_death == false)
                            teams.Add(player.team);
                    }

                }
            }
            if (teams.Count == 1 && bot == false && inDungeon == false)
            {
                FindObjectOfType<MapSelect>().NextMap();
            }
            if (bot == false && finalBoss == false && inDungeon)
            {
                bool ok = false;
                if(SceneName == "Boss")
                {
                    foreach (player player in FindObjectsOfType<player>())
                    {
                        if (player != null)
                        {
                            if (player.bot == false && player.finalBoss == false && player.gameObject != myObj)
                            {
                                ok = true;
                            }
                        }
                    }
                }
                else foreach (player player in cameraFollow.playerScripts)
                {
                    if (player != null)
                    {
                        if (player.bot == false && player.finalBoss == false && player.gameObject != myObj)
                        {
                            ok = true;
                        }
                    }
                }
                if (ok == false)
                {
                    Destroy(dungeonData);
                    if (SceneName == "TutorialScene")
                    {
                        SceneManager.LoadScene("TutorialScene");
                        ServerNotification.instance.SendDungeonData("This guy literally died in the tutorial LOOOOOOOOOOOOOL XDDDDD AHAHHA +))))))))))))))))).");
                    }
                    else ServerNotification.instance.SendDungeonData("User got defeated.");

                    if (DungeonData.instance.killedMobs >= 10)
                    {
                        switch (caracterId)
                        {
                            case 1:
                                DungeonData.instance.AddToQuest(17, 1);
                                break;
                                case 2:
                                DungeonData.instance.AddToQuest(18, 1);
                                break;
                                case 3:
                                DungeonData.instance.AddToQuest(19, 1);
                                break;
                                case 4:
                                DungeonData.instance.AddToQuest(20, 1);
                                break;
                                case 5:
                                DungeonData.instance.AddToQuest(21, 1);
                                break;

                        }
                    }

                    FindObjectOfType<arrivePad>().GetComponent<Animator>().Play("death");
                }
                else
                {
                    foreach (CharacterSlot slot in inventorySpawn.inventory.slots)
                    {
                        if (slot.characterId == caracterId)
                        {
                            dungeonData.abil1Lvl[caracterId] = slot.ability1.GetComponent<SelectableItem>().lvl;
                            dungeonData.abil2Lvl[caracterId] = slot.ability2.GetComponent<SelectableItem>().lvl;
                            dungeonData.abil3Lvl[caracterId] = slot.ability3.GetComponent<SelectableItem>().lvl;
                            break;
                        }
                    }
                    dungeonData.xp[caracterId] = xp;
                    dungeonData.lvl[caracterId] = lvl;

                    dungeonData.statuses[caracterId].hp = hp;
                    dungeonData.statuses[caracterId].reg = reg;
                    dungeonData.statuses[caracterId].str = str;
                    dungeonData.statuses[caracterId].cdr = cdr;
                    dungeonData.statuses[caracterId].spd = spd;
                    dungeonData.statuses[caracterId].dex = dex;

                    dungeonData.statuses[caracterId].abilityPoints = abilityPoints;
                    dungeonData.statuses[caracterId].masterPoints = masterPoints;

                    dungeonData.statuses[caracterId].pointsLeft = pointsLeft;
                }
            }
        }
        if (inDungeon == false || bot || finalBoss)
            Destroy(myObj);
        else
        {
            for (int i = 0; i < LimbSprites.Count; i++)
            {
                LimbSprites[i].color = limbColors[i].Color;
            }
            circleCollider.enabled = true;
            for(int i=0;i<limbSprites.Count;i++)
            {
                SpriteRenderer limbSprite = limbSprites[i];
                if (limbs[i].name.Contains("body"))
                {
                    limbs[i].transform.eulerAngles = new Vector3(0, 0, 0);
                    limbSprite.color = new Color32(
                    (byte)(limbSprite.color.r * 255),
                    (byte)(limbSprite.color.g * 255),
                    (byte)(limbSprite.color.b * 255),
                    255
                    );
                }
            }
            foreach (GameObject hitbox in hitboxes)
            {
                SpriteRenderer hitboxSprite = hitbox.GetComponent<SpriteRenderer>();
                if (hitboxSprite != null)
                {
                    hitboxSprite.color = new Color32(
                      (byte)(hitboxSprite.color.r * 255),
                      (byte)(hitboxSprite.color.g * 255),
                      (byte)(hitboxSprite.color.b * 255),
                      255
                      );
                }
            }

            animator.Play("idle");
            this.enabled = true;
            animator.Play("idle");
            CancelInvoke("FireDamage");
            CancelInvoke("PoisonDamage");
            CancelInvoke("StopIce");
            CancelInvoke("StopStun");
            StopIce();
            StopStun();
            fireParticles.SetActive(false);
            poisonParticles.SetActive(false);
            FindObjectOfType<DeadMates>().players.Add(this);
            cameraFollow.players.Remove(myObj);
            cameraFollow.playerScripts.Remove(this);
            myObj.SetActive(false);
        }

        if (bot && SceneName != "TutorialScene" && SceneName != "Shops")
        {
            DungeonData.instance.killedMobs++;
            DungeonData.instance.AddToQuest(3, 1);
        }
    }
    bool canAttack2 = false;
    public int team;

    PolygonCollider2D[] rooms;

    player myPlayer;

    public void GetRooms()
    {
        rooms = FindObjectsOfType<PolygonCollider2D>();
    }
    bool executedOnEnable = false;

    List<player> nonBotPlayers = new List<player>();

    void GetPlayerList()
    {
        nonBotPlayers = new List<player>();

        player[] players = FindObjectsOfType<player>();

        for(int i=0;i<players.Length;i++)
        {
            player player = players[i];
            if (player.bot == false && player.boss == false && player.finalBoss == false)
            {
                nonBotPlayers.Add(player);
            }
        }
    }

    void GetTarget()
    {
        CancelInvoke("GetTarget");
        player player = null;
        float dist = 9999;
        bool resetPlayerList = false;
        for(int i=0;i<nonBotPlayers.Count;i++)
        {
            player nonBot = nonBotPlayers[i];
            if (nonBot != null)
            {
                if (nonBot.gameObject.activeInHierarchy == false)
                    resetPlayerList = true;
                else
                {
                    float playerDist = Vector2.Distance(playerTransform.position, nonBot.transform.position);

                    if (playerDist < dist)
                    {
                        player = nonBot;
                        dist = playerDist;
                    }
                }
            }
            else resetPlayerList = true;
        }
        if (resetPlayerList)
            GetPlayerList();

        if (player != null)
        {
            target = player.gameObject;
            if (hideSpot == null)
                aiDestinationSetter.target = target.transform;
            else if (aiDestinationSetter.target != hideSpot.transform)
                aiDestinationSetter.target = target.transform;
        }
        Invoke("GetTarget", 1f);

    }

    public void GetSlots(Inventory inventory)
    {
        if (bot == false && inDungeon)
        {
            try
            {
                this.inventory = inventory;
                foreach (CharacterSlot slot in inventory.slots)
                {
                    if (slot.characterId == caracterId)
                    {
                        abilSlot1 = slot.ability1.GetComponent<SelectableItem>();
                        abilSlot2 = slot.ability2.GetComponent<SelectableItem>();
                        abilSlot3 = slot.ability3.GetComponent<SelectableItem>();
                        break;
                    }
                }
            }
            catch
            {

            }
        }
    }
    List<hitbox> hitboxScripts = new List<hitbox>();
    bool rendered = false;
    // Start is called before the first frame update

    bool calledTargetOnStart = false;

    private void OnWillRenderObject()
    {
        if (calledTargetOnStart == false)
        {
            CancelInvoke("GetTarget");
            GetTarget();
            calledTargetOnStart = true;
        }

        if (MathUtils.CompareDistances(playerTransform.position, target.transform.position, 20, MathUtils.CompareTypes.LessThan))
        {
            if (TutorialScene.instance != null && bot)
            {
                if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                {
                    Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                    tutorial.Disable();
                }
            }
            ableToMove = true;
            rendered = true;
            if (bot)
            {
                aiPath.enabled = true;
                aiDestinationSetter.enabled = true;
            }
            animator.enabled = true;
            GetComponent<CharacterSortingOrder>().enabled = true;
        }
    }

    public float difficultyPercent = 1;

    private void OnEnable()
    {
        bossScene = SceneName == "Boss";
        newDex = 0.1f;
        if (executedOnEnable && isBoss)
        {
            CancelInvoke("FireParticles");
            fireParticles.SetActive(false);
            CancelInvoke("PoisonParticles");
            poisonParticles.SetActive(false);
            CancelInvoke("StopStun");
            StopStun();
            if (speedObjNotNull)
            {
                CancelInvoke("StopSpeed");
                StopSpeed();
            }
            CancelInvoke("StopSlow");
            StopSlow();
            if (mobAuraNotNull)
                mobAura.SetActive(false);
            if (silenceObjNotNull)
            {
                CancelInvoke("StopSilence");
                StopSilence();
            }
            if (weakenObjNotNull)
            {
                CancelInvoke("StopWeaken");
                StopWeaken();
            }

            CancelInvoke("StopDamage");

            if (stunned)
                rb.velocity = myVelocity = Vector2.zero;

            hurt = false;
            int index = 0;
            for(int i=0;i<limbSprites.Count;i++)
            {
                if (spriteForDamage)
                    limbSprites[i].sprite = oldSprites[index++];
                else limbSprites[i].color = limbs[i].GetComponent<OriginalColor>().Color;
            }

            rb.sharedMaterial.bounciness = 0f;
            circleCollider.enabled = false;
            circleCollider.enabled = true;
        }

        if (executedOnEnable == false)
        {
            mobType = MobType.None;
            rb.sharedMaterial = new PhysicsMaterial2D();
            if (bot && isBoss == false && finalBoss == false)
            {
                healthBarObj.SetActive(false);
                if (botInShops && bot)
                    healthTextObj.SetActive(false);
            }
        }
        else if (mobType != MobType.None)
        {
            mobAura.SetActive(true);

        }
        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        player[] players = FindObjectsOfType<player>();

        foreach (player player in players)
        {
            if (player.circleCollider != null)
                Physics2D.IgnoreCollision(circleCollider, player.circleCollider);
        }
        foreach (hitbox box in hitboxScripts)
        {
            box.ResetPlayers();
        }
        if ((boss && executedOnEnable == false) || boss == false)
        {


            rooms = FindObjectsOfType<PolygonCollider2D>();
            maxXp = 0;

            if (bot)
            {
                if (boss == false && finalBoss == false && botInShops == false)
                {
                    try
                    {
                        int dungeonLevel = DungeonData.instance.currentMap;

                        switch (dungeonLevel)
                        {
                            case 1:
                                mobType = MobType.None;
                                break;
                            case 2:
                                if (UnityEngine.Random.Range(1, 999999) % 20 == 0)
                                    mobType = (MobType)Enum.Parse(typeof(MobType), SceneName);
                                else mobType = MobType.None;
                                break;
                            case 3:
                                if (UnityEngine.Random.Range(1, 999999) % 15 == 0)
                                    mobType = (MobType)Enum.Parse(typeof(MobType), SceneName);
                                else mobType = MobType.None;
                                break;
                            case 4:
                                if (UnityEngine.Random.Range(1, 999999) % 10 == 0)
                                    mobType = (MobType)Enum.Parse(typeof(MobType), SceneName);
                                else mobType = MobType.None;
                                break;
                            case 5:
                                if (UnityEngine.Random.Range(1, 999999) % 5 == 0)
                                    mobType = (MobType)Enum.Parse(typeof(MobType), SceneName);
                                else mobType = MobType.None;
                                break;

                        }
                    }
                    catch
                    {
                        mobType = MobType.None;
                    }

                    if (mobType != MobType.None)
                    {
                        mobAura.SetActive(true);
                        switch (mobType)
                        {
                            case MobType.Fire:
                                {
                                    mobAura.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                                    ParticleSystem.MainModule main = mobAura.GetComponentInChildren<ParticleSystem>().main;
                                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 0, 0, 255));
                                    break;
                                }
                            case MobType.Ice:
                                {
                                    mobAura.GetComponent<SpriteRenderer>().color = new Color32(49, 97, 255, 255);
                                    ParticleSystem.MainModule main = mobAura.GetComponentInChildren<ParticleSystem>().main;
                                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(49, 97, 255, 255));
                                    break;
                                }
                            case MobType.Plains:
                                {
                                    mobAura.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
                                    ParticleSystem.MainModule main = mobAura.GetComponentInChildren<ParticleSystem>().main;
                                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(0, 255, 0, 255));
                                    break;
                                }
                            case MobType.Magic:
                                {
                                    mobAura.GetComponent<SpriteRenderer>().color = new Color32(204, 95, 255, 255);
                                    ParticleSystem.MainModule main = mobAura.GetComponentInChildren<ParticleSystem>().main;
                                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(204, 95, 255, 255));
                                    break;
                                }
                            case MobType.Stone:
                                {
                                    mobAura.GetComponent<SpriteRenderer>().color = new Color32(87, 87, 87, 255);
                                    ParticleSystem.MainModule main = mobAura.GetComponentInChildren<ParticleSystem>().main;
                                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(87, 87, 87, 87));
                                    break;
                                }
                        }
                    }
                }

                circleCollider.isTrigger = false;

                GetPlayerList();
                Invoke("GetTarget", 1f);

                for (int i = 0; i < childrenSprites.Length; i++)
                {
                    string name = childrenSprites[i].transform.name;

                    if (name != "green" && name != "red" && name != "black" && name != "block" && name != "block (1)" && name != "block (2)")
                        childrenSprites[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;


                }

                if (spriteForDamage)
                {
                    oldSprites = new Sprite[limbs.Length];
                    int index = 0;
                    for (int i = 0; i < limbSprites.Count; i++)
                    {
                        oldSprites[index++] = limbSprites[i].sprite;
                    }
                }

                level = OMEGA.Data.GetMobThreatLevel();

                float minAttack = 0;
                float maxAttack = 0;
                if (caracterId == 1)
                {


                    if (level == 1)
                    {
                        health = 250;
                        speed = 5;
                        attackSpeed = 1;
                        minAttack = 2.5f;
                        maxAttack = 3.5f;
                        wizard.startStaffDamage = 25;
                        wizard.startFireballDamage = 25;
                        botAttacks = new string[1];
                        botAttacks[0] = "attack1";
                        canDash = false;
                        wizard.Start();
                    }
                    else if (level == 2)
                    {
                        health = 350;
                        speed = 5.5f;
                        attackSpeed = 1f;
                        minAttack = 2f;
                        maxAttack = 2.5f;
                        wizard.startStaffDamage = 45;
                        wizard.startFireballDamage = 60;
                        botAttacks = new string[1];
                        botAttacks[0] = "attack1";
                        canDash = false;
                        wizard.Start();
                    }
                    else if (level == 3)
                    {
                        health = 450;
                        speed = 5.75f;
                        attackSpeed = 1f;
                        minAttack = 1.5f;
                        maxAttack = 2f;
                        wizard.startStaffDamage = 60;
                        wizard.startFireballDamage = 75;
                        wizard.startIceDamage = 50;
                        botAttacks = new string[2];
                        botAttacks[0] = "attack1";
                        botAttacks[1] = "attack5";
                        canDash = false;
                        wizard.Start();
                    }
                    else if (level == 4)
                    {
                        health = 500;
                        speed = 6.25f;
                        attackSpeed = 1f;
                        minAttack = 1f;
                        maxAttack = 1.5f;
                        wizard.startStaffDamage = 75;
                        wizard.startFireballDamage = 90;
                        wizard.startIceDamage = 65;
                        botAttacks = new string[2];
                        botAttacks[0] = "attack1";
                        botAttacks[1] = "attack5";
                        canDash = true;
                        dashName = "attack3";
                        wizard.Start();
                    }
                    else if (level == 5)
                    {
                        health = 600;
                        speed = 6.5f;
                        minAttack = 0.75f;
                        maxAttack = 1.25f;
                        attackSpeed = 1f;
                        wizard.startStaffDamage = 85;
                        wizard.startFireballDamage = 100;
                        wizard.startIceDamage = 80;
                        wizard.startElectricityDamage = 100;
                        botAttacks = new string[3];
                        botAttacks[0] = "attack1";
                        botAttacks[1] = "attack5";
                        botAttacks[2] = "attack8";
                        canDash = true;
                        dashName = "attack3";
                        wizard.Start();
                    }
                    if (isBoss)
                    {
                        health *= 5;

                        attackSpeed = 1f;

                        botAttacks = new string[7];
                        botAttacks[0] = "attack1";
                        botAttacks[1] = "attack4";
                        botAttacks[2] = "attack5";
                        botAttacks[3] = "attack6";
                        botAttacks[4] = "attack7";
                        botAttacks[5] = "attack8";
                        botAttacks[6] = "attack2";

                        botAttackMinDistances = new float[7];

                        botAttackMinDistances[0] = 50;
                        botAttackMinDistances[1] = 10;
                        botAttackMinDistances[2] = 30;
                        botAttackMinDistances[3] = 20;
                        botAttackMinDistances[4] = 20;
                        botAttackMinDistances[5] = 10;
                        botAttackMinDistances[6] = 5;

                        wizard.Start();
                    }
                }
                else if (caracterId == 2)
                {
                    if (finalBoss)
                    {
                        minAttack = 2;
                        maxAttack = 3;
                        attackSpeed = 1;
                        knight.startWeaponDamage = 35;
                        botAttacks = new string[1];
                        botAttacks[0] = "botBasicAttack";
                        distanceFromPlayer = 5f;

                        canDash = false;
                        knight.Start();
                    }
                    else
                    {
                        if (level == 0)
                        {
                            botSpawnTime = 0;
                            tBA = 100;
                            timeBetweenAttacks = 0;
                            health = 1;
                            speed = 7f;
                            minAttack = 0;
                            maxAttack = 0;
                            attackSpeed = 1;
                            knight.startWeaponDamage = 50;
                            distanceFromPlayer = 5f;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            timeTillDash = 0;
                            canDash = false;
                            canAttack = true;
                            knight.Start();
                        }
                        else if (level == 1)
                        {
                            health = 500;
                            speed = 7.5f;
                            minAttack = 2;
                            maxAttack = 3;
                            attackSpeed = 1;
                            knight.startWeaponDamage = 50;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            canDash = false;
                            knight.Start();
                        }
                        else if (level == 2)
                        {
                            health = 675;
                            speed = 8f;
                            minAttack = 1.5f;
                            maxAttack = 2.25f;
                            attackSpeed = 1f;
                            knight.startWeaponDamage = 65;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            canDash = false;
                            knight.Start();
                        }
                        else if (level == 3)
                        {
                            health = 800;
                            speed = 8.5f;
                            minAttack = 1.25f;
                            maxAttack = 1.75f;
                            attackSpeed = 1f;
                            knight.startWeaponDamage = 80;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            canDash = true;
                            dashName = "BotDash";
                            knight.Start();
                        }
                        else if (level == 4)
                        {
                            health = 925;
                            speed = 9f;
                            minAttack = 0.75f;
                            maxAttack = 1.25f;
                            attackSpeed = 1f;
                            knight.startWeaponDamage = 95;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            canDash = true;
                            dashName = "BotDash";
                            knight.Start();
                        }
                        else if (level == 5)
                        {
                            health = 1050;
                            speed = 10f;
                            minAttack = 0.25f;
                            maxAttack = 0.5f;
                            attackSpeed = 1;
                            knight.startWeaponDamage = 115;
                            botAttacks = new string[1];
                            botAttacks[0] = "botBasicAttack";
                            canDash = true;
                            dashName = "BotDash";
                            knight.Start();
                        }
                    }
                    if (isBoss)
                    {
                        health *= 4;
                        attackSpeed = 1;
                        botAttacks = new string[6];
                        botAttacks[0] = "attack2";
                        botAttacks[1] = "attack1";
                        botAttacks[2] = "attack3";
                        botAttacks[3] = "attack4";
                        botAttacks[4] = "attack5";
                        botAttacks[5] = "attack7";

                        botAttackMinDistances = new float[6];

                        botAttackMinDistances[0] = 7.5f;
                        botAttackMinDistances[1] = 4f;
                        botAttackMinDistances[2] = 12.5f;
                        botAttackMinDistances[3] = 10f;
                        botAttackMinDistances[4] = 30f;
                        botAttackMinDistances[5] = 2f;


                        distanceFromPlayer = 7.5f;
                        dashDistanceFromPlayer = 15f;
                        knight.Start();
                    }
                }
                else if (caracterId == 3)
                {

                    if (level == 1)
                    {
                        health = 300;
                        speed = 10f;
                        minAttack = 3f;
                        maxAttack = 4f;
                        attackSpeed = 1;
                        distanceFromPlayer = 5f;
                        dashDistanceFromPlayer = 2.5f;
                        archer.startArrowDamage = 35;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        archer.Start();
                    }
                    else if (level == 2)
                    {
                        health = 380;
                        speed = 11.2f;
                        minAttack = 2.25f;
                        maxAttack = 2.75f;
                        attackSpeed = 1f;
                        distanceFromPlayer = 6.2f;
                        dashDistanceFromPlayer = 3f;
                        archer.startArrowDamage = 55;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        archer.Start();
                    }
                    else if (level == 3)
                    {
                        health = 460;
                        speed = 12.4f;
                        minAttack = 1.5f;
                        maxAttack = 2f;
                        attackSpeed = 1f;
                        distanceFromPlayer = 7.4f;
                        dashDistanceFromPlayer = 3.5f;
                        archer.startArrowDamage = 75;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = true;
                        dashName = "attack4";
                        archer.Start();
                    }
                    else if (level == 4)
                    {
                        health = 540;
                        speed = 13.6f;
                        minAttack = 1f;
                        maxAttack = 1.5f;
                        attackSpeed = 1f;
                        distanceFromPlayer = 8.6f;
                        dashDistanceFromPlayer = 4f;
                        archer.startArrowDamage = 95;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = true;
                        dashName = "attack4";
                        archer.Start();
                    }
                    else if (level == 5)
                    {
                        health = 620;
                        speed = 15f;
                        minAttack = 0.5f;
                        maxAttack = 0.75f;
                        attackSpeed = 1f;
                        distanceFromPlayer = 10f;
                        dashDistanceFromPlayer = 5f;
                        archer.startArrowDamage = 110;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = true;
                        dashName = "attack4";
                        archer.Start();
                    }
                    if (isBoss)
                    {
                        health *= 6;
                        attackSpeed = 1f;
                        botAttacks = new string[7];
                        botAttacks[0] = "botAttack";
                        botAttacks[1] = "attack1";
                        botAttacks[2] = "attack2";
                        botAttacks[3] = "attack3";
                        botAttacks[4] = "attack5";
                        botAttacks[5] = "attack6";
                        botAttacks[6] = "attack7";

                        botAttackMinDistances = new float[7];
                        botAttackMinDistances[0] = 15;
                        botAttackMinDistances[1] = 15;
                        botAttackMinDistances[2] = 15;
                        botAttackMinDistances[3] = 8;
                        botAttackMinDistances[4] = 8;
                        botAttackMinDistances[5] = 8;
                        botAttackMinDistances[6] = 4;

                        archer.Start();
                    }
                }
                else if (caracterId == 4)
                {



                    if (level == 1)
                    {
                        health = 1000;
                        speed = 2f;
                        minAttack = 4f;
                        maxAttack = 5f;
                        attackSpeed = 1;
                        tank.startWeaponDamage = 90;
                        tank.startWeaponKnock = 5;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        tank.Start();
                    }
                    else if (level == 2)
                    {
                        health = 1250;
                        speed = 2.5f;
                        minAttack = 3.25f;
                        maxAttack = 4.25f;
                        attackSpeed = 1f;
                        tank.startWeaponDamage = 120;
                        tank.startWeaponKnock = 7;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        tank.Start();
                    }
                    else if (level == 3)
                    {
                        health = 1500;
                        speed = 3f;
                        minAttack = 2.5f;
                        maxAttack = 3.25f;
                        attackSpeed = 1f;
                        tank.startWeaponDamage = 150;
                        tank.startWeaponKnock = 9;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        tank.Start();
                    }
                    else if (level == 4)
                    {
                        health = 1750;
                        speed = 3.5f;
                        minAttack = 1.75f;
                        maxAttack = 2.25f;
                        attackSpeed = 1f;
                        tank.startWeaponDamage = 190;
                        tank.startWeaponKnock = 12;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        noKnockback = true;
                        tank.Start();
                    }
                    else if (level == 5)
                    {
                        health = 2000;
                        speed = 4f;
                        minAttack = 1f;
                        maxAttack = 1.5f;
                        attackSpeed = 1f;
                        tank.startWeaponDamage = 210;
                        tank.startWeaponKnock = 15;
                        botAttacks = new string[1];
                        botAttacks[0] = "botAttack";
                        canDash = false;
                        noKnockback = true;
                        tank.Start();
                    }
                    if (isBoss)
                    {
                        distanceFromPlayer = 7.5f;
                        dashDistanceFromPlayer = 15f;
                        health *= 3.5f;
                        attackSpeed = 1f;
                        botAttacks = new string[7];

                        botAttacks[0] = "attack2";
                        botAttacks[1] = "attack4";
                        botAttacks[2] = "attack5";
                        botAttacks[3] = "attack7";
                        botAttacks[4] = "botAttack";
                        botAttacks[5] = "attack1";
                        botAttacks[6] = "attack3";

                        botAttackMinDistances = new float[7];

                        botAttackMinDistances[0] = 40;
                        botAttackMinDistances[1] = 15;
                        botAttackMinDistances[2] = 15;
                        botAttackMinDistances[3] = 8f;
                        botAttackMinDistances[4] = 6f;
                        botAttackMinDistances[5] = 6f;
                        botAttackMinDistances[6] = 3f;


                        tank.Start();
                    }

                }
                else if (caracterId == 5)
                {

                    if (level == 1)
                    {
                        health = 400;
                        speed = 8f;
                        minAttack = 2.5f;
                        maxAttack = 3f;
                        attackSpeed = 1f;
                        supportClass.startWeaponDamage = 60;
                        supportClass.aoeBox.healAmmount = 100;
                        supportClass.poisonDamage = 100;
                        botAttacks = new string[0];
                        canCounterAttack = false;
                        canHeal = false;
                        canRevive = false;
                        supportClass.Start();
                    }
                    else if (level == 2)
                    {
                        health = 600;
                        speed = 9f;
                        minAttack = 2.5f;
                        maxAttack = 3f;
                        attackSpeed = 1f;
                        supportClass.startWeaponDamage = 70;
                        supportClass.aoeBox.healAmmount = 250;
                        supportClass.poisonDamage = 100;
                        botAttacks = new string[0];
                        canCounterAttack = false;
                        canHeal = true;
                        canRevive = false;
                        supportClass.Start();
                    }
                    else if (level == 3)
                    {
                        health = 800;
                        speed = 10;
                        minAttack = 1.75f;
                        maxAttack = 2.25f;
                        attackSpeed = 1f;
                        supportClass.startWeaponDamage = 80;
                        supportClass.aoeBox.healAmmount = 400;
                        supportClass.poisonDamage = 150;
                        botAttacks = new string[0];
                        canCounterAttack = false;
                        canHeal = true;
                        canRevive = false;
                        supportClass.Start();
                    }
                    else if (level == 4)
                    {
                        health = 1000;
                        speed = 11;
                        minAttack = 1.25f;
                        maxAttack = 1.5f;
                        attackSpeed = 1f;
                        supportClass.startWeaponDamage = 90;
                        supportClass.aoeBox.healAmmount = 550;
                        supportClass.poisonDamage = 150;
                        botAttacks = new string[0];
                        canCounterAttack = true;
                        canHeal = true;
                        canRevive = true;
                        supportClass.Start();
                    }
                    else if (level == 5)
                    {
                        health = 1200;
                        speed = 12;
                        minAttack = 0.5f;
                        maxAttack = 0.75f;
                        attackSpeed = 1f;
                        supportClass.startWeaponDamage = 100;
                        supportClass.aoeBox.healAmmount = 700;
                        supportClass.poisonDamage = 300;
                        botAttacks = new string[0];
                        canCounterAttack = true;
                        canHeal = true;
                        canRevive = true;
                        supportClass.Start();
                    }
                    if (isBoss)
                    {
                        health *= 4;
                        botAttacks = new string[7];
                        botAttacks[0] = "attack1";
                        botAttacks[1] = "attack2";
                        botAttacks[2] = "attack4";
                        botAttacks[3] = "attack5";
                        botAttacks[4] = "attack8";
                        botAttacks[5] = "attack6";
                        botAttacks[6] = "attack7";


                        botAttackMinDistances = new float[7];
                        botAttackMinDistances[0] = 15;
                        botAttackMinDistances[1] = 15;
                        botAttackMinDistances[2] = 5;
                        botAttackMinDistances[3] = 30f;
                        botAttackMinDistances[4] = 30;
                        botAttackMinDistances[5] = 5;
                        botAttackMinDistances[6] = 5;


                        canCounterAttack = true;
                        canHeal = true;
                        canRevive = true;
                        supportClass.Start();
                    }
                }

                if (mobType != MobType.None)
                {
                    health *= 1.5f;
                    minAttack = 2f;
                    maxAttack = 2f;
                }

                xpDrop = 10 + UnityEngine.Random.Range(-5, 5);
                if (boss)
                    xpDrop *= 5;
                if (SceneName == "TutorialScene")
                    xpDrop *= 5;

                switch (PlayerPrefs.GetInt("Difficulty", 1))
                {
                    case 0:
                        difficultyPercent = 0.33f;
                        break;
                    case 1:
                        difficultyPercent = 0.66f;
                        break;
                    case 2:
                        difficultyPercent = 1;
                        break;
                    case 3:
                        difficultyPercent = 1.5f;
                        break;
                    case 4:
                        difficultyPercent = 2;
                        break;

                }

                health *= difficultyPercent;

                float num = ((float)dungeonData.playerCount) / 5f;

                if (finalBoss)
                {
                    health *= (0.4f + num * 3);

                    maxHealthTillSpawnMobs = (int)(health / 4) * (0.4f + num * 3);
                }
                else
                {
                    ableToMove = true;

                    if (boss)
                    {
                        num *= 4;
                        num *= 1 + ((float)dungeonData.currentMap - 1) / 5f;
                        speed *= 2;
                        distanceFromPlayer *= 1.5f;
                        minAttack = 0.5f;
                        maxAttack = 1.5f;

                        for (int i = 0; i < botAttacks.Length - 1; i++)
                        {
                            for (int j = i + 1; j < botAttacks.Length; j++)
                            {
                                if (botAttackMinDistances[i] > botAttackMinDistances[j])
                                {
                                    float aux1 = botAttackMinDistances[i];
                                    botAttackMinDistances[i] = botAttackMinDistances[j];
                                    botAttackMinDistances[j] = aux1;

                                    string aux2 = botAttacks[i];
                                    botAttacks[i] = botAttacks[j];
                                    botAttacks[j] = aux2;
                                }
                            }
                        }
                    }

                    float spd = 0;

                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].speed > spd)
                            spd = players[i].speed;
                    }

                    originalSpeed = spd * 2.5f;

                    if (bossScene == false)
                        health *= (1.2f + num);
                    else
                    {
                    }

                    if (boss)
                    {
                        noKnockback = true;
                    }

                    maxHealthTillSpawnMobs = health / 5;
                }

                healthTillSpawnMobs = maxHealthTillSpawnMobs + 100;

                timeBetweenAttacks = UnityEngine.Random.Range(minAttack, maxAttack);

                tBA = timeBetweenAttacks;

                if (noAI)
                    health = 1500;

                if (isBoss && SceneName == "Boss" && setBossHealth == false)
                {
                    
                    float num2 = ((float)dungeonData.playerCount) / 5f;

                    health = 25000;

                    health *= (num2 * 2 + 0.6f);

                    health *= difficultyPercent;
                }

                maxHealth = health;

                hpBarTransform.localScale = new Vector2(1, 1);
                if (botInShops && bot)
                    healthText.text = ((int)health).ToString();

                originalSpeed = speed;
                if (parkourRoom)
                {

                    originalSpeed = 7.5f;

                }
                for(int i=0;i<limbSprites.Count;i++)
                {
                    
                    if (wizard != null && limbs[i].name.Contains("palm") == false && limbs[i].name.Contains("face") == false)
                    {

                    }
                    else limbSprites[i].color = new Color32(255, 255, 255, 255);
                    limbs[i].GetComponent<OriginalColor>().Start();
                }
                for(int i=0;i<childrenSprites.Length;i++)
                {
                    SpriteRenderer sprite = childrenSprites[i];
                    if (sprite.sprite.name != "block")
                        sprite.color = new Color32((byte)(sprite.color.r * 255), (byte)(sprite.color.g * 255), (byte)(sprite.color.b * 255), 0);
                }

                OMEGA.Events.OnSpawnMob(this);
            }
            else if (finalBoss)
            {
                int num = 0;
                foreach (player player in players)
                {
                    if (player.bot == false && player.finalBoss == false)
                    {
                        num++;
                    }
                }
            }
            executedOnEnable = true;

            if (botInShops && bot)
                healthText.text = ((int)health).ToString();
        }
        if (noAI)
        {
            t = 1;
            foreach (SpriteRenderer sprite in childrenSprites)
            {
                sprite.color = new Color32(255, 255, 255, 255);
            }
        }
        if (bossScene)
            timeBetweenAttacks = 0.5f;
        if (SceneName == "TutorialScene")
            ableToMove = false;
    }
    public bool canHeal = false;
    public bool canCounterAttack = false;
    public bool canRevive = false;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer firstLimb;
    BoxCollider2D cursorBoxCollider;
    public support supportClass;
    public List<SpriteRenderer> LimbSprites
    {
        get
        {
            return limbSprites;
        }
        set
        {
            limbSprites = value;
        }
    }

    public void SetSecondPhaseBossHealth(float health)
    {
        int num = 0;
        foreach (player player in FindObjectsOfType<player>())
        {
            if (player.bot == false && player.finalBoss == false)
            {
                num++;
            }
        }
        this.health = health;
        this.health *= 1 + num;
        maxHealth = this.health;
    }


    public List<SpriteRenderer> limbSprites;

    public List<OriginalColor> limbColors;
    public CircleCollider2D circleCollider;
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetter;
    Inventory inventory;
    DungeonData dungeonData;
    InventorySpawn inventorySpawn;
    hitbox weaponHitbox;
    PlayerData playerData;
    Cursor cursorObject;
    ControllerReconnect controllerReconnect;
    public ItemShop itemShop;
    public itemMenu[] itemMenus;
    public abilityShop abilityShop;
    public abilityShop[] abilityShops;
    public Shop shop;
    public Shop[] shops;

    public CameraFollow cameraFollow;

    public PlayerData myPlayerData;

    Tank tankClass;
    bool setBossHealth = false;
    public void SetBossHp(float health)
    {
        this.health = health;

        setBossHealth = true;

        if (health > maxHealth)
            this.health = maxHealth;

        try
        {
            hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
            if (botInShops && bot)
                healthText.text = ((int)health).ToString();
        }
        catch
        {

        }
    }
    public void SetHp(float health)
    {
        float hpAmount = health;

        if ((bossScene && isBoss) == false)
        {
            if (bot == false && this.inDungeon)
            {
                for(int i=0;i<inventorySpawn.itemSlots.Length;i++)
                {
                    SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                    if (itemSlot.containsItem)
                    {
                        if (itemSlot.itemType == 3)
                        {
                            health += 0.2f * hpAmount;
                        }
                    }
                }
            }
            health += hpAmount * (float)reg / 10f;
        }
        this.health = health;

        if (health > maxHealth)
            this.health = maxHealth;

        try
        {
            hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
            if (botInShops && bot)
                healthText.text = ((int)health).ToString();
        }
        catch
        {

        }


    }

    public void SetHpPercent(float health)
    {
        float hpAmount = health;

        this.health = maxHealth * hpAmount;

        if (this.health > maxHealth)
            this.health = maxHealth;

        try
        {
            hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
            if (botInShops && bot)
                healthText.text = ((int)health).ToString();
        }
        catch
        {

        }

    }

    SpriteRenderer[] childrenSprites;
    BlindnessObj blindness;

    public wizard wizard;
    public knight knight;
    public archer archer;
    public Tank tank;
    GameObject myObj;
    Transform hpBarTransform;
    ParticleSystem healthObjVfx;
    ParticleSystem shieldBreakVfx;
    GameObject healthTextObj;
    bool enemyBarActive;
    bool barFadeActive;
    LockRoom bossRoomScript;
    GameObject hpBarObj;
    bool speedObjNotNull;
    bool mobAuraNotNull;
    bool weaponHitboxNotNull;
    bool isInShops;
    bool silenceObjNotNull;
    bool weakenObjNotNull;
    bool wizardNotNull;
    bool knightNotNull;
    bool tankNotNull;
    bool supportNotNull;
    bool fireParticlesNotNull;
    bool statsNotNull;
    bool healthTextNotNull;
    List<Transform> abilCooldownImages = new List<Transform>();
    List<SpriteRenderer> abilCooldownColors = new List<SpriteRenderer>();
    public void Awake()
    {
        if (PlayerPrefs.GetInt("ToggleMovement", 1) == 1)
            movementToggle = true;
        else movementToggle = false;

        fireParticlesNotNull = fireParticles != null;
        statsNotNull = Stats != null;
        healthTextNotNull = healthText != null;
        hpBarObj = hpBar.gameObject;
        if(bossRoom != null)
        bossRoomScript = bossRoom.GetComponent<LockRoom>();
        barFadeActive = PlayerPrefs.GetInt("BarFade", 1) == 0;
        enemyBarActive = PlayerPrefs.GetInt("EnemyBar", 1) == 1;
        if(healthText != null)
        healthTextObj = healthText.gameObject;
        myObj = gameObject;
        playerTransform = transform;
        SceneName = SceneManager.GetActiveScene().name;

        if (SceneName != "TutorialScene")
            rendered = true;

        wizard = GetComponent<wizard>();
        knight = GetComponent<knight>();
        archer = GetComponent<archer>();
        tank = GetComponent<Tank>();

        hpBarTransform = hpBar.transform;
        healthObjVfx = healthObj.GetComponent<ParticleSystem>();
        shieldBreakVfx = shieldBreak.GetComponent<ParticleSystem>();

        baseHealth = health;

        if (caracterId == 1)
            originalSpeed = 5.5f;
        else if (caracterId == 2)
            originalSpeed = 8f;
        else if (caracterId == 3)
            originalSpeed = 10f;
        else if (caracterId == 4)
            originalSpeed = 3.5f;
        else if (caracterId == 5)
            originalSpeed = 9f;

        baseSpeed = originalSpeed;
        /*if (SceneName.Contains("SampleScene"))
        {
            switch (caracterId)
            {
                case 1:
                    health = 1500;
                    break;
                case 2:
                    health = 2000;
                    break;
                case 3:
                    health = 1000;
                    break;
                case 4:
                    health = 3500;
                    break;
                case 5:
                    health = 1250;
                    break;
            }
        }*/
        for(int i=0;i<hitboxes.Length;i++)
        {
            hitbox hitbox = hitboxes[i].GetComponent<hitbox>();
            if (hitbox != null)
                hitboxScripts.Add(hitbox);
        }
        if (inDungeon == false && bot == false && finalBoss == false)
        {
            name.gameObject.SetActive(true);

            for(int i=0;i<hitboxes.Length;i++)
            {
                hitboxes[i].layer = LayerMask.NameToLayer("AllSolids");
            }

            MakeChildrenUnlit(playerTransform);
        }
        ParticleSystem.MainModule fireModule = fireParticles.GetComponent<ParticleSystem>().main;
        fireModule.playOnAwake = true;
        ParticleSystem.MainModule poisonModule = poisonParticles.GetComponent<ParticleSystem>().main;
        poisonModule.playOnAwake = true;
        myPlayer = GetComponent<player>();
        LimbSprites = new List<SpriteRenderer>();
        limbColors = new List<OriginalColor>();
        for(int i=0;i<limbs.Length;i++)
        {
            LimbSprites.Add(limbs[i].GetComponent<SpriteRenderer>());
            limbColors.Add(limbs[i].GetComponent<OriginalColor>());
        }

#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        PlayerData[] playerDatas = FindObjectsOfType<PlayerData>();

        for(int i=0;i<playerDatas.Length;i++)
        {
            PlayerData data = playerDatas[i];
            if (data.playerNumber == playerNumber)
                myPlayerData = data;
        }
        try
        {
            blindness = FindObjectOfType<BlindnessObj>();
        }
        catch
        {

        }
        try
        {
            inventory = FindObjectOfType<Inventory>();
        }
        catch
        {

        }
        try
        {
            childrenSprites = GetComponentsInChildren<SpriteRenderer>();
        }
        catch
        {

        }
        try
        {
            tankClass = tank;
        }
        catch
        {

        }
        try
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
        catch
        {

        }
        try
        {
            controllerReconnect = FindObjectOfType<ControllerReconnect>();
        }
        catch
        {

        }
        try
        {
            cursorObject = FindObjectOfType<Cursor>();
        }
        catch
        {

        }
        try
        {
            playerData = FindObjectOfType<PlayerData>();
        }
        catch
        {

        }
        try
        {
            weaponHitbox = weapon.GetComponent<hitbox>();
        }
        catch
        {

        }
        try
        {
            inventorySpawn = FindObjectOfType<InventorySpawn>();
        }
        catch
        {

        }
        try
        {
            dungeonData = FindObjectOfType<DungeonData>();
        }
        catch
        {

        }
        try
        {
            supportClass = GetComponent<support>();
        }
        catch
        {

        }
        try
        {
            rb = GetComponent<Rigidbody2D>();
        }
        catch
        {

        }
        try
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
        catch
        {

        }
        try
        {
            aiPath = GetComponent<AIPath>();
            aiPath.enabled = false;
        }
        catch
        {

        }
        try
        {
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            aiDestinationSetter.enabled = false;
        }
        catch
        {

        }
        try
        {
            animator = GetComponent<Animator>();
            if (bot && SceneName == "TutorialScene")
            {
                animator.enabled = false;
                GetComponent<CharacterSortingOrder>().enabled = false;
            }
            else if(bot)
            {
                ableToMove = true;
                rendered = true;
                aiPath.enabled = true;
                aiDestinationSetter.enabled = true;
                animator.enabled = true;
                GetComponent<CharacterSortingOrder>().enabled = true;
            }
        }
        catch
        {

        }
        try
        {
            firstLimb = limbs[0].GetComponent<SpriteRenderer>();
        }
        catch
        {

        }
        if (inDungeon && bot == false && finalBoss == false)
        {
            bool ok = false;
            for(int i=0;i<playerDatas.Length;i++)
            {
                PlayerData data = playerDatas[i];
                if (data.idCaracter == caracterId && data.enabled)
                {
                    playerNumber = data.playerNumber;
                    myPlayerData = data;
                    ok = true;
                }
            }
            if (ok == false)
            {
                Destroy(myObj);
                return;
            }
        }
        if (playerNumber == 5)
        {
            keyboard = true;
            buttons = new int[8] { 16, 30, 31, 29, 15, 16, 17, 32 };
        }

        if (finalBoss && ableToMove)
            noKnockback = true;
        onlineOldPos = onlineNewPos = new Vector2(-1024, -1024);
        /*if (FindObjectOfType<GameClient>() != null)
        {
            for (int i = 0; i < abilityCooldowns.Length; i++)
            {
                abilityCooldowns[i] = 5f;
            }
        }*/

        player[] players = FindObjectsOfType<player>();

        if (inDungeon == false)
        {
            for(int i=0;i<childrenSprites.Length;i++)
            {
                childrenSprites[i].maskInteraction = SpriteMaskInteraction.None;
            }
        }
        else if (bot == false && cameraFollow != null)
            cameraFollow.players.Add(myObj);
        if (online == false && bot == false)
        {
            for(int i=0;i<abilImages.Length;i++)
            {
                abilCooldownImages.Add(abilImages[i].transform.GetChild(0));
                abilCooldownImages[i].localScale = new Vector2(0, 1);
                if (onPhone)
                {
                    switch (i)
                    {
                        case 0:
                            abillity1.GetSizer();
                            abillity1.sizer.localScale = new Vector2(0, 1);
                            abillity1.cooldownImage.color = new Color32(0, 0, 0, 128);
                            break;
                        case 1:
                            abillity2.GetSizer();
                            abillity2.sizer.localScale = new Vector2(0, 1);
                            abillity2.cooldownImage.color = new Color32(0, 0, 0, 128);
                            break;
                        case 2:
                            abillity3.GetSizer();
                            abillity3.sizer.localScale = new Vector2(0, 1);
                            abillity3.cooldownImage.color = new Color32(0, 0, 0, 128);
                            break;
                    }
                }
                abilCooldownColors.Add(abilCooldownImages[i].GetComponentInChildren<SpriteRenderer>());
            }
            bool ok = false;
            for(int i=0;i<playerDatas.Length;i++)
            {
                PlayerData data = playerDatas[i];

                if ((data.playerNumber == playerNumber || inDungeon) && data.idCaracter == caracterId && data.enabled)
                {
                    if (inDungeon == false)
                    {
                        attacks[0] = "attack" + data.abilitate1.ToString();
                        attacks[1] = "attack" + data.abilitate2.ToString();
                        attacks[2] = "attack" + data.abilitate3.ToString();
                        name.text = data.playerName;
                        team = data.echipa;
                    }
                    ok = true;
                }
            }
            if (ok == false && SceneName != "Boss" && SceneName != "Magic" && SceneName != "Ice" && SceneName != "Plains" && SceneName != "Stone" && !isInShops && SceneName != "Fire")
            {
                Destroy(myObj);
            }
            else
            {
                if (SceneName == "Magic" || SceneName == "Ice" || SceneName == "Plains" || SceneName == "Stone" || SceneName == "Fire")
                    ableToMove = false;
                maxHealth = health;
                originalSpeed = speed;
                for(int i=0;i<limbs.Length;i++)
                {
                        limbs[i].GetComponent<OriginalColor>().Start();
                }
                //for(int i=0;i<players.Length;i++)
                //    Physics2D.IgnoreCollision(circleCollider, players[i].circleCollider);
                if (bot == false)
                {
                    if (playerNumber == 5)
                    {
                        cursorTarget = new GameObject();
                        Target cursorScript = cursorTarget.AddComponent<Target>();
                        cursorBoxCollider = cursorTarget.AddComponent<BoxCollider2D>();
                        cursorBoxCollider.isTrigger = true;
                        cursorBoxCollider.size = new Vector2(0.25f, 0.25f);
                        cursorScript.playerNumber = playerNumber;

                        /*controllers = new List<SharpDX.DirectInput.Joystick>();
                        var joystickGuid = Guid.Empty;
                        var di = new DirectInput();
                        IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);
                        for (int device = 0; device < keyboards.Count; device++)
                        {
                            joystickGuid = keyboards[device].InstanceGuid;
                            controllers.Add(new Joystick(di, joystickGuid));
                        }
                        
                        joystick = controllers[0];
                        secondaryJoystick = joystick;
                        secondaryJoystick.Acquire();
                        joystick.Acquire();*/
                    }
                    else
                    {
                        //controllers = new List<SharpDX.DirectInput.Joystick>();
                        /*var joystickGuid = Guid.Empty;
                        var di = new DirectInput();
                        IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
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
                        joystick = controllers[playerNumber - 1];
                        secondaryJoystick = joystick;
                        secondaryJoystick.Acquire();
                        joystick.Acquire();*/
                    }
                }
            }
        }
        else if (online && bot == false)
        {
            name.gameObject.SetActive(true);
            maxHealth = health;
            originalSpeed = speed;
            for(int i=0;i<limbSprites.Count;i++)
            {
                if (wizard != null && limbs[i].name.Contains("palm") == false && limbs[i].name.Contains("face") == false)
                    limbSprites[i].color = new Color32(0, 0, 255, 255);
                else limbSprites[i].color = new Color32(255, 255, 255, 255);
                limbs[i].GetComponent<OriginalColor>().Color = new Color32(255, 255, 255, 255);
            }
            //for(int i=0;i<players.Length;i++)
            //    Physics2D.IgnoreCollision(circleCollider, players[i].circleCollider);
            //controllers = new List<SharpDX.DirectInput.Joystick>();
            /*var joystickGuid = Guid.Empty;
            var di = new DirectInput();
            IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);*/

            /*for (int device = 0; device < keyboards.Count; device++)
            {
                joystickGuid = keyboards[device].InstanceGuid;
                secondaryJoystick = new Joystick(di, joystickGuid);
                secondaryJoystick.Acquire();
                break;
            }*/
            buttons = new int[8] { 16, 30, 31, 29, 15, 16, 17, 32 };
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
            if (controllers.Count != 0)
            {
                joystick = controllers[0];
                joystick.Acquire();
            }
            else
            {
                joystick = null;
            }*/
        }

        speedObjNotNull = speedObj != null;
        mobAuraNotNull = mobAura != null;
        weaponHitboxNotNull = weaponHitbox != null;
        isInShops = SceneName.Contains("Shops");
        silenceObjNotNull = silenceObj != null;
        weakenObjNotNull = weakenObj != null;

        wizardNotNull = wizard != null;
        knightNotNull = knight != null;
        tankNotNull = tank != null;
        supportNotNull = supportClass != null;

        if (bot == false)
        {
            ResetAbilities();
        }
        if (finalBoss || myObj.name.Contains("FinalBoss"))
        {
            for(int i=0;i<limbs.Length;i++)
            {
                 limbs[i].GetComponent<OriginalColor>().Color = new Color32(255, 255, 255, 255);
            }
        }

        if (bot == false)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            keyboard = false;
            onPhone = true;
#endif
        }

        if (keyboard == false && PlayerPrefs.GetInt("HelperLine", 1) == 0 && isBoss == false && finalBoss == false && bot == false)
            helperLine.SetActive(true);
    }

    bool onPhone = false;

    public bool keyboard = false;

    float[] imageSizes = new float[3];

    public void ResetAbilities()
    {
        if (inDungeon)
        {
            attacks[0] = "attack" + PlayerPrefs.GetInt("SavedAbility" + caracterId + "1", 1);
            attacks[1] = "attack" + PlayerPrefs.GetInt("SavedAbility" + caracterId + "2", 2);
            attacks[2] = "attack" + PlayerPrefs.GetInt("SavedAbility" + caracterId + "3", 3);
        }

        if (inDungeon)
        {
            currentCooldowns = new float[3];

            for (int i = 0; i <= 2; i++)
            {
                if ((attacks[i] == "attack3" && supportClass != null) || (attacks[i] == "attack6" && tank != null))
                    onCooldown[i] = false;
            }
        }
        else
        {
            currentCooldowns = new float[3] { 5, 5, 5 };
            abilityCooldowns = new float[8] { 5, 5, 5, 5, 5, 5, 5, 5 };
        }
        cooldownReduce = new float[3];

        for (int i = 0; i <= 2; i++)
            abilCooldownImages[i].transform.parent = null;

        abilImages[0].transform.localScale = new Vector3(abilImages[0].transform.localScale.x * abilImages[0].sprite.bounds.size.x / abilityImages[int.Parse(attacks[0].Split('k')[1]) - 1].bounds.size.x, abilImages[0].transform.localScale.y * abilImages[0].sprite.bounds.size.y / abilityImages[int.Parse(attacks[0].Split('k')[1]) - 1].bounds.size.y, 1);
       
        abilImages[0].sprite = abilityImages[int.Parse(attacks[0].Split('k')[1]) - 1];
        if (inDungeon)
            currentCooldowns[0] = abilityCooldowns[int.Parse(attacks[0].Split('k')[1]) - 1];
        cooldownReduce[0] = cooldownReductions[int.Parse(attacks[0].Split('k')[1]) - 1];
        abilImages[1].transform.localScale = new Vector3(abilImages[1].transform.localScale.x * abilImages[1].sprite.bounds.size.x / abilityImages[int.Parse(attacks[1].Split('k')[1]) - 1].bounds.size.x, abilImages[1].transform.localScale.y * abilImages[1].sprite.bounds.size.y / abilityImages[int.Parse(attacks[1].Split('k')[1]) - 1].bounds.size.y, 1);
       
        abilImages[1].sprite = abilityImages[int.Parse(attacks[1].Split('k')[1]) - 1];
        if (inDungeon)
            currentCooldowns[1] = abilityCooldowns[int.Parse(attacks[1].Split('k')[1]) - 1];
        cooldownReduce[1] = cooldownReductions[int.Parse(attacks[1].Split('k')[1]) - 1];
        abilImages[2].transform.localScale = new Vector3(abilImages[2].transform.localScale.x * abilImages[2].sprite.bounds.size.x / abilityImages[int.Parse(attacks[2].Split('k')[1]) - 1].bounds.size.x, abilImages[2].transform.localScale.y * abilImages[2].sprite.bounds.size.y / abilityImages[int.Parse(attacks[2].Split('k')[1]) - 1].bounds.size.y, 1);
       
        abilImages[2].sprite = abilityImages[int.Parse(attacks[2].Split('k')[1]) - 1];

        for (int i = 0; i <= 2; i++)
        {
            abilCooldownImages[i].transform.parent = abilImages[i].transform;
            imageSizes[i] = abilCooldownImages[i].transform.localScale.y;
        }

        if (inDungeon)
            currentCooldowns[2] = abilityCooldowns[int.Parse(attacks[2].Split('k')[1]) - 1];
        cooldownReduce[2] = cooldownReductions[int.Parse(attacks[2].Split('k')[1]) - 1];
        if (isInShops)
        {
            cooldownReduction = 100;
        }
    }

    public void Kinematic()
    {
        rb.velocity = myVelocity = new Vector2(0, 0);
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    public void Revive()
    {

        StopAttack(false);
        circleCollider.enabled = true;
        Stats.SetActive(true);
        health = maxHealth / 2;
        StopDamage();
        try
        {
            hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
            if (botInShops && bot)
                healthText.text = ((int)health).ToString();
        }
        catch
        {

        }
        revived = true;
        if (bot == false)
            animator.Play("idle");
    }
    public void Attacking()
    {
        followTarget = false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isNotHurt = (stateInfo.IsName("hurt") == false && stateInfo.IsName("hurt2") == false);

        if (isNotHurt)
        {
            if (online)
                animator.speed = 1;
            else animator.speed = attackSpeed;
        }
        if (speedObjNotNull)
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else if (speedObjActive)
                speed = bonusSpeedValue;
            else speed = originalSpeed;
        }
        else
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else speed = originalSpeed;
        }
        canAttack = false;
        canAttackButWithMovement = false;
        for(int i=0;i<hitboxScripts.Count;i++)
        {
            hitboxScripts[i].ResetPlayers(true);
        }
        if (isNotHurt)
        {
            rb.velocity = myVelocity = Vector2.zero;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (bot == false && isBoss == false && finalBoss == false)
                myObj.layer = 8;
            else myObj.layer = 16;
            if (bot)
            {
                //if (tBA == timeBetweenAttacks)
                //    tBA -= 0.5f;
                circleCollider.enabled = true;
                if (aiPath != null)
                    aiPath.canMove = false;
            }
        }
        if (caracterId == 5 && isBoss && stateInfo.IsName("attack4"))
        {
            animator.speed = 0.25f;
        }
        if (((caracterId == 1 && stateInfo.IsName("attack6")) || (caracterId == 5 && stateInfo.IsName("attack2"))) && isBoss)
        {
            animator.speed = 0.75f;
        }
    }
    void AttackingWithMovement()
    {
        followTarget = false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if ((stateInfo.IsName("hurt") == false && stateInfo.IsName("hurt2") == false))
        {

        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (bot == false && isBoss == false && finalBoss == false)
                myObj.layer = 8;
            else myObj.layer = 16;
        }
        canAttack = true;
        canAttackButWithMovement = true;
        for(int i=0;i<hitboxScripts.Count;i++)
        {
            hitboxScripts[i].ResetPlayers();
        }
    }
    public bool canAttackButWithMovement = false;
    public void StopSpam()
    {
        spamDirection = false;
    }
    void StopAttack4()
    {
        currentAttack = 0;
    }
    void StopAttack()
    {
        if (damaged == false)
        {
            animator.speed = 1;
            canAttack = true;
            canAttackButWithMovement = false;
            networkCanAttack = true;
            detectAttack = false;
            networkDetect = false;
            hurt = false;
            combo = 1;
            for (int i = 0; i < hitboxScripts.Count; i++)
            {
                hitboxScripts[i].ResetPlayers();
            }
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (bot == false && isBoss == false && finalBoss == false)
                myObj.layer = 8;
            else myObj.layer = 16;


                if (wizardNotNull)
                {
                    wizard.ResetCh(true);
                }
                else if (knightNotNull)
                {
                    knight.ResetCh(true);
                }
                else if (tankNotNull)
                {
                    tank.ResetCh(true);
                }
                else if (supportNotNull)
                {
                    supportClass.ResetCh(true);
                }
            
            if (bot)
            {
                if (ice.activeInHierarchy == false && aiPath != null && animator.GetCurrentAnimatorStateInfo(0).IsName("death") == false)
                    aiPath.canMove = true;
            }
        }
    }
    void StopAttack(bool resetCharacters)
    {
        if (damaged == false)
        {
            animator.speed = 1;
            canAttack = true;
            canAttackButWithMovement = false;
            networkCanAttack = true;
            detectAttack = false;
            networkDetect = false;
            hurt = false;
            combo = 1;
            for(int i=0;i<hitboxScripts.Count;i++)
            {
                hitboxScripts[i].ResetPlayers();
            }
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (bot == false && isBoss == false && finalBoss == false)
                myObj.layer = 8;
            else myObj.layer = 16;

                if (wizardNotNull)
                {
                    wizard.ResetCh(resetCharacters);
                }
                else if (knightNotNull)
                {
                    knight.ResetCh(resetCharacters);
                }
                else if (tankNotNull)
                {
                    tank.ResetCh(resetCharacters);
                }
                else if (supportNotNull)
                {
                    supportClass.ResetCh(resetCharacters);
                }
            
            if (bot)
            {
                if (ice.activeInHierarchy == false && aiPath != null && animator.GetCurrentAnimatorStateInfo(0).IsName("death") == false)
                    aiPath.canMove = true;
            }
        }
    }
    void ResetPlayers()
    {
        for(int i=0;i<hitboxScripts.Count;i++)
        {
                hitboxScripts[i].ResetPlayers(true );
        }
    }
    public bool detectAttack = false;
    void DetectAttack()
    {
        detectAttack = true;
        networkDetect = true;
    }
    void StopDetection()
    {
        detectAttack = false;
        networkDetect = false;
        combo = 1;
        for (int i = 0; i < hitboxScripts.Count; i++)
        {
            hitboxScripts[i].ResetPlayers();
        }
    }
    public bool canAttack = true;
    public int combo = 1;
    void IncrementCombo()
    {
        combo++;
    }
    void AnimWalk()
    {
        if (speedObjNotNull)
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else if (speedObjActive)
                speed = bonusSpeedValue;
            else speed = originalSpeed;
        }
        else
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else speed = originalSpeed;
        }
        if (aiPath != null)
            aiPath.maxSpeed = speed;
        //if (FindObjectOfType<GameClient>() == null || (FindObjectOfType<GameClient>() != null && FindObjectOfType<GameHost>() != null))
            rb.velocity = myVelocity = speed * -playerTransform.up;
    }
    void StopWalk()
    {
        rb.velocity = myVelocity = Vector2.zero;
    }
    void DetectDirection()
    {
        if ((online && controlling) || online == false)
        {
            float rotZ = playerTransform.rotation.z;
            if (gamePad != null)
            {
                if (playerData.hasFocus && (gamePad.leftStick.magnitude > 0.5))
                {
                    Quaternion playerRotation = playerTransform.rotation;
                    float rotz = playerRotation.z;
                    playerRotation = Quaternion.LookRotation(gamePad.leftStick, new Vector3(0, 0, 1));
                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                    if (gamePad.leftStick == new Vector2(0, 0))
                    {
                        playerRotation = new Quaternion(0, 0, rotz, playerRotation.w);
                        if (online)
                        {
                            FindObjectOfType<GameClient>().Send("PlayerRot" + " " + playerNumber + " " + playerTransform.localEulerAngles.z);
                        }
                    }
                    playerTransform.rotation = playerRotation;
                }
            }
            else if (phoneController != null ? phoneController.gameObject.activeInHierarchy : false)
            {
                if (playerData.hasFocus && (phoneController.dir.magnitude > 0.5))
                {
                    Quaternion playerRotation = playerTransform.rotation;
                    float rotz = playerRotation.z;
                    playerRotation = Quaternion.LookRotation(phoneController.dir, new Vector3(0, 0, 1));
                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                    if (phoneController.dir == new Vector2(0, 0))
                    {
                        playerRotation = new Quaternion(0, 0, rotz, playerRotation.w);
                        if (online)
                        {
                            FindObjectOfType<GameClient>().Send("PlayerRot" + " " + playerNumber + " " + playerTransform.localEulerAngles.z);
                        }
                    }
                    playerTransform.rotation = playerRotation;
                }
            }
            if (online || (keyboard))
            {
                /*bool yAxis = false;
                bool xAxis = false;
                Vector2 vector = new Vector2();
                if (CustomControls.GetButton(secondaryJoystick, buttons[0]))
                {
                    vector = new Vector2(vector.x, speed);
                    yAxis = true;
                }
                else if (CustomControls.GetButton(secondaryJoystick, buttons[1]))
                {
                    vector = new Vector2(vector.x, -speed);
                    yAxis = true;
                }
                else vector = new Vector2(vector.x, 0);
                if (CustomControls.GetButton(secondaryJoystick, buttons[2]))
                {
                    vector = new Vector2(speed, vector.y);
                    xAxis = true;
                }
                else if (CustomControls.GetButton(secondaryJoystick, buttons[3]))
                {
                    vector = new Vector2(-speed, vector.y);
                    xAxis = true;
                }
                else vector = new Vector2(0, vector.y);
                if (xAxis || yAxis)
                {
                    rotZ = playerRotation.z;
                    playerRotation = Quaternion.LookRotation(vector * 100, new Vector3(0, 0, 1));
                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                    if (vector == new Vector2(0, 0))
                    {
                        playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                    }
                    if (online)
                    {
                        FindObjectOfType<GameClient>().Send("PlayerRot" + " " + playerNumber + " " + playerTransform.localEulerAngles.z);
                    }
                }*/

                Vector2 diff = cursorObject.currentPos - (Vector2)playerTransform.position;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                playerTransform.localEulerAngles = new Vector3(0, 0, rot_z + 90);

                if (online)
                {
                    FindObjectOfType<GameClient>().Send("PlayerRot" + " " + playerNumber + " " + playerTransform.localEulerAngles.z);
                }
            }
        }

    }
    public bool spamDirection = false;

    float[] abilTimes = new float[3];
    float[] abilMax = new float[3];
    bool[] onCooldown = { true, true, true };
    bool[] playAnim = new bool[3];
    float[] animTimes = new float[3];

    void ActivateAbillity(int id)
    {
        if (canDoAbils[id - 1])
            return;

        canDoAbils[id - 1] = true;
        abilImages[id - 1].color = new Color32(255, 255, 255, 255);

        abilCooldownImages[id - 1].transform.localScale = new Vector2(imageSizes[id-1], imageSizes[id-1]);

        if (onPhone)
        {
            switch (id-1)
            {
                case 0:
                    abillity1.sizer.localScale = new Vector2(1, 1);
                    break;
                case 1:
                    abillity2.sizer.localScale = new Vector2(1, 1);
                    break;
                case 2:
                    abillity3.sizer.localScale = new Vector2(1, 1);
                    break;
            }
        }

        playAnim[id - 1] = true;
        animTimes[id - 1] = 0;

        switch (id)
        {
            case 1:
                if (abillity1 != null)
                    abillity1.Activate();
                break;
            case 2:
                if (abillity2 != null)
                    abillity2.Activate();
                break;
            case 3:
                if (abillity3 != null)
                    abillity3.Activate();
                break;
        }
    }

    void DeactivateAbillity(int id)
    {
        canDoAbils[id-1] = false;
        abilImages[id-1].color = new Color32(128, 128, 128, 255);

        switch(id)
        {
            case 1:
                if (abillity1 != null)
                    abillity1.Deactivate();
                break;
            case 2:
                if (abillity2 != null)
                    abillity2.Deactivate();
                break;
            case 3:
                if (abillity3 != null)
                    abillity3.Deactivate();
                break;
        }
    }

    void ArcherDashExpire1()
    {
        if (attacks[0] == "attack4" && dash1 >= 1)
        {
            DeactivateAbillity(1);

            abilTimes[0] = 0;
            abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

            dash1 = 0;
            if (isInShops)
                EnableAbil1();
            else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
        }
    }
    void ArcherDashExpire2()
    {
        if (attacks[1] == "attack4" && dash2 >= 1)
        {
            DeactivateAbillity(2);

            abilTimes[1] = 0;
            abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

            dash2 = 0;
            if (isInShops)
                EnableAbil2();
            else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
        }
    }
    void ArcherDashExpire3()
    {
        if (attacks[2] == "attack4" && dash3 >= 1)
        {
            DeactivateAbillity(3);

            abilTimes[2] = 0;
            abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

            dash3 = 0;
            if (isInShops)
                EnableAbil3();
            else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
        }
    }
    public float healthBonus;
    public GameObject healthObj;
    public void HealthUp()
    {
        if (bot == false && this.inDungeon)
        {
            for(int i=0;i<inventorySpawn.itemSlots.Length;i++)
            {
                SelectableItem itemSlot = inventorySpawn.itemSlots[i];
                if (itemSlot.containsItem)
                {
                    if (itemSlot.itemType == 3)
                    {
                        health += 0.1f * (tank.healAmount + maxHealth / 100);
                    }
                }
            }
        }

        DungeonData.instance.AddToQuest(25, (int)(tank.healAmount + maxHealth / 100));

        health += tank.healAmount + maxHealth / 100;
        if (health > maxHealth)
            health = maxHealth;
        hpBarTransform.localScale = new Vector3(health / maxHealth, 1, 1);
        if (botInShops && bot)
            healthText.text = ((int)health).ToString();
        healthObjVfx.Play();
    }
    int dash1 = 0;
    int dash2 = 0;
    int dash3 = 0;
    bool bonusSpeed = false;
    public float bonusSpeedValue;
    public GameObject speedObj;


    //Unused
    public void GetVelocity(float x, float y)
    {
        if (online && networkCanAttack && (animator.GetCurrentAnimatorStateInfo(0).IsName("hurt") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("hurt2") == false))
        {
            rb.velocity = myVelocity = new Vector2(x, y);
            if (x != 0 || y != 0)
            {
                Quaternion playerRotation = playerTransform.rotation;
                float rotz = playerRotation.z;
                playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                if (rb.velocity == new Vector2(0, 0))
                    playerRotation = new Quaternion(0, 0, rotz, playerRotation.w);

                playerTransform.rotation = playerRotation;
            }
        }
    }
    public bool networkCanAttack = true;

    //Unused
    public void PlayAnim(string anim)
    {

        if (online)
        {
            animator.speed = 1;
            if (anim != "idle" && anim != "run")
                UnityEngine.Debug.Log("");
            else
            {
                if (canAttack == false)
                    return;
            }
            if (anim.Contains("hurt"))
            {
                networkCanAttack = false;
                canAttack = false;
                detectAttack = false;
                networkDetect = false;
                hurt = true;

                foreach (GameObject limb in limbs)
                {
                    limb.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("hurt2"))
                {
                    animator.Play("hurt");
                }
                else
                {
                    animator.Play("hurt2");
                }
                hurt1 = !hurt1;
                return;
            }
            if (anim == "death")
            {
                networkCanAttack = false;
                canAttack = false;
                detectAttack = false;
                networkDetect = false;
                Stats.SetActive(false);
                rb.velocity = myVelocity = new Vector2(0, 0);
                rb.bodyType = RigidbodyType2D.Kinematic;
                animator.Play(anim);
                return;
            }
        }
        if (online && ((canAttack || networkCanAttack) && (animator.GetCurrentAnimatorStateInfo(0).IsName("hurt") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("hurt2") == false) || controlling))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
                return;
            if (weaponHitboxNotNull)
            {
                weaponHitbox.playerDirection = true;
                weaponHitbox.limbDirection = false;
            }
            if (poisonParticles.activeInHierarchy == false)
                speed = originalSpeed;
            spamDirection = false;

            if (anim.Contains("attack") || anim.Contains("basic"))
            {
                StopAttack();
                networkCanAttack = false;
                canAttack = false;
                detectAttack = false;
                if (anim.Contains("attack"))
                {
                    detectAttack = false;
                    networkDetect = false;
                    combo = 1;
                    int i = -1;
                    if (anim == attacks[0])
                    {
                        i = 4;
                    }
                    if (anim == attacks[1])
                    {
                        i = 5;
                    }
                    if (anim == attacks[2])
                    {
                        i = 6;
                    }
                    if (attacks[i - 4] == "attack4" && caracterId == 3)
                    {
                        if (i == 4)
                        {
                            dash1++;
                            if (dash1 == 1)
                            {
                                Invoke("ArcherDashExpire1", 3f);
                            }
                            if (dash1 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                dash1 = 0;

                                abilTimes[0] = 0;
                                abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                CancelInvoke("ArcherDashExpire1");
                                if (isInShops)
                                    EnableAbil1();
                                else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                            }
                        }
                        if (i == 5)
                        {
                            dash2++;
                            if (dash2 == 1)
                            {
                                Invoke("ArcherDashExpire2", 3f);
                            }
                            if (dash2 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                abilTimes[1] = 0;
                                abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                dash2 = 0;
                                CancelInvoke("ArcherDashExpire2");
                                if (isInShops)
                                    EnableAbil2();
                                else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                            }
                        }
                        if (i == 6)
                        {
                            dash3++;
                            if (dash3 == 1)
                            {
                                Invoke("ArcherDashExpire3", 3f);
                            }
                            if (dash3 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                abilTimes[2] = 0;
                                abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                dash3 = 0;
                                CancelInvoke("ArcherDashExpire3");
                                if (isInShops)
                                    EnableAbil3();
                                else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                            }
                        }
                    }
                    else
                    {
                        canDoAbils[i - 4] = false;
                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                        abilTimes[i - 4] = 0;
                        abilMax[i - 4] = currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4];

                        if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                        {
                            if (i == 4)
                            {
                                if (isInShops)
                                    EnableAbil1();
                                else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                            if (i == 5)
                            {
                                if (isInShops)
                                    EnableAbil2();
                                else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                            if (i == 6)
                            {
                                if (isInShops)
                                    EnableAbil3();
                                else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 4:
                                    abillity1.Deactivate();
                                    break;
                                case 5:
                                    abillity2.Deactivate();
                                    break;
                                case 6:
                                    abillity3.Deactivate();
                                    break;
                            }
                            if (supportClass != null)
                            {
                                supportClass.AddDamage(-suppCharge);

                                abilMax[i - 4] = suppCharge;
                            }
                            else
                            {
                                tankClass.AddDamage(-tankCharge);

                                abilMax[i - 4] = tankCharge;
                            }
                        }
                    }
                }
            }
            animator.Play(anim);
        }
        else if (online && ((detectAttack || networkDetect) && canAttack == false && (animator.GetCurrentAnimatorStateInfo(0).IsName("hurt") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("hurt2") == false) || controlling))
        {
            if (anim.Contains("attack") || anim.Contains("basic"))
            {
                networkCanAttack = false;
                canAttack = false;
                detectAttack = false;
                if (anim.Contains("attack"))
                {
                    detectAttack = false;
                    networkDetect = false;
                    combo = 1;
                    int i = -1;
                    if (anim == attacks[0])
                    {
                        i = 4;
                    }
                    if (anim == attacks[1])
                    {
                        i = 5;
                    }
                    if (anim == attacks[2])
                    {
                        i = 6;
                    }
                    if (attacks[i - 4] == "attack4" && caracterId == 3)
                    {
                        if (i == 4)
                        {
                            dash1++;
                            if (dash1 == 1)
                            {
                                Invoke("ArcherDashExpire1", 3f);
                            }
                            if (dash1 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                abilTimes[0] = 0;
                                abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                dash1 = 0;
                                CancelInvoke("ArcherDashExpire1");
                                if (isInShops)
                                    EnableAbil1();
                                else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                            }
                        }
                        if (i == 5)
                        {
                            dash2++;
                            if (dash2 == 1)
                            {
                                Invoke("ArcherDashExpire2", 3f);
                            }
                            if (dash2 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                dash2 = 0;

                                abilTimes[1] = 0;
                                abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                CancelInvoke("ArcherDashExpire2");
                                if (isInShops)
                                    EnableAbil2();
                                else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                            }
                        }
                        if (i == 6)
                        {
                            dash3++;
                            if (dash3 == 1)
                            {
                                Invoke("ArcherDashExpire3", 3f);
                            }
                            if (dash3 == 3)
                            {
                                canDoAbils[i - 4] = false;
                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                dash3 = 0;

                                abilTimes[2] = 0;
                                abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                CancelInvoke("ArcherDashExpire3");
                                if (isInShops)
                                    EnableAbil3();
                                else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                            }
                        }
                    }
                    else
                    {
                        canDoAbils[i - 4] = false;
                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                        abilTimes[i-4] = 0;
                        abilMax[i-4] = currentCooldowns[i-4] - cooldownReduction / 100 * currentCooldowns[i-4];

                        if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                        {
                            if (i == 4)
                            {
                                if (isInShops)
                                    EnableAbil1();
                                else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                            if (i == 5)
                            {
                                if (isInShops)
                                    EnableAbil2();
                                else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                            if (i == 6)
                            {
                                if (isInShops)
                                    EnableAbil3();
                                else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 4:
                                    abillity1.Deactivate();
                                    break;
                                case 5:
                                    abillity2.Deactivate();
                                    break;
                                case 6:
                                    abillity3.Deactivate();
                                    break;
                            }
                            if (supportClass != null)
                            {
                                supportClass.AddDamage(-suppCharge);

                                abilMax[i - 4] = suppCharge;
                            }
                            else
                            {
                                tankClass.AddDamage(-tankCharge);

                                abilMax[i - 4] = tankCharge;
                            }
                        }
                    }
                }
                animator.Play(anim);
            }
        }


    }
    public bool networkDetect = false;

    public void BonusSpeed()
    {
        speed = archer.bonusSpeedSpeed + originalSpeed;
        speedObj.SetActive(true);
        CancelInvoke("StopSpeed");
        Invoke("StopSpeed", archer.bonusSpeedTime);
    }
    void StopSpeed()
    {
        speed = originalSpeed;
        speedObj.SetActive(false);
    }
    public GameObject walls;
    public void Walls()
    {
        GameObject wallz = Instantiate(walls);
        wallz.transform.position = playerTransform.position;
        wallz.transform.localScale = new Vector3(tank.wallsRange, tank.wallsRange, 1);
        Destroy(wallz, 4f);
    }
    // Update is called once per frame
    public bool inDungeon = false;
    public Vector3 newPos;
    public Vector3 oldPos;
    public Vector3 oldererPos;

    Transform playerTransform;

    string data;

    Vector3 angularVelocity;
    Quaternion lastRot;

    public void SetT(float t)
    {
        this.t = t;
    }

    private void FixedUpdate()
    {
        if (rendered || bot == false)
        {
            if (bot)
            {
                Quaternion playerRotation = playerTransform.rotation;

                var deltaRot = playerRotation * Quaternion.Inverse(lastRot);
                Vector3 deltaEulerAngles = deltaRot.eulerAngles;
                var eulerRot = new Vector3(0, 0, Mathf.DeltaAngle(0, deltaEulerAngles.z));

                angularVelocity = eulerRot / PublicVariables.fixedDeltaTime;

                lastRot = playerRotation;
            }
            if (online == false && (inDungeon || (bot && finalBoss == false)))
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                bool hurt1 = stateInfo.IsName("hurt");
                bool hurt2 = stateInfo.IsName("hurt2");
                bool isIdle = stateInfo.IsName("idle");
                bool isRunning = stateInfo.IsName("run");
                bool attack1 = stateInfo.IsName("attack");
                bool iceActive = ice.activeInHierarchy;
                if (bot)
                {

                    data = ((locked && inDungeon || (bot && finalBoss == false)) && (bot == false || (bot && ((canAttack || canAttackButWithMovement) && (hurt1 == false && hurt2 == false) && iceActive == false && stunned == false && (isIdle || isRunning || (attack1 && caracterId == 2))) == false))).ToString();
                }
                else data = "";
                if (finalBoss && ableToMove == false && (hurt1 == false && hurt2 == false) && noKnockback)
                    rb.velocity = myVelocity = Vector2.zero;
                if (finalBoss && isInShops)
                {
                    if (((canAttack || canAttackButWithMovement) && (hurt1 == false && hurt2 == false) && iceActive == false && stunned == false && (isIdle || isRunning || (attack1 && caracterId == 2))) == true)
                    {
                        rb.velocity = myVelocity = Vector2.zero;
                    }
                }
                if ((locked && inDungeon || (bot && finalBoss == false)) && ((((canAttack || canAttackButWithMovement) && (hurt1 == false && hurt2 == false) && iceActive == false && stunned == false && (isIdle || isRunning || (attack1 && caracterId == 2))) == false)))
                {
                    if (currentBox.gameObject.activeInHierarchy && !isInShops)
                    {
                        Collider2D[] boxesY;
                        Collider2D[] boxesX = boxesY = Physics2D.OverlapBoxAll(playerTransform.position, new Vector2(0.01f, 0.01f), 0f);
                        //Collider2D[] boxesY = Physics2D.OverlapBoxAll(playerTransform.position, new Vector2(0.01f, 0.01f), 0f);
                        bool okX = false;
                        bool okY = false;

                        foreach (Collider2D box in boxesX)
                        {
                            if (box == currentBox)
                            {
                                okX = okY = true;
                            }
                        }
                        /*foreach (Collider2D box in boxesY)
                        {
                            if (box == currentBox)
                            {
                                okY = true;
                            }
                        }*/

                        if (okX == false || okY == false)
                        {

                            playerTransform.position = currentBox.ClosestPoint(playerTransform.position);


                        }
                    }
                    else locked = false;
                }
                if (bot == false && locked == false && ((playerNumber == 5 && keyboard) || playerNumber != 5))
                {
                    bool sentToCamera = false;
                    if (!OMEGA.DebugMaster.freecam)
                    {
                        Vector2 pos = playerTransform.position;

                        Vector2 cameraPoint = Camera.main.WorldToViewportPoint(pos);

                        if (cameraPoint.x < 0.05 || cameraPoint.x > 0.95 || cameraPoint.y < 0.1 || cameraPoint.y > 0.9)
                        {
                            float screenAspect = Screen.width / (float)Screen.height;
                            float cameraHeight = Camera.main.orthographicSize * 2f;
                            Bounds bounds = new Bounds(
                                Camera.main.transform.position,
                                new Vector3(cameraHeight * screenAspect - 2.5f, cameraHeight - 2.5f, 0));

                            playerTransform.position = bounds.ClosestPoint(playerTransform.position);

                            playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);

                            sentToCamera = true;
                        }
                    }

                    Collider2D[] boxesX = Physics2D.OverlapBoxAll(playerTransform.position, new Vector2(0.01f, 0.01f), 0f);
                    Collider2D[] boxesY = Physics2D.OverlapBoxAll(playerTransform.position, new Vector2(0.01f, 0.01f), 0f);
                    bool okX = false;
                    bool okY = false;

                    foreach (Collider2D box in boxesX)
                    {
                        if (box.GetComponent<thisisaroom>() != null)
                        {
                            okX = true;
                        }
                    }
                    foreach (Collider2D box in boxesY)
                    {
                        if (box.GetComponent<thisisaroom>() != null)
                        {
                            okY = true;
                        }
                    }

                    if (okX == false || okY == false)
                    {
                        if (!isInShops)
                        {
                            if (sentToCamera)
                            {
                                int players = 0;
                                foreach (player player in cameraFollow.playerScripts)
                                {
                                    if (player != null)
                                        players++;
                                }
                                player currentPlayer = null;
                                foreach (player player in cameraFollow.playerScripts)
                                {
                                    if (player != null)
                                    {
                                        if (player != this)
                                        {
                                            if (currentPlayer != null)
                                            {
                                                if (player.myVelocity.magnitude > currentPlayer.myVelocity.magnitude)
                                                    currentPlayer = player;
                                            }
                                            else currentPlayer = player;
                                        }
                                    }
                                }
                                if (currentPlayer != null)
                                    playerTransform.position = currentPlayer.transform.position;
                            }
                            /*else
                            {
                                if (rooms.Length != 0)
                                {
                                    Vector2 pos = rooms[0].ClosestPoint(playerTransform.position);

                                    for (int i = 1; i < rooms.Length; i++)
                                    {

                                        if (rooms[i] != null)
                                        {
                                            Vector2 newPos = rooms[i].ClosestPoint(playerTransform.position);

                                            if (Vector2.Distance(playerTransform.position, pos) > Vector2.Distance(playerTransform.position, newPos))
                                            {
                                                pos = newPos;
                                            }
                                        }
                                    }


                                    playerTransform.position = pos;
                                }
                            }*/
                        }
                    }
                }
            }
            /*if (inDungeon && rb.velocity != Vector2.zero && bot == false && online == false)
            {
                oldererPos = oldPos;
                oldPos = newPos;
                newPos = playerTransform.position;

                    Collider2D[] boxesX = Physics2D.OverlapBoxAll(new Vector2(newPos.x, oldPos.y), new Vector2(0.01f, 0.01f), 0f);
                    Collider2D[] boxesY = Physics2D.OverlapBoxAll(new Vector2(oldPos.x, newPos.y), new Vector2(0.01f, 0.01f), 0f);
                    bool okX = false;
                    bool okY = false;
                    if (locked == true)
                    {
                        foreach (Collider2D box in boxesX)
                        {
                            if (box == currentBox)
                            {
                                okX = true;
                            }
                        }
                        foreach (Collider2D box in boxesY)
                        {
                            if (box == currentBox)
                            {
                                okY = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (Collider2D box in boxesX)
                        {
                            if (box.GetComponent<room>() != null)
                            {
                                okX = true;
                            }
                        }
                        foreach (Collider2D box in boxesY)
                        {
                            if (box.GetComponent<room>() != null)
                            {
                                okY = true;
                            }
                        }
                    }
                    if (okX == false)
                    {
                        playerTransform.position = newPos = new Vector2(oldPos.x, playerTransform.position.y);
                        oldPos = oldererPos;
                    }
                    if (okY == false)
                    {
                        playerTransform.position = newPos = new Vector2(playerTransform.position.x, oldPos.y);
                        oldPos = oldererPos;
                    }

            }
            if(bot)
            {
                oldPos = newPos;
                newPos = playerTransform.position;
                if (((canAttack || canAttackButWithMovement) && (animator.GetCurrentAnimatorStateInfo(0).IsName("hurt") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("hurt2") == false) && ice.activeInHierarchy == false && stunned == false && (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("run") || (animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") && caracterId == 2))) == false)
                {
                        Collider2D[] boxesX = Physics2D.OverlapBoxAll(new Vector2(newPos.x, oldPos.y), new Vector2(0.01f, 0.01f), 0f);
                        Collider2D[] boxesY = Physics2D.OverlapBoxAll(new Vector2(oldPos.x, newPos.y), new Vector2(0.01f, 0.01f), 0f);
                        bool okX = false;
                        bool okY = false;
                        if (locked == true)
                        {
                            foreach (Collider2D box in boxesX)
                            {
                                if (box == currentBox)
                                {
                                    okX = true;
                                }
                            }
                            foreach (Collider2D box in boxesY)
                            {
                                if (box == currentBox)
                                {
                                    okY = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (Collider2D box in boxesX)
                            {
                                if (box.GetComponent<room>() != null)
                                {
                                    okX = true;
                                }
                            }
                            foreach (Collider2D box in boxesY)
                            {
                                if (box.GetComponent<room>() != null)
                                {
                                    okY = true;
                                }
                            }
                        }
                        if (okX == false)
                        {
                            playerTransform.position = newPos = new Vector2(oldPos.x, playerTransform.position.y);
                        }
                        if (okY == false)
                        {
                            playerTransform.position = newPos = new Vector2(playerTransform.position.x, oldPos.y);
                        }

                }
            }*/
        }
    }

    public bool bot = false;
    public string[] botAttacks;
    public float[] botAttackMinDistances;
    float timeTillAttacked = 0;
    public string currentNames;

    public Vector3 movementDirection;

    public Vector2 onlineOldPos;
    public Vector2 onlineNewPos;

    public Vector3 interpolationCurrentPos;
    public Vector3 interpolationTarget;
    public float interpolationTime = -1;

    bool exec = false;
    bool doDamage = false;
    int counter = 0;
    bool canPlayFootstep = true;
    void CanPlayFootstep()
    {
        canPlayFootstep = true;
    }

    bool silenceObjActive;

    void ResetCharge(int i)
    {
        animTimes[i] = 1;

        if (supportNotNull)
        {
            abilCooldownImages[i].localScale = new Vector2((1 - supportClass.damageDealt / suppCharge) * imageSizes[i], imageSizes[i]);

            if (onPhone)
            {
                switch (i)
                {
                    case 0:
                        abillity1.sizer.localScale = new Vector2(1 - supportClass.damageDealt / suppCharge, 1);
                        break;
                    case 1:
                        abillity2.sizer.localScale = new Vector2(1 - supportClass.damageDealt / suppCharge, 1);
                        break;
                    case 2:
                        abillity3.sizer.localScale = new Vector2(1 - supportClass.damageDealt / suppCharge, 1);
                        break;
                }
            }
        }
        else
        {
            abilCooldownImages[i].localScale = new Vector2((1 - tankClass.damageDealt / tankCharge) * imageSizes[i], imageSizes[i]);
            
            if (onPhone)
            {
                switch (i)
                {
                    case 0:
                        abillity1.sizer.localScale = new Vector2(1 - tankClass.damageDealt / suppCharge, 1);
                        break;
                    case 1:
                        abillity2.sizer.localScale = new Vector2(1 - tankClass.damageDealt / suppCharge, 1);
                        break;
                    case 2:
                        abillity3.sizer.localScale = new Vector2(1 - tankClass.damageDealt / suppCharge, 1);
                        break;
                }
            }
        }
    }

    public void ResetCharge1()
    {
        ResetCharge(0);
    }
    public void ResetCharge2()
    {
        ResetCharge(1);
    }
    public void ResetCharge3()
    {
        ResetCharge(2);
    }

    bool animState_death;
    bool animState_run;
    bool slowParticlesActive;
    bool poisonParticlesActive;
    bool speedObjActive;
    bool animState_idle;
    bool firstLimbRed;
    bool animState_basic1;
    bool animState_basic2;
    bool animState_basic3;
    bool animState_basic1_nonstop;
    bool animState_hurt1;
    bool animState_hurt2;
    bool animState_attack1;
    void Update()
    {
        // UW: This funny gem shall remain here for the rest of eternity
        //if (Input.GetKeyDown(KeyCode.D))
            //DecreaseHp(999999, Vector2.zero, this);

        if (rendered || bot == false)
        {
            if (PublicVariables.TimeScale > 0.01f)
            {
                for(int i=0;i<=2;i++)
                {
                    if(!canDoAbils[i] && onCooldown[i])
                    {
                        animTimes[i] = 1;

                        abilTimes[i] += PublicVariables.deltaTime;

                        if (abilTimes[i] > abilMax[i])
                            abilTimes[i] = abilMax[i];

                        abilCooldownImages[i].localScale = new Vector2((1 - abilTimes[i] / abilMax[i]) * imageSizes[i], imageSizes[i]);

                        if(onPhone)
                        {
                            switch(i)
                            {
                                case 0:
                                    abillity1.sizer.localScale = new Vector2(1 - abilTimes[i] / abilMax[i], 1);
                                    break;
                                case 1:
                                    abillity2.sizer.localScale = new Vector2(1 - abilTimes[i] / abilMax[i], 1);
                                    break;
                                case 2:
                                    abillity3.sizer.localScale = new Vector2(1 - abilTimes[i] / abilMax[i], 1);
                                    break;
                            }
                        }
                    }

                    if(playAnim[i])
                    {
                        animTimes[i] += PublicVariables.deltaTime * 2;

                        if(animTimes[i] > 1)
                        {
                            animTimes[i] = 1;
                            abilCooldownColors[i].color = new Color32(0, 0, 0, 128);
                            abilCooldownImages[i].localScale = new Vector2(0, imageSizes[i]);

                            if (onPhone)
                            {
                                switch (i)
                                {
                                    case 0:
                                        abillity1.sizer.localScale = new Vector2(0, 1);
                                        abillity1.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                    case 1:
                                        abillity2.sizer.localScale = new Vector2(0, 1);
                                        abillity2.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                    case 2:
                                        abillity3.sizer.localScale = new Vector2(0, 1);
                                        abillity3.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                }
                            }

                            playAnim[i] = false;
                        }
                        else
                        {
                            abilCooldownColors[i].color = Color32.Lerp(new Color32(255, 255, 255, 128), new Color32(255, 255, 255, 0), animTimes[i]);

                            if (onPhone)
                            {
                                switch (i)
                                {
                                    case 0:
                                        abillity1.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                    case 1:
                                        abillity2.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                    case 2:
                                        abillity3.cooldownImage.color = abilCooldownColors[i].color;
                                        break;
                                }
                            }
                        }
                    }
                }

                if(silenceObjNotNull)
                silenceObjActive = silenceObj.activeInHierarchy;

                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                animState_death = animState.IsName("death");
                animState_run = animState.IsName("run");
                animState_idle = animState.IsName("idle");
                slowParticlesActive = slowParticles.activeInHierarchy;
                poisonParticlesActive = poisonParticles.activeInHierarchy;
                if(speedObjNotNull)
                speedObjActive = speedObj.activeInHierarchy;
                animState_basic1 = animState.IsName("basic1");
                animState_basic2 = animState.IsName("basic2");
                animState_basic3 = animState.IsName("basic3");
                animState_hurt1 = animState.IsName("hurt");
                animState_hurt2 = animState.IsName("hurt2");
                animState_attack1 = animState.IsName("attack1");
                animState_basic1_nonstop = animState.IsName("basic1_nonstop");

                firstLimbRed = firstLimb.color == new Color32(255, 0, 0, 255);

                Vector2 playerPosition = playerTransform.position;

                if (animState_run && canPlayFootstep && bot == false && finalBoss == false && ableToMove)
                {
                    canPlayFootstep = false;
                    Invoke("CanPlayFootstep", 0.35f);
                    AudioSource.PlayClipAtPoint(footstep, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                }

                if (speedObjNotNull)
                {
                    if (slowParticlesActive)
                        speed = originalSpeed / 6;
                    else if (slowParticlesActive)
                        speed = originalSpeed / 3;
                    else if (speedObjActive)
                        speed = bonusSpeedValue;
                    else speed = originalSpeed;
                }
                else
                {
                    if (slowParticlesActive)
                        speed = originalSpeed / 6;
                    else if (slowParticlesActive)
                        speed = originalSpeed / 3;
                    else speed = originalSpeed;
                }
                if (aiPath != null)
                {
                    aiPath.maxSpeed = speed;
                }

                bool ret = false;
                if (bot == false && finalBoss == false && playerNumber != 5)
                {
                    /*try
                    {
                        CustomControls.GetButton(joystick, 0);
                        joystick.Poll();
                    }
                    catch
                    {
                        joystick = null;
                        rb.velocity = new Vector2(0, 0);
                        if (myPlayerData != null)
                            joystick = myPlayerData.joystick;
                        ret = true;
                    }*/
                    if (gamePad == null)
                    {
                        if (myPlayerData != null)
                            gamePad = myPlayerData.gamepad;
                    }
                }

                if (animState_idle)
                {
                    StopAttack(false);
                    if (firstLimbRed && poisonCombo == 0 && fireCombo == 0)
                        StopDamage();
                }
                damaged = false;

                onlineOldPos = onlineNewPos;
                onlineNewPos = playerPosition;


                /*if (online && FindObjectOfType<GameHost>() == null)
                {
                    playerPosition += movementDirection*PublicVariables.deltaTime;
                    if (interpolationTime != -1)
                        interpolationTarget += movementDirection * PublicVariables.deltaTime;
                }

                if(online && interpolationTime != -1)
                {
                    interpolationTime += PublicVariables.deltaTime;

                    float time = interpolationTime / (0.05f);
                    if (time > 1)
                        time = 1;

                    playerPosition = Vector3.Lerp(interpolationCurrentPos, interpolationTarget, time);

                    if (time == 1)
                        interpolationTime = -1;
                }




                if (online)
                    animator.speed = 1;*/
                if (canAttack)
                    timeTillAttacked += PublicVariables.deltaTime;
                else timeTillAttacked = 0;
                /*if (animState_idle && firstLimbRed && online == false && bot == false)
                {
                    //StopAttack();
                }*/
                if (canAttack == false)
                    timeTillStop += PublicVariables.deltaTime;
                else timeTillStop = 0;
                /*if (online && health <= 0)
                {
                    Stats.SetActive(false);
                    rb.velocity = new Vector2(0, 0);
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    animator.Play("death");
                    FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "death");
                }*/

                if (Stats.activeInHierarchy == false)
                {
                    if (finalBoss)
                    {
                        animator.Play("death");
                        FindObjectOfType<FinalBossScene>().InvokeNextFaze();
                        CancelInvoke("Expl");
                        foreach (borderExpl border in FindObjectsOfType<borderExpl>())
                        {
                            border.StopBorder();
                        }
                        GameObject.Find("Borders").SetActive(false);
                    }
                    else
                    {

                        animator.Play("death");
                        circleCollider.enabled = false;
                    }
                }

                if (ret)
                    return;
                if (canAttack2 == false)
                    canAttack2 = true;

                Transform iceTransform = ice.transform;

                iceTransform.eulerAngles = Vector3.zero;
                iceTransform.position = new Vector3(playerPosition.x, playerPosition.y + 0.75f, 0);
                /*if (online && controlling && bot == false)
                {
                    FindObjectOfType<GameClient>().Send("PlayerAbil " + playerNumber + " " + canDoAbils[0] + " " + canDoAbils[1] + " " + canDoAbils[2]);
                }*/

                if(bot == false)
                {
                    if(movementToggle)
                    {
                        if (playerNumber == 5 ? (keyboard == false ? phoneToggle.isInteracting : Input.GetKey(KeyCode.LeftShift)) : (gamePad.leftTrigger > 0.5 || gamePad.rightTrigger > 0.5))
                        {
                            if (togglePressed == false)
                            {
                                movementToggled = !movementToggled;
                                togglePressed = true;
                            }
                        }
                        else togglePressed = false;
                    }
                    else
                    {
                        movementToggled = true;
                        if (playerNumber == 5 ? (keyboard == false ? phoneToggle.isInteracting : Input.GetKey(KeyCode.LeftShift)) : (gamePad.leftTrigger > 0.5 || gamePad.rightTrigger > 0.5))
                            movementToggled = false;
                    }

                    lockObj.SetActive(!movementToggled);
                    if(onPhone)
                    {
                        if (movementToggled)
                            phoneToggle.SetUnlockImage();
                        else phoneToggle.SetLockImage();
                    }
                }

                if (spamDirection && bot == false && HumanPlayerAbleToMove())
                    DetectDirection();
                if (controlling && bot == false && HumanPlayerAbleToMove())
                {
                    if (online == false && keyboard)
                    {
                        if (playerData.hasFocus && Input.GetMouseButton(1))
                        {
                            followTarget = true;
                            cursorTarget.transform.position = cursorObject.currentPos;
                            cursorBoxCollider.enabled = false;
                            cursorBoxCollider.enabled = true;

                            if (MathUtils.CompareDistances(playerPosition, cursorTarget.transform.position, 0.5f, MathUtils.CompareTypes.LessThan))
                            {
                                followTarget = false;
                            }

                            if (TutorialScene.instance != null)
                            {
                                if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                                {
                                    Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                                    if (tutorial.textIndex == 1)
                                        tutorial.NextQuote();
                                }
                            }
                        }

                    }
                    if (gamePad != null || playerNumber == 5)
                    {
                        if (detectAttack && (animState_basic1 || animState_basic2 || animState_basic1_nonstop))
                        {
                            bool canDo = false;
                            for (int i = 4; i <= 6; i++)
                            {
                                if (playerData.hasFocus && GetGamePadButton(buttons[i]) && canDoAbils[i - 4])
                                {
                                    if (onPhone == false)
                                    {
                                        if (i == 6 && playerNumber == 5 && keyboard)
                                        {
                                            if (itemShop != null)
                                            {
                                                bool ok = true;
                                                foreach (itemMenu menu in itemMenus)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (abilityShop != null)
                                            {
                                                bool ok = true;
                                                foreach (abilityShop menu in abilityShops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (shop != null)
                                            {
                                                bool ok = true;
                                                foreach (Shop menu in shops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                        }
                                        else if (i == 4 && playerNumber != 5)
                                        {
                                            if (itemShop != null)
                                            {
                                                bool ok = true;
                                                foreach (itemMenu menu in itemMenus)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (abilityShop != null)
                                            {
                                                bool ok = true;
                                                foreach (abilityShop menu in abilityShops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (shop != null)
                                            {
                                                bool ok = true;
                                                foreach (Shop menu in shops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                        }
                                    }

                                    DetectDirection();
                                    if (online == false)
                                    {
                                        animator.Play(attacks[i - 4]);

                                        if (lockRoom != null)
                                            lockRoom.usedAbil = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + attacks[i - 4]);*/
                                    detectAttack = false;
                                    if (online == false)
                                    {
                                        if (attacks[i - 4] == "attack4" && caracterId == 3)
                                        {
                                            if (i == 4)
                                            {
                                                dash1++;
                                                if (dash1 == 1)
                                                {
                                                    Invoke("ArcherDashExpire1", 3f);
                                                }
                                                if (dash1 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity1 != null)
                                                        abillity1.Deactivate();
                                                    dash1 = 0;

                                                    abilTimes[0] = 0;
                                                    abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                                    CancelInvoke("ArcherDashExpire1");
                                                    if (isInShops)
                                                        EnableAbil1();
                                                    else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                                                }
                                            }
                                            if (i == 5)
                                            {
                                                dash2++;
                                                if (dash2 == 1)
                                                {
                                                    Invoke("ArcherDashExpire2", 3f);
                                                }
                                                if (dash2 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity2 != null)
                                                        abillity2.Deactivate();
                                                    dash2 = 0;

                                                    abilTimes[1] = 0;
                                                    abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                                    CancelInvoke("ArcherDashExpire2");
                                                    if (isInShops)
                                                        EnableAbil2();
                                                    else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                                                }
                                            }
                                            if (i == 6)
                                            {
                                                dash3++;
                                                if (dash3 == 1)
                                                {
                                                    Invoke("ArcherDashExpire3", 3f);
                                                }
                                                if (dash3 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity3 != null)
                                                        abillity3.Deactivate();
                                                    dash3 = 0;

                                                    abilTimes[2] = 0;
                                                    abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                                    CancelInvoke("ArcherDashExpire3");
                                                    if (isInShops)
                                                        EnableAbil3();
                                                    else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            canDoAbils[i - 4] = false;
                                            abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                            abilTimes[i-4] = 0;
                                            abilMax[i-4] = currentCooldowns[i-4] - cooldownReduction / 100 * currentCooldowns[i-4];

                                            if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                                            {
                                                if (i == 4)
                                                {
                                                    if (abillity1 != null)
                                                        abillity1.Deactivate();

                                                    if (isInShops)
                                                        EnableAbil1();
                                                    else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                                if (i == 5)
                                                {
                                                    if (abillity2 != null)
                                                        abillity2.Deactivate();

                                                    if (isInShops)
                                                        EnableAbil2();
                                                    else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                                if (i == 6)
                                                {
                                                    if (abillity3 != null)
                                                        abillity3.Deactivate();
                                                    if (isInShops)
                                                        EnableAbil3();
                                                    else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                            }
                                            else
                                            {
                                                switch (i)
                                                {
                                                    case 4:
                                                        abillity1.Deactivate();
                                                        break;
                                                    case 5:
                                                        abillity2.Deactivate();
                                                        break;
                                                    case 6:
                                                        abillity3.Deactivate();
                                                        break;
                                                }
                                                if (supportClass != null)
                                                {
                                                    supportClass.AddDamage(-suppCharge);

                                                    abilMax[i - 4] = suppCharge;
                                                }
                                                else
                                                {
                                                    tankClass.AddDamage(-tankCharge);

                                                    abilMax[i - 4] = tankCharge;
                                                }
                                            }
                                        }
                                    }
                                    canDo = true;
                                    break;

                                }
                            }

                            if (playerData.hasFocus && GetGamePadButton(buttons[7]) && canDo == false)
                            {
                                if (online)
                                {
                                    if (animState_basic1 || animState_basic1_nonstop)
                                        combo = 2;
                                    if (animState_basic2)
                                        combo = 3;
                                }
                                DetectDirection();
                                if (combo >= 3 && nonStop)
                                {
                                    combo = 1;
                                    if (online == false)
                                    {
                                        animator.Play("basic" + combo + "_nonstop");

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic" + combo + "_nonstop");*/
                                }
                                else
                                {
                                    if (online == false)
                                    {
                                        animator.Play("basic" + combo);

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic" + combo);*/
                                }
                                detectAttack = false;
                            }

                        }
                    }
                    if (online || (keyboard))
                    {
                        if (detectAttack && (animState_basic1 || animState_basic2 || animState_basic1_nonstop))
                        {
                            bool canDo = false;
                            for (int i = 4; i <= 6; i++)
                            {
                                if (playerData.hasFocus && GetKeyboardButton(buttons[i]) && canDoAbils[i - 4])
                                {
                                    if (onPhone == false)
                                    {
                                        if (i == 6 && playerNumber == 5 && keyboard)
                                        {
                                            if (itemShop != null)
                                            {
                                                bool ok = true;
                                                foreach (itemMenu menu in itemMenus)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (abilityShop != null)
                                            {
                                                bool ok = true;
                                                foreach (abilityShop menu in abilityShops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (shop != null)
                                            {
                                                bool ok = true;
                                                foreach (Shop menu in shops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                        }
                                        else if (i == 4 && playerNumber != 5)
                                        {
                                            if (itemShop != null)
                                            {
                                                bool ok = true;
                                                foreach (itemMenu menu in itemMenus)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (abilityShop != null)
                                            {
                                                bool ok = true;
                                                foreach (abilityShop menu in abilityShops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                            if (shop != null)
                                            {
                                                bool ok = true;
                                                foreach (Shop menu in shops)
                                                {
                                                    if (menu.players.Contains(myPlayer))
                                                    {
                                                        ok = false; break;
                                                    }
                                                }
                                                if (ok == false)
                                                    break;
                                            }
                                        }
                                    }
                                    DetectDirection();
                                    if (online == false)
                                    {
                                        animator.Play(attacks[i - 4]);

                                        if (lockRoom != null)
                                            lockRoom.usedAbil = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + attacks[i - 4]);*/
                                    detectAttack = false;
                                    if (online == false)
                                    {
                                        if (attacks[i - 4] == "attack4" && caracterId == 3)
                                        {
                                            if (i == 4)
                                            {
                                                dash1++;
                                                if (dash1 == 1)
                                                {
                                                    Invoke("ArcherDashExpire1", 3f);
                                                }
                                                if (dash1 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity1 != null)
                                                        abillity1.Deactivate();
                                                    dash1 = 0;

                                                    abilTimes[0] = 0;
                                                    abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                                    CancelInvoke("ArcherDashExpire1");
                                                    if (isInShops)
                                                        EnableAbil1();
                                                    else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                                                }
                                            }
                                            if (i == 5)
                                            {
                                                dash2++;
                                                if (dash2 == 1)
                                                {
                                                    Invoke("ArcherDashExpire2", 3f);
                                                }
                                                if (dash2 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity2 != null)
                                                        abillity2.Deactivate();
                                                    dash2 = 0;

                                                    abilTimes[1] = 0;
                                                    abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                                    CancelInvoke("ArcherDashExpire2");
                                                    if (isInShops)
                                                        EnableAbil2();
                                                    else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                                                }
                                            }
                                            if (i == 6)
                                            {
                                                dash3++;
                                                if (dash3 == 1)
                                                {
                                                    Invoke("ArcherDashExpire3", 3f);
                                                }
                                                if (dash3 == 3)
                                                {
                                                    canDoAbils[i - 4] = false;
                                                    abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                    if (abillity3 != null)
                                                        abillity3.Deactivate();
                                                    dash3 = 0;

                                                    abilTimes[2] = 0;
                                                    abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                                    CancelInvoke("ArcherDashExpire3");
                                                    if (isInShops)
                                                        EnableAbil3();
                                                    else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            canDoAbils[i - 4] = false;
                                            abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                            abilTimes[i-4] = 0;
                                            abilMax[i-4] = currentCooldowns[i-4] - cooldownReduction / 100 * currentCooldowns[i-4];

                                            if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                                            {
                                                if (i == 4)
                                                {
                                                    if (abillity1 != null)
                                                        abillity1.Deactivate();
                                                    if (isInShops)
                                                        EnableAbil1();
                                                    else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                                if (i == 5)
                                                {
                                                    if (abillity2 != null)
                                                        abillity2.Deactivate();
                                                    if (isInShops)
                                                        EnableAbil2();
                                                    else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                                if (i == 6)
                                                {
                                                    if (abillity3 != null)
                                                        abillity3.Deactivate();
                                                    if (isInShops)
                                                        EnableAbil3();
                                                    else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                }
                                            }
                                            else
                                            {
                                                switch (i)
                                                {
                                                    case 4:
                                                        abillity1.Deactivate();
                                                        break;
                                                    case 5:
                                                        abillity2.Deactivate();
                                                        break;
                                                    case 6:
                                                        abillity3.Deactivate();
                                                        break;
                                                }
                                                if (supportClass != null)
                                                {
                                                    supportClass.AddDamage(-suppCharge);

                                                    abilMax[i - 4] = suppCharge;
                                                }
                                                else
                                                {
                                                    tankClass.AddDamage(-tankCharge);

                                                    abilMax[i - 4] = tankCharge;
                                                }
                                            }
                                        }
                                    }
                                    canDo = true;
                                    break;
                                }
                            }

                            if (playerData.hasFocus && ((Input.GetMouseButton(0) && keyboard) || (GetGamePadButton(buttons[7]) && keyboard == false && playerNumber == 5)) && silenceObjActive == false && canDo == false && (inventorySpawn != null ? inventorySpawn.megamap.activeInHierarchy == false : true))
                            {
                                if (online)
                                {
                                    if (animState_basic1 || animState_basic1_nonstop)
                                        combo = 2;
                                    if (animState_basic2)
                                        combo = 3;
                                }

                                DetectDirection();
                                if (combo >= 3 && nonStop)
                                {
                                    combo = 1;
                                    if (online == false)
                                    {
                                        animator.Play("basic" + combo + "_nonstop");

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic" + combo + "_nonstop");*/
                                }
                                else
                                {
                                    if (online == false)
                                    {
                                        animator.Play("basic" + combo);

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }
                                    /*if (online)
                                    {
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic" + combo);
                                    }*/
                                }
                                if (online == false)
                                    detectAttack = false;
                            }

                        }
                    }
                }

                #region

                #endregion
                if ((canAttack || canAttackButWithMovement) && (animState_hurt1 == false && animState_hurt2 == false) && ice.activeInHierarchy == false && stunned == false && (animState_idle || animState_run || (animState_attack1 && caracterId == 2)))
                {

                    if (online)
                    {
                        if (caracterId == 1)
                            originalSpeed = 5.5f;
                        else if (caracterId == 2)
                            originalSpeed = 8f;
                        else if (caracterId == 3)
                            originalSpeed = 10f;
                        else if (caracterId == 4)
                            originalSpeed = 3.5f;
                        else if (caracterId == 5)
                            originalSpeed = 9f;
                    }


                    if ((rb.velocity != Vector2.zero || followTarget) && online == false && inDungeon)
                    {
                        if (timeTillAttacked <= 1.5f && bot == false)
                            speed = speed * (timeTillAttacked * 2 + 1);
                        else if (bot == false) speed = speed * 4;
                    }
                    else timeTillAttacked = 0;
                    if (fireParticles.activeInHierarchy == false && canAttack2 && slowParticlesActive == false)
                    {
                        if (bot && t == 1)
                        {
                            for (int i = 0; i < LimbSprites.Count; i++)
                            {
                                LimbSprites[i].color = limbColors[i].Color;
                            }

                        }
                        else if (bot == false)
                        {
                            for (int i = 0; i < LimbSprites.Count; i++)
                            {
                                LimbSprites[i].color = limbColors[i].Color;
                            }
                        }
                    }
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    if (bot == false && isBoss == false && finalBoss == false)
                        myObj.layer = 8;
                    else myObj.layer = 16;
                    bool yAxis = false;
                    bool xAxis = false;
                    if (controlling && online == false && bot == false && HumanPlayerAbleToMove())
                    {
                        float x = 0;
                        float y = 0;
                        if (gamePad != null)
                        {
                            if (playerData.hasFocus && (gamePad.leftStick.magnitude > 0.5))
                            {
                                Vector2 oldSpeed = rb.velocity;
                                rb.velocity = myVelocity = gamePad.leftStick * speed;
                                x = gamePad.leftStick.x;
                                y = gamePad.leftStick.y;
                                if (x == 0 && y == 0 && online)
                                {
                                    rb.velocity = myVelocity = oldSpeed;
                                }

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 1)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (online == false)
                            {
                                rb.velocity = myVelocity = Vector2.zero;
                            }
                        }
                        else if (phoneController != null ? phoneController.gameObject.activeInHierarchy : false)
                        {
                            if (playerData.hasFocus && (phoneController.dir.magnitude > 0.5))
                            {
                                Vector2 oldSpeed = rb.velocity;
                                rb.velocity = myVelocity = phoneController.dir * speed;
                                x = phoneController.dir.x;
                                y = phoneController.dir.y;
                                if (x == 0 && y == 0 && online)
                                {
                                    rb.velocity = myVelocity = oldSpeed;
                                }

                                if (TutorialScene.instance != null)
                                {
                                    if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                                    {
                                        Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                                        if (tutorial.textIndex == 1)
                                            tutorial.NextQuote();
                                    }
                                }
                            }
                            else if (online == false)
                                rb.velocity = myVelocity = Vector2.zero;
                        }
                        if (online && x == 0 && y == 0)
                        {
                            if (playerData.hasFocus && Input.GetKey(KeyCode.W))
                            {
                                rb.velocity = myVelocity = new Vector2(rb.velocity.x, speed);
                                yAxis = true;
                                y = 1;
                            }
                            else if (playerData.hasFocus && Input.GetKey(KeyCode.S))
                            {
                                rb.velocity = myVelocity = new Vector2(rb.velocity.x, -speed);
                                yAxis = true;
                                y = -1;
                            }
                            else rb.velocity = myVelocity = new Vector2(rb.velocity.x, 0);
                            if (playerData.hasFocus && Input.GetKey(KeyCode.D))
                            {
                                rb.velocity = myVelocity = new Vector2(speed, rb.velocity.y);
                                xAxis = true;
                                x = 1;

                            }
                            else if (playerData.hasFocus && Input.GetKey(KeyCode.A))
                            {
                                rb.velocity = myVelocity = new Vector2(-speed, rb.velocity.y);
                                xAxis = true;
                                x = -1;
                            }
                            else rb.velocity = myVelocity = new Vector2(0, rb.velocity.y);
                            if ((xAxis || yAxis) && canAttackButWithMovement == false)
                            {
                                if (online == false && movementToggled)
                                {
                                    animator.Play("run");
                                }

                                Quaternion playerRotation = playerTransform.rotation;

                                float rotZ = playerRotation.z;
                                playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                if (rb.velocity == new Vector2(0, 0))
                                    playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);


                                playerTransform.rotation = playerRotation;

                                if (movementToggled == false)
                                {
                                    rb.velocity = Vector2.zero;
                                }
                            }
                        }
                        if (x != 0 || y != 0)
                        {
                            /*if (online && canAttack && networkCanAttack)
                            {
                                FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "run");

                            }*/
                        }
                        if (gamePad != null && keyboard == false)
                        {
                            if (playerData.hasFocus && gamePad.leftStick.y < 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        yAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }

                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            else if (playerData.hasFocus && gamePad.leftStick.y > 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        yAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }
                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            if (playerData.hasFocus && gamePad.leftStick.x > 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        xAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }
                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            else if (playerData.hasFocus && gamePad.leftStick.x < 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        xAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }
                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                        }
                        else if (phoneController != null ? phoneController.gameObject.activeInHierarchy : false && keyboard == false)
                        {
                            if (playerData.hasFocus && phoneController.dir.y < 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        yAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }

                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            else if (playerData.hasFocus && phoneController.dir.y > 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        yAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }
                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            if (playerData.hasFocus && phoneController.dir.x > 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        xAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }
                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                            else if (playerData.hasFocus && phoneController.dir.x < 0.5)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    if (rb.velocity != new Vector2(0, 0))
                                    {
                                        xAxis = true;
                                        if (online == false && movementToggled)
                                        {
                                            animator.Play("run");
                                        }

                                        Quaternion playerRotation = playerTransform.rotation;

                                        float rotZ = playerRotation.z;
                                        playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                        playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);

                                        playerTransform.rotation = playerRotation;

                                        if (movementToggled == false)
                                        {
                                            rb.velocity = Vector2.zero;
                                        }
                                    }
                                }
                            }
                        }
                        if ((xAxis == false && yAxis == false) || movementToggled == false)
                        {
                            if (canAttackButWithMovement == false)
                            {
                                if (online == false && followTarget == false)
                                    animator.Play("idle");
                                /*if (online && canAttack && networkCanAttack && (animState_hurt1 == false && animState_hurt2 == false))
                                {
                                    FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "idle");
                                }*/
                            }
                        }
                        /*if (online)
                        {
                            FindObjectOfType<GameClient>().Send("SetVelocity " + FindObjectOfType<GameClient>().playerId + " " + x + " " + y);
                            if (FindObjectOfType<GameHost>() == null)
                                rb.velocity = new Vector2(0, 0);
                        }*/
                    }
                    if (controlling && (canAttack || (networkCanAttack && online)) && bot == false && HumanPlayerAbleToMove())
                    {
                        if (gamePad != null || playerNumber == 5)
                        {
                            if (canAttackButWithMovement == false && (animState_idle || animState_run))
                            {
                                bool canDo = false;
                                for (int i = 4; i <= 6; i++)
                                {
                                    if (playerData.hasFocus && (GetGamePadButton(buttons[i]) || GetKeyboardButton(buttons[i])) && canDoAbils[i - 4])
                                    {
                                        if (onPhone == false)
                                        {
                                            if (i == 6 && playerNumber == 5 && keyboard)
                                            {
                                                if (itemShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (itemMenu menu in itemMenus)
                                                    {
                                                        if (menu != null && menu.players != null && menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (abilityShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (abilityShop menu in abilityShops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (shop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (Shop menu in shops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                            }
                                            else if (i == 4 && playerNumber != 5)
                                            {
                                                if (itemShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (itemMenu menu in itemMenus)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (abilityShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (abilityShop menu in abilityShops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (shop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (Shop menu in shops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                            }
                                        }
                                        DetectDirection();
                                        if (online == false)
                                        {
                                            animator.Play(attacks[i - 4]);

                                            if (lockRoom != null)
                                                lockRoom.usedAbil = true;
                                        }
                                        /*if (online)
                                            FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + attacks[i - 4]);*/

                                        if (TutorialScene.instance != null)
                                        {
                                            if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                                            {
                                                Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                                                if (tutorial.textIndex == 3)
                                                    tutorial.NextQuote();
                                            }
                                        }

                                        if (online == false)
                                        {
                                            if (attacks[i - 4] == "attack4" && caracterId == 3)
                                            {
                                                if (i == 4)
                                                {
                                                    dash1++;
                                                    if (dash1 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire1", 3f);
                                                    }
                                                    if (dash1 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity1 != null)
                                                            abillity1.Deactivate();
                                                        dash1 = 0;

                                                        abilTimes[0] = 0;
                                                        abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                                        CancelInvoke("ArcherDashExpire1");
                                                        if (isInShops)
                                                            EnableAbil1();
                                                        else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                                                    }
                                                }
                                                if (i == 5)
                                                {
                                                    dash2++;
                                                    if (dash2 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire2", 3f);
                                                    }
                                                    if (dash2 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity2 != null)
                                                            abillity2.Deactivate();
                                                        dash2 = 0;

                                                        abilTimes[1] = 0;
                                                        abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                                        CancelInvoke("ArcherDashExpire2");
                                                        if (isInShops)
                                                            EnableAbil2();
                                                        else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                                                    }
                                                }
                                                if (i == 6)
                                                {
                                                    dash3++;
                                                    if (dash3 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire3", 3f);
                                                    }
                                                    if (dash3 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity3 != null)
                                                            abillity3.Deactivate();
                                                        dash3 = 0;

                                                        abilTimes[2] = 0;
                                                        abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                                        CancelInvoke("ArcherDashExpire3");
                                                        if (isInShops)
                                                            EnableAbil3();
                                                        else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                canDoAbils[i - 4] = false;
                                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                                abilTimes[i-4] = 0;
                                                abilMax[i-4] = currentCooldowns[i-4] - cooldownReduction / 100 * currentCooldowns[i-4];

                                                if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                                                {
                                                    if (i == 4)
                                                    {
                                                        if (abillity1 != null)
                                                            abillity1.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil1();
                                                        else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                    if (i == 5)
                                                    {
                                                        if (abillity2 != null)
                                                            abillity2.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil2();
                                                        else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                    if (i == 6)
                                                    {
                                                        if (abillity3 != null)
                                                            abillity3.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil3();
                                                        else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                }
                                                else
                                                {
                                                    switch (i)
                                                    {
                                                        case 4:
                                                            abillity1.Deactivate();
                                                            break;
                                                        case 5:
                                                            abillity2.Deactivate();
                                                            break;
                                                        case 6:
                                                            abillity3.Deactivate();
                                                            break;
                                                    }
                                                    if (supportClass != null)
                                                    {
                                                        supportClass.AddDamage(-suppCharge);

                                                        abilMax[i - 4] = suppCharge;
                                                    }
                                                    else
                                                    {
                                                        tankClass.AddDamage(-tankCharge);

                                                        abilMax[i - 4] = tankCharge;
                                                    }
                                                }
                                            }
                                        }
                                        canDo = true;
                                        break;

                                    }
                                }
                                if (playerData.hasFocus && ((GetGamePadButton(buttons[7]) && playerNumber != 5) || (((keyboard && Input.GetMouseButton(0)) || (GetGamePadButton(buttons[7]) && keyboard == false && playerNumber == 5)) && silenceObjActive == false && playerNumber == 5 && (inventorySpawn != null ? inventorySpawn.megamap.activeInHierarchy == false : true))) && canDo == false)
                                {
                                    if (online)
                                    {
                                        combo = 1;
                                    }

                                    if (TutorialScene.instance != null)
                                    {
                                        if (TutorialScene.instance.movement.gameObject.activeInHierarchy)
                                        {
                                            Tutorial tutorial = TutorialScene.instance.movement.GetComponentInChildren<Tutorial>();
                                            if (tutorial.textIndex == 2)
                                                tutorial.NextQuote();
                                        }
                                    }

                                    DetectDirection();
                                    if (online == false)
                                    {
                                        animator.Play("basic1");

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }
                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic1");*/
                                }
                            }
                        }
                        else if (online)
                        {
                            if (canAttackButWithMovement == false && (animState_idle || animState_run || online == false))
                            {
                                bool canDo = false;
                                for (int i = 4; i <= 6; i++)
                                {
                                    if (playerData.hasFocus && (GetKeyboardButton(buttons[i])) && canDoAbils[i - 4])
                                    {
                                        if (onPhone == false)
                                        {
                                            if (i == 6 && playerNumber == 5 && keyboard)
                                            {
                                                if (itemShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (itemMenu menu in itemMenus)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (abilityShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (abilityShop menu in abilityShops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (shop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (Shop menu in shops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                            }
                                            else if (i == 4 && playerNumber != 5)
                                            {
                                                if (itemShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (itemMenu menu in itemMenus)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (abilityShop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (abilityShop menu in abilityShops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                                if (shop != null)
                                                {
                                                    bool ok = true;
                                                    foreach (Shop menu in shops)
                                                    {
                                                        if (menu.players.Contains(myPlayer))
                                                        {
                                                            ok = false; break;
                                                        }
                                                    }
                                                    if (ok == false)
                                                        break;
                                                }
                                            }
                                        }
                                        DetectDirection();
                                        if (online == false)
                                        {
                                            animator.Play(attacks[i - 4]);

                                            if (lockRoom != null)
                                                lockRoom.usedAbil = true;
                                        }
                                        /*if (online)
                                            FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + attacks[i - 4]);*/
                                        if (online == false)
                                        {
                                            if (attacks[i - 4] == "attack4" && caracterId == 3)
                                            {
                                                if (i == 4)
                                                {
                                                    dash1++;
                                                    if (dash1 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire1", 3f);
                                                    }
                                                    if (dash1 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity1 != null)
                                                            abillity1.Deactivate();
                                                        dash1 = 0;

                                                        abilTimes[0] = 0;
                                                        abilMax[0] = currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0];

                                                        CancelInvoke("ArcherDashExpire1");
                                                        if (isInShops)
                                                            EnableAbil1();
                                                        else Invoke("EnableAbil1", currentCooldowns[0] - cooldownReduction / 100 * currentCooldowns[0]);
                                                    }
                                                }
                                                if (i == 5)
                                                {
                                                    dash2++;
                                                    if (dash2 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire2", 3f);
                                                    }
                                                    if (dash2 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity2 != null)
                                                            abillity2.Deactivate();
                                                        dash2 = 0;

                                                        abilTimes[1] = 0;
                                                        abilMax[1] = currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1];

                                                        CancelInvoke("ArcherDashExpire2");
                                                        if (isInShops)
                                                            EnableAbil2();
                                                        else Invoke("EnableAbil2", currentCooldowns[1] - cooldownReduction / 100 * currentCooldowns[1]);
                                                    }
                                                }
                                                if (i == 6)
                                                {
                                                    dash3++;
                                                    if (dash3 == 1)
                                                    {
                                                        Invoke("ArcherDashExpire3", 3f);
                                                    }
                                                    if (dash3 == 3)
                                                    {
                                                        canDoAbils[i - 4] = false;
                                                        abilImages[i - 4].color = new Color32(128, 128, 128, 255);
                                                        if (abillity3 != null)
                                                            abillity3.Deactivate();
                                                        dash3 = 0;

                                                        abilTimes[2] = 0;
                                                        abilMax[2] = currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2];

                                                        CancelInvoke("ArcherDashExpire3");
                                                        if (isInShops)
                                                            EnableAbil3();
                                                        else Invoke("EnableAbil3", currentCooldowns[2] - cooldownReduction / 100 * currentCooldowns[2]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                canDoAbils[i - 4] = false;
                                                abilImages[i - 4].color = new Color32(128, 128, 128, 255);

                                                abilTimes[i-4] = 0;
                                                abilMax[i-4] = currentCooldowns[i-4] - cooldownReduction / 100 * currentCooldowns[i-4];

                                                if (((supportClass == null || attacks[i - 4] != "attack3") && (tankClass == null || attacks[i - 4] != "attack6")) || inDungeon == false)
                                                {
                                                    if (i == 4)
                                                    {
                                                        if (abillity1 != null)
                                                            abillity1.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil1();
                                                        else Invoke("EnableAbil1", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                    if (i == 5)
                                                    {
                                                        if (abillity2 != null)
                                                            abillity2.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil2();
                                                        else Invoke("EnableAbil2", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                    if (i == 6)
                                                    {
                                                        if (abillity3 != null)
                                                            abillity3.Deactivate();
                                                        if (isInShops)
                                                            EnableAbil3();
                                                        else Invoke("EnableAbil3", currentCooldowns[i - 4] - cooldownReduction / 100 * currentCooldowns[i - 4]);
                                                    }
                                                }
                                                else
                                                {
                                                    switch (i)
                                                    {
                                                        case 4:
                                                            abillity1.Deactivate();
                                                            break;
                                                        case 5:
                                                            abillity2.Deactivate();
                                                            break;
                                                        case 6:
                                                            abillity3.Deactivate();
                                                            break;
                                                    }
                                                    if (supportClass != null)
                                                    {
                                                        supportClass.AddDamage(-suppCharge);

                                                        abilMax[i - 4] = suppCharge;
                                                    }
                                                    else
                                                    {
                                                        tankClass.AddDamage(-tankCharge);

                                                        abilMax[i - 4] = tankCharge;
                                                    }
                                                }
                                            }
                                        }
                                        canDo = true;
                                        break;

                                    }
                                }
                                if (playerData.hasFocus && ((keyboard && Input.GetMouseButton(0)) || (GetGamePadButton(buttons[7]) && keyboard == false && playerNumber == 5)) && silenceObjActive == false && canDo == false && inventorySpawn.megamap.activeInHierarchy == false)
                                {
                                    combo = 1;

                                    DetectDirection();
                                    if (online == false)
                                    {
                                        animator.Play("basic1");

                                        if (lockRoom != null)
                                            lockRoom.usedBasic = true;
                                    }

                                    /*if (online)
                                        FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "basic1");*/
                                }
                            }
                        }
                    }

                    #region
                    if (controlling && online && bot == false && HumanPlayerAbleToMove())
                    {

                        float x = 0;
                        float y = 0;
                        /*if (joystick != null)
                        {
                            if ((CustomControls.GetAxis(joystick).Yaxis > 40000 || CustomControls.GetAxis(joystick).Yaxis < 1tankCharge || CustomControls.GetAxis(joystick).Xaxis > 40000 || CustomControls.GetAxis(joystick).Xaxis < 1tankCharge))
                            {
                                Vector2 oldSpeed = rb.velocity;
                                rb.velocity = gamePad.leftStick;
                                x = (CustomControls.GetAxis(joystick).Xaxis - 32767) / 32767;
                                y = (CustomControls.GetAxis(joystick).Yaxis - 32767) / 32767;
                                if (x == 0 && y == 0 && online)
                                {
                                    rb.velocity = oldSpeed;
                                }
                            }
                            else if (online == false)
                                rb.velocity = Vector2.zero;
                        }*/
                        if (online)// && x == 0 && y == 0)
                        {
                            /*if(playerData.hasFocus && Input.GetMouseButton(1))
                            {
                                    FindObjectOfType<GameClient>().Send("PlayerTarget" + " " +playerNumber+ " " + cursorObject.currentPos.x + " " + cursorObject.currentPos.y);
                            }*/


                            /*if (CustomControls.GetButton(secondaryJoystick, buttons[0]))
                            {
                                rb.velocity = new Vector2(rb.velocity.x, speed);
                                yAxis = true;
                                y = 1;
                            }
                            else if (CustomControls.GetButton(secondaryJoystick, buttons[1]))
                            {
                                rb.velocity = new Vector2(rb.velocity.x, -speed);
                                yAxis = true;
                                y = -1;
                            }
                            else rb.velocity = new Vector2(rb.velocity.x, 0);
                            if (CustomControls.GetButton(secondaryJoystick, buttons[2]))
                            {
                                rb.velocity = new Vector2(speed, rb.velocity.y);
                                xAxis = true;
                                x = 1;

                            }
                            else if (CustomControls.GetButton(secondaryJoystick, buttons[3]))
                            {
                                rb.velocity = new Vector2(-speed, rb.velocity.y);
                                xAxis = true;
                                x = -1;
                            }
                            else rb.velocity = new Vector2(0, rb.velocity.y);
                            if ((xAxis || yAxis) && canAttackButWithMovement == false)
                            {
                                if (online == false)
                                {
                                    UnityEngine.Debug.Log("cancel");
                                    animator.Play("run");
                                }
                                float rotZ = playerRotation.z;
                                playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                if (rb.velocity == new Vector2(0, 0))
                                    playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                            }*/
                        }
                        /*if (x != 0 || y != 0)
                        {
                            if (online && canAttack && networkCanAttack)
                            {
                                FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "run");

                            }
                        }
                        if (joystick != null)
                        {
                            if (CustomControls.GetAxis(joystick).Yaxis < 2tankCharge)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    yAxis = true;
                                    if (online == false)
                                    {
                                        UnityEngine.Debug.Log("cancel");
                                        animator.Play("run");
                                    }
                                    float rotZ = playerRotation.z;
                                    playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                    if (rb.velocity == new Vector2(0, 0))
                                        playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                                }
                            }
                            else if (CustomControls.GetAxis(joystick).Yaxis > 40000)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    yAxis = true;
                                    if (online == false)
                                    {
                                        UnityEngine.Debug.Log("cancel");
                                        animator.Play("run");
                                    }
                                    float rotZ = playerRotation.z;
                                    playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                    if (rb.velocity == new Vector2(0, 0))
                                        playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                                }
                            }
                            if (CustomControls.GetAxis(joystick).Xaxis > 40000)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    xAxis = true;
                                    if (online == false)
                                    {
                                        UnityEngine.Debug.Log("cancel");
                                        animator.Play("run");
                                    }
                                    float rotZ = playerRotation.z;
                                    playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                    if (rb.velocity == new Vector2(0, 0))
                                        playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                                }
                            }
                            else if (CustomControls.GetAxis(joystick).Xaxis < 2tankCharge)
                            {

                                if (canAttackButWithMovement == false)
                                {
                                    xAxis = true;
                                    if (online == false)
                                    {
                                        UnityEngine.Debug.Log("cancel");
                                        animator.Play("run");
                                    }
                                    float rotZ = playerRotation.z;
                                    playerRotation = Quaternion.LookRotation(rb.velocity * 100, new Vector3(0, 0, 1));
                                    playerRotation = new Quaternion(0, 0, playerRotation.z, playerRotation.w);
                                    if (rb.velocity == new Vector2(0, 0))
                                        playerRotation = new Quaternion(0, 0, rotZ, playerRotation.w);
                                }
                            }
                        }
                        if (xAxis == false && yAxis == false)
                        {
                            if (canAttackButWithMovement == false)
                            {
                                if (online == false)
                                    animator.Play("idle");
                                if (online && canAttack && networkCanAttack)
                                {
                                    FindObjectOfType<GameClient>().Send("PlayerAnim" + " " + playerNumber + " " + "idle");
                                }
                            }
                        }
                        if (online)
                        {
                            FindObjectOfType<GameClient>().Send("SetVelocity " + FindObjectOfType<GameClient>().playerId + " " + x + " " + y);
                            if (FindObjectOfType<GameHost>() == null)
                                rb.velocity = new Vector2(0, 0);
                        }*/
                    }
                    #endregion
                }
                else clearCombo = true;

            }
        }
    }
    bool clearCombo = false;
    Vector2 startLoc;

    bool revived = false;

    GameObject hideSpot;

    bool knightBackMove = false;
    void StopKnightBackMove()
    {
        knightBackMove = false;
    }

    float resetHpTimer = 0;
    float oldEulerAngles;
    bool animState_attack2;
    bool iceActive;

    private void LateUpdate()
    {
        if (rendered || bot == false || noAI)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            animState_run = animState.IsName("run");
            animState_idle = animState.IsName("idle");
            animState_hurt1 = animState.IsName("hurt");
            animState_hurt2 = animState.IsName("hurt2");
            animState_attack1 = animState.IsName("attack1");
            animState_attack2 = animState.IsName("attack2");

            if (health <= 0)
                Stats.SetActive(false);
            if (bot && animState_death)
                circleCollider.enabled = false;
            if (online || (keyboard))
            {
                ResetSpeed();

                bool canMove = PlayerCanMove();

                if (canMove)
                {
                    GatherSpeed();
                }

                if (canMove)
                {
                    rb.velocity = myVelocity = Vector2.zero;
                    if (followTarget)
                    {
                        MobFollowTarget();
                    }
                }
            }

            iceActive = ice.activeInHierarchy;

            Vector3 playerPosition = playerTransform.position;

            firstLimbRed = firstLimb.color == new Color32(255, 0, 0, 255);

            if (PublicVariables.TimeScale > 0.01f)
            {
                CheckDeath();
            }
            if (bot && PublicVariables.TimeScale > 0.01f)
            {
                if (noAI && canAttack && ableToMove && (animState_hurt1 == false && animState_hurt2 == false) && iceActive == false && stunned == false && (animState_idle || animState_run || (animState_attack1 && caracterId == 2)))
                {
                    resetHpTimer += PublicVariables.deltaTime;

                    rb.velocity = Vector2.zero;

                    if (resetHpTimer >= 3)
                    {
                        health = maxHealth;
                        healthText.text = ((int)health).ToString();
                        resetHpTimer = 0;
                    }
                }
                else resetHpTimer = 0;

                ResetSpeed();

                if (canAttackButWithMovement && finalBoss)
                {
                    playerTransform.position = Vector2.MoveTowards(playerPosition, targetLocation, speed * 2 * PublicVariables.deltaTime);
                    return;
                }
                if (animState_death == false)
                {
                    dashTimer += PublicVariables.deltaTime;
                    if (finalBoss == false && playerTransform.name.Contains("FinalBoss") == false)
                    {
                        for(int i=0;i<childrenSprites.Length;i++)
                        {
                            SpriteRenderer sprite = childrenSprites[i];

                            Color color = sprite.color;

                            sprite.color = Color32.Lerp(new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), 0), new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), 255), t);
                        }
                    }
                    if (animState_idle)
                    {
                        canAttack = true;
                    }
                    /*if (animState_idle && firstLimb.color == new Color32(255, 0, 0, 255) && online == false)
                    {
                        // StopAttack();
                    }*/

                    if (isBoss && animState_attack2 && caracterId == 3)
                    {
                        if (target == null)
                        {
                            CancelInvoke("GetTarget");
                            GetTarget();
                        }
                        else
                        {
                            Vector2 diff = target.transform.position - playerPosition;
                            diff.Normalize();

                            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                            playerTransform.eulerAngles = new Vector3(0, 0, rot_z + 90);
                        }
                    }

                    //if(finalBoss && animState_attack1)
                    //{
                    //    playerTransform.position = Vector2.MoveTowards(playerTransform.position, targetLocation, speed * 6 * PublicVariables.deltaTime);
                    //}

                    if (canAttack && noAI == false && ableToMove && (animState_hurt1 == false && animState_hurt2 == false) && iceActive == false && stunned == false && (animState_idle || animState_run || (animState_attack1 && caracterId == 2)) && attackDelay == false)
                    {
                        if (aiDestinationSetter.target != null && aiPath.canMove && isBoss)
                        {
                            timeTillAttacked += PublicVariables.deltaTime;
                            speed = 10;
                            if (timeTillAttacked <= 1.5f)
                                speed = speed * (timeTillAttacked * 2 + 1);
                            else speed = speed * 4;
                            aiPath.maxSpeed = speed;
                        }
                        else timeTillAttacked = 0;

                        if (target != null)
                        {
                            if (target.gameObject.activeInHierarchy == false)
                                target = null;
                        }

                        if (finalBoss == false && (animState_attack2 && caracterId == 3) == false)
                        {
                            if (aiDestinationSetter.target != null)
                            {
                                GraphNode node1 = AstarPath.active.GetNearest(playerPosition, NNConstraint.Default).node;
                                GraphNode node2 = AstarPath.active.GetNearest(aiDestinationSetter.target.position, NNConstraint.Default).node;
                                /*if (node1 == null || node2 == null)
                                {
                                    Vector2 diff = aiDestinationSetter.target.position - playerPosition;
                                    diff.Normalize();

                                    float oldRot = playerTransform.localEulerAngles.z;

                                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                                    playerTransform.localEulerAngles = new Vector3(0, 0, rot_z + 90);

                                    rb.velocity = myVelocity = -playerTransform.up * speed;

                                    if ((animState_attack1 && caracterId == 2) == false)
                                    {
                                        animator.Play("run");

                                        /*if (inDungeon == false)
                                            FindObjectOfType<GameHost>().SendAnim("run", playerNumber);*/

                                /*}
                                else
                                {
                                    playerTransform.localEulerAngles = new Vector3(0, 0, oldRot);
                                }

                                aiPath.enabled = false;
                            }*/
                                if (PathUtilities.IsPathPossible(node1, node2))
                                {
                                    MobFollowPlayer();
                                }
                                /*else
                                {
                                    Vector2 diff = aiDestinationSetter.target.position - playerPosition;
                                    diff.Normalize();

                                    float oldRot = playerTransform.localEulerAngles.z;

                                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                                    playerTransform.localEulerAngles = new Vector3(0, 0, rot_z + 90);

                                    rb.velocity = myVelocity = -playerTransform.up * speed;

                                    if ((animState_attack1 && caracterId == 2) == false)
                                    {
                                        animator.Play("run");

                                        /*if (inDungeon == false)
                                            FindObjectOfType<GameHost>().SendAnim("run", playerNumber);*/

                                //}
                                /*else
                                {
                                    playerTransform.localEulerAngles = new Vector3(0, 0, oldRot);
                                }

                                aiPath.enabled = false;
                            }*/
                            }
                        }
                        if (finalBoss)
                        {
                            FinalBossAlgorithm();
                        }
                        else
                        {
                            if (aiPath.enabled)
                                rb.velocity = myVelocity = Vector2.zero;
                            if (t != 1 && myObj.name.Contains("FinalBoss") == false)
                            {
                                t += PublicVariables.deltaTime * botSpawnTime;
                                if (t > 1)
                                {
                                    t = 1;
                                    for(int i=0;i<limbColors.Count;i++)
                                    {
                                        limbColors[i].Start();
                                    }
                                }
                            }
                            else
                            {
                                tBA += PublicVariables.deltaTime;

                                if (caracterId != 5 || isBoss)
                                {
                                    //if (support == false || isBoss)
                                    //{

                                    if (isBoss && bossCurrentAttackIndex == -1)
                                    {
                                        if (botAttacks.Length == 0)
                                            bossCurrentAttackIndex = 0;
                                        else bossCurrentAttackIndex = UnityEngine.Random.Range(1, 99999) % botAttacks.Length;
                                    }
                                    if (bossScene)
                                        bossCurrentAttack = 0;
                                    if (target == null)
                                    {
                                        CancelInvoke("GetTarget");
                                        GetTarget();
                                    }
                                    else if ((animState_attack2 && caracterId == 3) == false)
                                    {
                                        //if (caracterId != 5 || isBoss)
                                        //{
                                        if (tBA >= timeBetweenAttacks && ((isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks()) == false))
                                        {
                                            hideSpot = null;
                                            if (MobCanDashMelee() && isBoss == false)
                                            {
                                                MobDashMelee();
                                            }
                                            else if (CanAttack())
                                            {
                                                if (caracterId == 2 && UnityEngine.Random.Range(1, 999999) % 3 == 0 && canDash && isBoss == false)
                                                {
                                                    KnightDash();
                                                }
                                                else
                                                {
                                                    bool ok = true;

                                                    if (bot && ((caracterId == 1 && isBoss == false) || (caracterId == 3)) && ok)
                                                    {
                                                        if (Mathf.Abs(angularVelocity.z) > 100)
                                                            ok = false;

                                                        RaycastHit2D hit = Physics2D.Linecast(playerPosition, target.transform.position, 1 << LayerMask.NameToLayer("Walls"));

                                                        if (hit.collider != null)
                                                        {
                                                            ok = false;
                                                        }
                                                    }
                                                    if (ok)
                                                        MobAttack();
                                                }
                                            }
                                            /*else if (target != null && (animState_attack1 && caracterId == 2) == false)
                                            {
                                                GetPathToPlayer();
                                            }*/
                                        }
                                        else if (bossScene == false) RunFromPlayer();
                                        //}
                                        /*else
                                        {
                                            if (MobCanDashRanged())
                                            {
                                                MobDashRanged();
                                            }
                                            else if (MobCanDashMelee())
                                            {
                                                MobDashMelee();
                                            }
                                            else if (CanAttack())
                                            {
                                                if (caracterId == 2 && UnityEngine.Random.Range(1, 999999) % 3 == 0 && canDash)
                                                {
                                                    KnightDash();
                                                }
                                                else
                                                {
                                                    MobAttack();
                                                }
                                            }
                                            else
                                            {
                                                if (aiPath.enabled)
                                                    rb.velocity = Vector2.zero;

                                                aiPath.maxSpeed = speed;
                                                //animator.Play("run");
                                                aiPath.canMove = true;
                                            }
                                        }*/

                                    }
                                    //}
                                    /*else
                                    {
                                        if (mobToRevive != null && canRevive)
                                        {
                                            MedicRevive();
                                        }
                                        else
                                        {
                                            if (mobToHeal != null && canHeal)
                                            {
                                                MedicHeal();
                                            }
                                            else
                                            {
                                                RunFromPlayer();
                                            }
                                        }
                                    }*/
                                }
                                else
                                {
                                    if (mobToRevive != null && canRevive)
                                    {
                                        MedicRevive();
                                    }
                                    else
                                    {
                                        if (mobToHeal != null && canHeal)
                                        {
                                            MedicHeal();
                                        }
                                        else if (bossScene == false)
                                        {
                                            RunFromPlayer();
                                        }
                                    }
                                }
                                //else
                                //{
                                //    aiPath.canMove = false;
                                //}
                            }
                        }
                        if (aiPath.canMove && ((animState_attack1 && caracterId == 2) == false) && ((animState_attack2 && caracterId == 3) == false))
                        {
                            if (aiPath.desiredVelocity != Vector3.zero)
                            {
                                animator.Play("run");
                            }
                            else if (aiPath.enabled) animator.Play("idle");
                        }
                    }
                    else
                    {
                        aiPath.canMove = false;
                    }
                }
                else
                {
                    aiPath.canMove = false;
                    Stats.SetActive(false);
                }
            }
            if (healthTextNotNull)
            {
                Transform healthTextTransform = healthText.transform;

                healthTextTransform.eulerAngles = new Vector3(0, 0, 0);
                healthTextTransform.position = playerPosition + new Vector3(0, (caracterId == 4 ? 1.44f : 1) * 2, 0);
                stunObj.transform.position = healthTextTransform.position + new Vector3(0, 0.75F);
            }
            if (statsNotNull)
            {
                Stats.transform.eulerAngles = Vector3.zero;
            }
            if (fireParticles != null)
                fireParticles.transform.eulerAngles = Vector3.zero;
            if (parkourRoom)
            {
                speed = 7.5f;

                aiPath.speed = speed;
            }
            if (bot == false && isBoss == false && finalBoss == false)
                if (helperLine.activeInHierarchy)
                {
                    if (gamePad != null)
                    {
                        if (gamePad.leftStick != Vector2.zero)
                            helperLine.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(gamePad.leftStick.y, gamePad.leftStick.x) * Mathf.Rad2Deg + 90);
                    }
                    else
                    {
                        if (phoneController.dir != Vector2.zero)
                            helperLine.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(phoneController.dir.y, phoneController.dir.x) * Mathf.Rad2Deg + 90);
                    }
                }
        }
    }

    float followPlayer = 0;
    float followHideSpot = 0;
    bool followingPlayer = true;
    float maxFollow;
    GameObject playerHideSpot;
    private void MobFollowPlayer()
    {
        aiPath.canMove = true;
        aiPath.enabled = true;
        if (bot == false)
            rb.velocity = myVelocity = Vector2.zero;
        if (followingPlayer || bossScene)
        {
            if (target != null)
                aiDestinationSetter.target = target.transform;
            followPlayer += PublicVariables.deltaTime;
            if (followPlayer > maxFollow)
            {
                followingPlayer = false;
                followPlayer = 0f;
                maxFollow = UnityEngine.Random.Range(1.25f, 3f);
            }
        }
        else
        {
            followHideSpot += PublicVariables.deltaTime;
            if (playerHideSpot == null && target != null)
            {
                if (parentRoom == null)
                    playerHideSpot = shopSpawn.GetHideSpotClosest(playerTransform.position, target.transform.position);
                else playerHideSpot = parentRoom.GetHideSpotClosest(playerTransform.position, target.transform.position);
            }
            if (playerHideSpot != null)
                aiDestinationSetter.target = playerHideSpot.transform;
            if (followHideSpot > maxFollow || aiPath.reachedEndOfPath)
            {
                followingPlayer = true;
                followHideSpot = 0f;
                playerHideSpot = null;
                maxFollow = UnityEngine.Random.Range(1.25f, 3f);
            }
        }
    }

    private void GetPathToPlayer()
    {
        if (bot)
        {
            if (randomAroundTarget == null)
                randomAroundTarget = new GameObject().AddComponent<RandomTarget>();
            if (MathUtils.CompareDistances(playerTransform.position, target.transform.position, distanceFromPlayer + 4f, MathUtils.CompareTypes.LessThan))
            {
                aiDestinationSetter.target = target.transform;
            }
            else
            {
                if (randomAroundTarget.target != null)
                {
                    if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(playerTransform.position).node, AstarPath.active.GetNearest(randomAroundTarget.target.position).node) == false)
                        randomAroundTarget.target = null;
                }
                if ((randomAroundTarget.target == null || aiDestinationSetter.target != randomAroundTarget.transform) && (target != null ? target != randomAroundTarget.target : true))
                {

                    var gg = AstarPath.active.data.gridGraph;

                    List<GridNode> nodes = new List<GridNode>();

                    for(int i=0;i<gg.nodes.Length;i++)
                    {
                        GridNode node = gg.nodes[i];
                        if (Vector2.Distance(target.transform.position, (Vector3)(node.position)) < distanceFromPlayer + 2.5f && PathUtilities.IsPathPossible(AstarPath.active.GetNearest(playerTransform.position).node, node))
                        {
                            nodes.Add(node);
                        }
                    }

                    if (nodes.Count != 0)
                    {
                        randomAroundTarget.transform.position = (Vector3)nodes[UnityEngine.Random.Range(1, 99999) % nodes.Count].position;

                        randomAroundTarget.target = target.transform;

                        aiDestinationSetter.target = randomAroundTarget.transform;
                    }

                }
            }
        }
    }

    private void OnDestroy()
    {

    }

    private void OnDisable()
    {

    }

    private void RunFromPlayer()
    {
        if (target == null)
        {
            CancelInvoke("GetTarget");
            GetTarget();
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") && caracterId == 2)
        {
            aiPath.enabled = true;
            aiPath.canMove = true;
            aiDestinationSetter.target = target.transform;
        }
        else if (Vector2.Distance(playerTransform.position, target.transform.position) < (parkourRoom ? 1000 : (isBoss ? 20 : (distanceFromPlayer)) + (caracterId == 4 ? 0 : 2) + (caracterId == 5 ? 6 : 0)) || knightBackMove == true)
        {
            animator.Play("run");

            if (hideSpot == null)
            {
                if (parentRoom == null)
                    hideSpot = shopSpawn.GetHideSpot(target);
                else hideSpot = parentRoom.GetHideSpot(target);
            }

            aiPath.enabled = true;
            aiPath.canMove = true;
            aiDestinationSetter.target = hideSpot.transform;

            if (Vector2.Distance(playerTransform.position, hideSpot.transform.position) < 0.5f)
            {
                if (parentRoom == null)
                    hideSpot = shopSpawn.GetHideSpot(target, hideSpot);
                else hideSpot = parentRoom.GetHideSpot(target, hideSpot);
            }

            if (knightBackMove == false)
            {
                knightBackMove = true;

                Invoke("StopKnightBackMove", 1f);
            }
        }
        else
        {
            animator.Play("idle");
            aiPath.canMove = false;
            aiDestinationSetter.target = null;
        }
    }

    private void CheckDeath()
    {
        try
        {
            if (animState_death == false)
            {
                if (health <= 0)
                {
                    if (bot == false && inDungeon && finalBoss == false)
                    {
                        foreach (SelectableItem itemSlot in inventorySpawn.itemSlots)
                        {
                            if (itemSlot.containsItem)
                            {
                                if (itemSlot.itemType == 7)
                                {
                                    Heal(350);
                                    itemSlot.usesLeft--;
                                    if (itemSlot.usesLeft <= 0)
                                        itemSlot.RemoveItem();
                                    else itemSlot.abilityDescription.GetComponentInParent<AbilityDesc>().abilityText.text = inventorySpawn.itemDescs[7] + " Remaining uses: <color=red><b>" + itemSlot.usesLeft + "</b></color>.";

                                    DungeonData.instance.AddToQuest(31, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (health <= 0)
                {
                    StopIce();
                    CancelInvoke("FireParticles");
                    fireParticles.SetActive(false);
                    CancelInvoke("PoisonParticles");
                    poisonParticles.SetActive(false);
                    CancelInvoke("StopStun");
                    StopStun();
                    if (speedObjNotNull)
                    {
                        CancelInvoke("StopSpeed");
                        StopSpeed();
                    }
                    CancelInvoke("StopSlow");
                    StopSlow();
                    if (mobAuraNotNull)
                        mobAura.SetActive(false);
                    if (silenceObjNotNull)
                    {
                        CancelInvoke("StopSilence");
                        StopSilence();
                    }
                    if (weakenObjNotNull)
                    {
                        CancelInvoke("StopWeaken");
                        StopWeaken();
                    }
                    health = 0;
                    hpBarTransform.localScale = new Vector2(0, 1);
                    if (botInShops && bot)
                        healthText.text = ((int)health).ToString();
                    if (bot && finalBoss == false)
                    {
                        if (isBoss)
                            bossRoomScript.BossKillMobs(lastHit);
                        if (revived == false && botInShops == false && bossScene == false)
                        {
                            int value = 7 + UnityEngine.Random.Range(-3, 3);
                            if (isBoss)
                                value *= 10;
                            if (SceneName == "TutorialScene")
                                value *= 5;
                            inventorySpawn.AddCoins(value);
                            GameObject vfx = Instantiate(coinVfx); vfx.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                            vfx.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();
                            if (caracterId == 4)
                            {
                                vfx.transform.position = new Vector2(playerTransform.position.x + 4.52f, playerTransform.position.y);
                            }
                            else vfx.transform.position = new Vector2(playerTransform.position.x + 4.12f, playerTransform.position.y);
                            vfx.transform.parent = Camera.main.transform;
                        }
                        aiPath.canMove = false;
                        aiDestinationSetter.target = null;
                    }
                    else if (inDungeon && finalBoss == false)
                    {
                        bool ok = false;
                        CharacterSlot playerSlot = null;
                        foreach (CharacterSlot slot in inventorySpawn.inventory.GetComponent<Inventory>().slots)
                        {
                            if (slot.characterId == caracterId)
                            {
                                playerSlot = slot;
                                ok = true;
                                break;
                            }
                        }
                        if (ok)
                        {
                            playerSlot.xpBar.transform.localScale = new Vector3(xp / (100 + lvl * 25), playerSlot.xpBar.transform.localScale.y, 1);
                            playerSlot.characterXp.text = xp + "/" + 100 + lvl * 25;
                            playerSlot.characterLevel.text = lvl.ToString();
                            playerSlot.APText.text = "AP: " + abilityPoints;
                            playerSlot.MPText.text = "MP: " + masterPoints;
                        }
                    }
                    Stats.SetActive(false);
                    rb.velocity = myVelocity = new Vector2(0, 0);
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    if (bot && finalBoss == false)
                    {
                        if (UnityEngine.Random.value > 0.9 && revived == false && botInShops == false)
                        {
                            dungeonData.RemoveCoins(-1);
                        }
                        foreach (support Player in FindObjectsOfType<support>())
                        {
                            if (Player.GetComponent<player>().bot && Player.GetComponent<player>().mobToRevive == null && Player.gameObject != myObj)
                            {
                                Player.GetComponent<player>().mobToRevive = myObj;
                                break;
                            }
                        }
                    }
                    if (finalBoss)
                    {
                        animator.Play("death");
                        FindObjectOfType<FinalBossScene>().InvokeNextFaze();
                        CancelInvoke("Expl");
                        foreach (borderExpl border in FindObjectsOfType<borderExpl>())
                        {
                            border.StopBorder();
                        }
                        GameObject.Find("Borders").SetActive(false);
                    }
                    else
                    {

                        animator.Play("death");
                        circleCollider.enabled = false;
                    }
                }
            }
        }
        catch
        {

        }
    }

    private void MobFollowTarget()
    {
        float oldRot = playerTransform.localEulerAngles.z;

        Vector2 diff = cursorTarget.transform.position - playerTransform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        playerTransform.localEulerAngles = new Vector3(0, 0, rot_z + 90);

        if (movementToggled)
        {
            rb.velocity = myVelocity = -playerTransform.up * speed;
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") && caracterId == 2) == false)
            {
                animator.Play("run");

                /*if (inDungeon == false)
                    FindObjectOfType<GameHost>().SendAnim("run", playerNumber);*/

            }
            else
            {
                playerTransform.localEulerAngles = new Vector3(0, 0, oldRot);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") && caracterId == 2))
                playerTransform.localEulerAngles = new Vector3(0, 0, oldRot);
            else animator.Play("idle");
        }


        if (Vector2.Distance(playerTransform.position, cursorTarget.transform.position) < 0.5f)
        {
            followTarget = false;
        }
    }

    private void GatherSpeed()
    {
        if (movementToggled == false)
            timeTillAttacked = 0;
        if ((rb.velocity != Vector2.zero || followTarget) && online == false && inDungeon)
        {
            if (timeTillAttacked <= 1.5f && bot == false)
                speed = speed * (timeTillAttacked * 2 + 1);
            else if (bot == false) speed = speed * 4;
        }
    }

    private bool PlayerCanMove()
    {

        return (canAttack || canAttackButWithMovement) && (animState_hurt1 == false && animState_hurt2 == false) && ice.activeInHierarchy == false && stunned == false && (animState_idle || animState_run || (animState_attack1 && caracterId == 2));
    }

    private void ResetSpeed()
    {
        if (speedObjNotNull)
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else if (speedObjActive)
                speed = bonusSpeedValue;
            else speed = originalSpeed;
        }
        else
        {
            if (slowParticles.activeInHierarchy)
                speed = originalSpeed / 6;
            else if (poisonParticles.activeInHierarchy)
                speed = originalSpeed / 3;
            else speed = originalSpeed;
        }
        if (aiPath != null)
            aiPath.maxSpeed = speed;
    }

    private void MedicRunFromPlayer()
    {
        if (playerDetect.GetComponent<playerDetect>().player != null)
            target = playerDetect.GetComponent<playerDetect>().player;
        else
        {
            target = null;
            tBA = timeBetweenAttacks;
            //randomAroundTarget.target = null;
        }
        if (target != null)
        {
            if (Vector2.Distance(playerTransform.position, target.transform.position) < 2.75f && tBA >= timeBetweenAttacks * 2 && canCounterAttack)
            {
                tBA = timeBetweenAttacks;

                //randomAroundTarget.target = null;

                Vector3 diff = target.transform.position - playerTransform.position;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
                animator.Play("attack7");
            }
            else
            {
                Vector2 newestPos = Vector2.MoveTowards(playerTransform.position, target.transform.position, speed * PublicVariables.deltaTime);

                Vector2 diff = (Vector2)playerTransform.position - newestPos;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                playerTransform.localEulerAngles = new Vector3(0, 0, rot_z - 90);

                /*Collider2D[] boxesX = Physics2D.OverlapBoxAll(new Vector2(((Vector2)(playerTransform.position) + (Vector2)(playerTransform.position) - newestPos).x, playerTransform.position.y), new Vector2(0.01f, 0.01f), 0f);

                Collider2D[] boxesY = Physics2D.OverlapBoxAll(new Vector2(playerTransform.position.x, ((Vector2)(playerTransform.position) + (Vector2)(playerTransform.position) - newestPos).y), new Vector2(0.01f, 0.01f), 0f);
                bool okX = false;
                bool okY = false;

                foreach (Collider2D box in boxesX)
                {
                    if (box.GetComponent<room>() != null)
                    {
                        okX = true;
                    }
                    if (box.GetComponent<Void>() != null)
                    {
                        okX = false;
                        break;
                    }
                }
                foreach (Collider2D box in boxesY)
                {
                    if (box.GetComponent<room>() != null)
                    {
                        okY = true;
                    }
                    if (box.GetComponent<Void>() != null)
                    {
                        okY = false;
                        break;
                    }
                }

                Vector2 vel = Vector2.zero;

                if (okX)
                {
                    vel = new Vector2(vel.x, playerTransform.up.y);
                }
                if (okY)
                {
                    vel = new Vector2(playerTransform.up.x, vel.y);
                }*/

                rb.velocity = myVelocity = playerTransform.up * speed;

                //if (okX || okY)
                //{
                animator.Play("run");
                //}

                aiPath.enabled = false;
            }
        }
        else
        {
            //animator.Play("idle");
        }
    }

    private void MedicHeal()
    {
        if (Vector2.Distance(playerTransform.position, mobToHeal.transform.position) < 1f)
        {
            Vector3 diff = mobToHeal.transform.position - playerTransform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
            animator.Play("attack9");
            mobToHeal = null;
            if (bot)
            {
                //randomAroundTarget.target = null;
            }
        }
        else
        {
            aiDestinationSetter.target = mobToHeal.transform;
            if (aiPath.enabled)
                rb.velocity = myVelocity = Vector2.zero;


            aiPath.maxSpeed = speed;
            //animator.Play("run");
            aiPath.canMove = true;
        }
    }

    private void MedicRevive()
    {
        if (Vector2.Distance(playerTransform.position, mobToRevive.transform.position) < 1f)
        {
            Vector3 diff = mobToHeal.transform.position - playerTransform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
            animator.Play("attack9");
            if (bot)
            {
                //randomAroundTarget.target = null;
            }
        }
        else
        {
            aiDestinationSetter.target = mobToRevive.transform;
            if (aiPath.enabled)
                rb.velocity = myVelocity = Vector2.zero;


            aiPath.maxSpeed = speed;
            //animator.Play("run");
            aiPath.canMove = true;
        }
    }

    private void FinalBossAlgorithm()
    {
        if (currentAttack == 6)
        {
            if (target == null)
            {
                List<player> players = new List<player>();
                foreach (player player in cameraFollow.playerScripts)
                {
                    if (player != null)
                    {
                        if (player.bot == false)
                        {
                            players.Add(player);
                        }
                    }
                }
                target = players[UnityEngine.Random.Range(1, 999999) % players.Count].gameObject;
                aiDestinationSetter.target = target.transform;
            }
            else
            {
                if (Vector2.Distance(playerTransform.position, target.transform.position) < distanceFromPlayer)
                {
                    aiPath.canMove = false;
                    string attack = "botBasicAttack";
                    Vector3 diff = target.transform.position - playerTransform.position;
                    diff.Normalize();

                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
                    animator.Play(attack);
                }

                else
                {
                    rb.velocity = myVelocity = Vector2.zero;

                    oldPos2 = newPos2;
                    newPos2 = playerTransform.position;
                    Vector3 diff = newPos2 - oldPos2;
                    diff.Normalize();

                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
                    playerTransform.position = Vector2.MoveTowards(playerTransform.position, target.transform.position, speed * PublicVariables.deltaTime);
                    animator.Play("run");

                }

            }
            return;
        }
        if (currentAttack == 0)
        {
            if (consecutiveAttacks >= 2)
            {
                consecutiveAttacks = 0;
                currentAttack = UnityEngine.Random.Range(1, 9999999) % 3 + 4;
                consecutiveAttacks = 0;
            }
            else
            {
                currentAttack = UnityEngine.Random.Range(1, 9999999) % 3 + 1;
                if (currentAttack == 1)
                {
                    currentFireballCount = 0;
                    maxFireballCount = UnityEngine.Random.Range(6, 14);
                }
                else
                {
                    timesAttacked = 0;
                    if(currentAttack == 3)
                        maxTimesAttacked = UnityEngine.Random.Range(6, 10);
                    else maxTimesAttacked = UnityEngine.Random.Range(6, 14);
                }
                consecutiveAttacks++;
            }
            fireball = false;
        }
        if (currentAttack == 1)
        {
            if (fireball == false)
            {
                teleportLocation = new Vector2(UnityEngine.Random.Range(minTpLoc.transform.position.x, maxTpLoc.transform.position.x), UnityEngine.Random.Range(minTpLoc.transform.position.y, maxTpLoc.transform.position.y));

                Vector2 dif = teleportLocation - (Vector2)playerTransform.position;
                dif.Normalize();
                float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                playerTransform.localEulerAngles = new Vector3(0, 0, rot + 90);

                animator.Play("boss2");

                fireball = true;
            }
            else fireball = false;
        }
        else if (currentAttack == 2)
        {
            targetLocation = new Vector2(UnityEngine.Random.Range(minTpLoc.transform.position.x, maxTpLoc.transform.position.x), UnityEngine.Random.Range(minTpLoc.transform.position.y, maxTpLoc.transform.position.y));

            Vector2 dif = targetLocation - (Vector2)playerTransform.position;
            dif.Normalize();
            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            playerTransform.eulerAngles = new Vector3(0, 0, rot + 90);

            animator.Play("attack2");

            timesAttacked++;

            if (timesAttacked > maxTimesAttacked)
                currentAttack = 0;
        }
        else if (currentAttack == 3)
        {
            startLoc = playerTransform.position;
            targetLocation = new Vector2(UnityEngine.Random.Range(minTpLoc.transform.position.x, maxTpLoc.transform.position.x), UnityEngine.Random.Range(minTpLoc.transform.position.y, maxTpLoc.transform.position.y));

            Vector2 dif = targetLocation - (Vector2)playerTransform.position;
            dif.Normalize();
            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            playerTransform.localEulerAngles = new Vector3(0, 0, rot + 90);

            animator.Play("attack1");

            timesAttacked++;

            if (timesAttacked > maxTimesAttacked / 2)
                currentAttack = 0;
        }
        else if (currentAttack == 4)
        {
            animator.Play("boss5");
        }
        else if (currentAttack == 5)
        {
            animator.Play("boss3");
        }
        else if (currentAttack == 6)
        {
            bar.SetActive(false);

            for (int i = 0; i <= 4; i++)
            {
                GameObject newMob = Instantiate(bossCopy);
                newMob.transform.position = new Vector2(UnityEngine.Random.Range(minTpLoc.transform.position.x, maxTpLoc.transform.position.x), UnityEngine.Random.Range(minTpLoc.transform.position.y, maxTpLoc.transform.position.y));
                mobs.Add(newMob);
                newMob.SetActive(true);
            }

            Invoke("KillMobs", 10f);

            playerTransform.position = new Vector2(UnityEngine.Random.Range(minTpLoc.transform.position.x, maxTpLoc.transform.position.x), UnityEngine.Random.Range(minTpLoc.transform.position.y, maxTpLoc.transform.position.y));

        }
    }

    private void ResetBossAttacks()
    {
        bossCurrentAttack = 0;
    }

    bool attackDelay = false;

    string prevAttack = "";

    private void MobAttack()
    {
        if (parkourRoom)
            speed = originalSpeed;
        tBA = 0;
        //randomAroundTarget.target = null;
        aiPath.canMove = false;
        string attack = "";

        List<string> botAttacksList = botAttacks.ToList();

        if (caracterId == 1 && target.GetComponent<player>().ice.activeInHierarchy)
            botAttacksList.Remove("attack5");

        attack = botAttacksList[UnityEngine.Random.Range(1, 999999) % botAttacksList.Count];

        if (isBoss)
        {
            List<string> possibleAttacks = new List<string>();

            float dist = Vector2.Distance(target.transform.position, playerTransform.position);

            UnityEngine.Debug.Log(dist);

            for (int i = 0; i < botAttacks.Length; i++)
            {
                if (dist <= botAttackMinDistances[i])
                {
                    for (int j = i; j < botAttacks.Length; j++)
                    {
                        if (botAttackMinDistances[i] == botAttackMinDistances[j])
                        {
                            possibleAttacks.Add(botAttacks[j]);
                        }
                    }
                    break;
                }
            }

            possibleAttacks.Remove(prevAttack);

            if (caracterId == 5 && shielded)
                possibleAttacks.Remove("attack5");

            if (caracterId == 4 && target.GetComponent<player>().slowed)
                possibleAttacks.Remove("attack5");

            if (possibleAttacks.Count != 0)
            attack = possibleAttacks[UnityEngine.Random.Range(1, 99999) % possibleAttacks.Count];
        }
        if (attack != "")
        {
            Vector3 diff = target.transform.position - playerTransform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);

            UnityEngine.Debug.Log(attack);

            if ((attack == "attack6" && caracterId == 1) || (attack == "attack2" && caracterId == 5))
            {
                animator.Play(attack);
                animator.speed = 0.75f;
            }
            else if (attack == "attack4" && caracterId == 5)
            {
                animator.Play(attack);
                animator.speed = 0.4f;
            }
            else
            {
                animator.Play(attack);
                animator.speed = 1f;
            }

            prevAttack = attack;

            bossCurrentAttack++;
            if (isBoss && bossCurrentAttack > OMEGA.Data.GetBossMaxAttacks())
                Invoke("ResetBossAttacks", 5f);
        }
    }

    IEnumerator AttackDelay(float delay, string attack)
    {
        animator.Play("idle");
        canAttack = false;
        attackDelay = true;
        float t = 0;
        while (t < delay)
        {
            aiPath.enabled = false;
            t += PublicVariables.deltaTime;
            yield break;
        }
        aiPath.enabled = true;
        attackDelay = false;
        animator.Play(attack);
        yield return null;
    }

    private void KnightDash()
    {
        aiPath.canMove = false;
        string attack = dashName;
        Vector3 diff = target.transform.position - playerTransform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        animator.Play(attack);
    }

    private bool CanAttack()
    {
        if (isBoss)
        {
            if (bossScene)
                return animator.GetCurrentAnimatorStateInfo(0).IsName("run");
            else return animator.GetCurrentAnimatorStateInfo(0).IsName("run");
        }
        else return Vector2.Distance(playerTransform.position, target.transform.position) < distanceFromPlayer && animator.GetCurrentAnimatorStateInfo(0).IsName("run");
    }

    private void MobDashMelee()
    {
        aiPath.canMove = false;
        string attack = dashName;
        Vector3 diff = target.transform.position - playerTransform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        dashTimer = 0;
        animator.Play(attack);
    }

    private bool MobCanDashMelee()
    {
        return Vector2.Distance(playerTransform.position, target.transform.position) < dashDistanceFromPlayer && dashTimer >= timeTillDash && animator.GetCurrentAnimatorStateInfo(0).IsName("run") && Vector2.Distance(playerTransform.position, target.transform.position) > distanceFromPlayer && ranged == false && canDash;
    }

    private void MobDashRanged()
    {
        int dir = UnityEngine.Random.Range(1, 9999999) % 3;
        aiPath.canMove = false;
        string attack = dashName;
        Vector3 diff = target.transform.position - playerTransform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        if (dir == 0)
        {
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);
        }
        else if (dir == 1)
        {
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        }
        else if (dir == 2)
        {
            playerTransform.rotation = Quaternion.Euler(0f, 0f, rot_z + 270);
        }
        dashTimer = 0;
        animator.Play(attack);
    }

    private bool MobCanDashRanged()
    {
        return Vector2.Distance(playerTransform.position, target.transform.position) < dashDistanceFromPlayer && ranged && animator.GetCurrentAnimatorStateInfo(0).IsName("run") && dashTimer >= timeTillDash && canDash;
    }

    public float timeBetweenAttacks;
    public float tBA = 0;
    public Vector2 oldPos2;
    public Vector2 newPos2;
    public float distanceFromPlayer;
    public float dashDistanceFromPlayer;
    public GameObject target;
    public float botSpawnTime = 1;
    public bool ranged = false;
    public string dashName;
    public float dashTimer;
    public float timeTillDash;
    public float attackSpeed;
    public bool support = false;
    public GameObject mobToRevive;
    public GameObject mobToHeal;
    public GameObject playerDetect;
    public bool canDash = false;
    public int level;
    public GameObject cursorTarget;
    public float maxXp;
    private Coroutine lastKnockbackCoroutine;
}
