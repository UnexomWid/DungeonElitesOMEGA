using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class hitbox : MonoBehaviour
{
    public GameObject thirdPhase;

    public bool destroyInstantly = false;

    public bool doImpactOnlyOnPlayers = true;
    public bool impactInRadius = false;
    public float impactRadius = 0f;
    public Collider2D collider;
    public bool saveParticlesWhenDestroyed = false;
    public float damage;
    public List<GameObject> players;
    public float knockBackSpeed;
    public GameObject parent;
    public GameObject secondaryParent;
    public bool canDie = false;
    public bool playerDirection = false;
    public bool limbDirection = false;
    public bool secondaryPlayerDirection = false;
    public bool doImpact = false;
    public GameObject impactVfx;
    public bool fire;
    public bool ice;
    public bool electricity;
    public GameObject electricalBolt;
    public bool stun;
    public float stunTime;
    public bool poison;
    public bool poison_pool;
    public GameObject poisonPool;
    public bool doDamage = true;
    public bool slow;
    public float slowTime;
    public bool heal;
    public float healAmmount;
    public bool shield;
    public bool applyParentDamage = false;
    public AudioClip impactSfx;
    public bool shakeWhenActivated = false;
    public bool shakeWhenHit = false;
    public float shakeAmp = 0.25f;
    public bool useRBVelForParticles = false;
    public bool sfxWhenHitMob;

    Dispenser dispenser;

    player parentScript;

    private void OnDestroy()
    {
        if(parentScript != null)
        parentScript.ClearComboList(players);
    }

    public void DestroyObject(Collider2D collision)
    {
        if (poison)
        {
            PoisonPool();
        }
        if (impactVfx != null && doImpact == true)
        {
            GameObject vfx = Instantiate(impactVfx);
            vfx.transform.position = collision.ClosestPoint(myTransform.position);
            ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;
            Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

            //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(-angle - 180, 90, 0);
            doImpact = false;
            Destroy(vfx, 2);
            AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
            if (shakeWhenHit)
                CameraFollow.instance.Shake(0.15f, shakeAmp);
        }
        if (saveParticlesWhenDestroyed)
        {
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.transform.parent = null;
                ps.transform.localScale = new Vector2(1, 1);
                Destroy(ps.gameObject, 2f);
            }
        }
            Destroy(myObj);
    }

    wizard parentWizard;
    bool parentIsWizard;
    knight parentKnight;
    bool parentIsKnight;
    archer parentArcher;
    bool parentIsArcher;
    Tank parentTank;
    bool parentIsTank;
    support parentMedic;
    bool parentIsMedic;

    GameObject myObj;
    InventorySpawn inventorySpawn;
    Transform myTransform;

    List<GameObject> combodPlayers = new List<GameObject>();

    void Start()
    {
        myObj = gameObject;
        myTransform = transform;

        collider = GetComponent<Collider2D>();

        inventorySpawn = FindObjectOfType<InventorySpawn>();

        parentScript = parent.GetComponent<player>();

        try
        {
            dispenser = parent.GetComponent<Dispenser>();
        }
        catch
        {

        }

        if (poison)
        {
            Invoke("PoisonPool", 0.5f);
        }

        inDungeon = SceneManager.GetActiveScene().name.Contains("SampleScene") == false;

        if (canDie && inDungeon && destroyInstantly == false)
        {
            Collider2D[] boxes = Physics2D.OverlapBoxAll(myTransform.position, new Vector2(5, 5), 0f);

            bool ok = false;

            Collider2D closestRoom = null;
            float dist = 0f;

            foreach (Collider2D box in boxes)
            {
                if (box != collider && (box is EdgeCollider2D) == false)
                {

                    if (Vector2.Distance(myTransform.position, box.ClosestPoint(myTransform.position)) < 0.33f || box.transform.name.Contains("spawnRoom") || box.transform.name.Contains("voidRoom") || box.transform.name.Contains("bossRoom"))
                    {
                        ok = true;
                    }
                    if(Vector2.Distance(myTransform.position, box.ClosestPoint(myTransform.position)) < dist || closestRoom == null)
                    {
                        closestRoom = box;
                        dist = Vector2.Distance(myTransform.position, box.ClosestPoint(myTransform.position));
                    }
                }
            }
            if (ok == false)
            {
                Impact(closestRoom);
            }
            /*Debug.Log(boxes.Length);

            if ((boxes.Length == 1 && boxes.Contains(collider)) || (boxes.Length == 0 && boxes.Contains(collider) == false))
            {
                Debug.Log(boxes[0].transform.name);
                Impact();
            }*/
        }
    }

    bool inDungeon = false;

    bool donePoison = false;
    void PoisonPool()
    {
        if (donePoison == false)
        {
            GameObject pool = Instantiate(poisonPool);
            pool.transform.position = myTransform.position;
            pool.GetComponent<PoisonPoolGrow>().active = true;
            pool.GetComponent<hitbox>().parent = parent;
            Destroy(pool, 1.5f);
            if (saveParticlesWhenDestroyed)
            {
                ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.parent = null;
                    ps.transform.localScale = new Vector2(1, 1);
                    Destroy(ps.gameObject, 2f);
                }
            }
            Destroy(myObj);
            donePoison = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            parentScript = parent.GetComponent<player>();

            parentWizard = parentScript.wizard;
            parentKnight = parentScript.knight;
            parentArcher = parentScript.archer;
            parentTank = parentScript.tank;
            parentMedic = parentScript.supportClass;

            if (parentWizard != null)
                parentIsWizard = true;
            if (parentKnight != null)
                parentIsKnight = true;
            if (parentArcher != null)
                parentIsArcher = true;
            if (parentTank != null)
                parentIsTank = true;
            if (parentMedic != null)
                parentIsMedic = true;
        }
        catch
        {
            parentScript = null;
        }

        GameObject collisionGameobject = collision.gameObject;

        Nexus nexus = collisionGameobject.GetComponent<Nexus>();
        DestroyBullets destroyBullets = collisionGameobject.GetComponent<DestroyBullets>();
        EdgeCollider2D edgeCollider = collisionGameobject.GetComponent<EdgeCollider2D>();
        Chest chest = collisionGameobject.GetComponent<Chest>();
        Void voidObj = collisionGameobject.GetComponent<Void>();
        player hitPlayer = collisionGameobject.GetComponent<player>();
        if (nexus != null && ((parentScript.isBoss && parentScript.bossScene) == false))
        {
            nexus.DecreaseHp(damage);
        }
        if (destroyBullets || edgeCollider != null || chest != null)
        {
            if (destroyInstantly)
            {
                Destroy(myObj);
                return;
            }
            if (poison)
            {
                PoisonPool();
            }
            if (impactVfx != null && doImpact == true)
            {
                GameObject vfx = Instantiate(impactVfx);
                vfx.transform.position = collision.ClosestPoint(myTransform.position);
                ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;
                Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

                //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                shape.rotation = new Vector3(-angle-180, 90, 0);
                doImpact = false;
                Destroy(vfx, 2);
                AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                if (shakeWhenHit)
                    CameraFollow.instance.Shake(0.15f, shakeAmp);
            }
            if (chest != null && dispenser == null)
            {
                chest.DestroyChest();
            }
            if (edgeCollider != null && canDie)
            {
                if (saveParticlesWhenDestroyed)
                {
                    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.transform.parent = null;
                        ps.transform.localScale = new Vector2(1, 1);
                        Destroy(ps.gameObject, 2f);
                    }
                }
                Destroy(myObj);
            }
        }
        if (voidObj != null)
        {
            if (parentScript.bot == false)
            {
                float ogDamage = damage;
                if (applyParentDamage)
                {
                    if (parentIsWizard)
                        ogDamage += parentWizard.staffDamage;
                    else if (parentIsKnight)
                        ogDamage += parentKnight.weaponDamage;
                    else if (parentIsTank)
                        ogDamage += parentTank.weaponDamage;
                    else if (parentIsMedic)
                        ogDamage += parentMedic.weaponDamage;
                }
                if(doDamage)
                    voidObj.DecreaseHp(ogDamage, parentScript);
            }
            if (poison)
            {
                PoisonPool();
            }
            if (impactVfx != null && doImpact == true)
            {
                GameObject vfx = Instantiate(impactVfx);
                vfx.transform.position = collision.ClosestPoint(myTransform.position);
                doImpact = false;
                ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;
                Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

                //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                shape.rotation = new Vector3(-angle - 180, 90, 0);
                Destroy(vfx, 2);

                AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                if (shakeWhenHit)
                    CameraFollow.instance.Shake(0.15f, shakeAmp);
            }
            if (canDie)
            {
                if (saveParticlesWhenDestroyed)
                {
                    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.transform.parent = null;
                        ps.transform.localScale = new Vector2(1, 1);
                        Destroy(ps.gameObject, 2f);
                    }
                }
                Destroy(myObj);
            }
        }

        if (hitPlayer != null && (parent != null ? collisionGameobject != parent : false))
        {
            player playerScript = hitPlayer;
            if(dispenser != null)
            {
                if (playerScript.shielded)
                    playerScript.BreakShield();
                else playerScript.DecreaseHp(damage, -myTransform.up * knockBackSpeed, null);
                if (saveParticlesWhenDestroyed)
                {
                    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.transform.parent = null;
                        ps.transform.localScale = new Vector2(1, 1);
                        Destroy(ps.gameObject, 2f);
                    }
                }
                Destroy(myObj);
            }
            else if(thirdPhase != null)
            {
                playerScript.DecreaseHp(damage, -myTransform.up * knockBackSpeed, null);
                if (fire)
                {
                    playerScript.SetOnFire(parentScript.str + parentScript.bonusStrPoints);
                }
                if (impactVfx != null && doImpact == true)
                {
                    GameObject vfx = Instantiate(impactVfx);
                    vfx.transform.position = collision.ClosestPoint(myTransform.position);
                    doImpact = false;
                    ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;
                    Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

                    //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    shape.rotation = new Vector3(-angle - 180, 90, 0);
                    Destroy(vfx, 2);

                    AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                    if (shakeWhenHit)
                        CameraFollow.instance.Shake(0.15f, shakeAmp);
                }
                Destroy(myObj);
            }
            else if (playerScript.team != parentScript.team)
            {
                bool alreadyAttacked = false;
                for(int i=0;i<players.Count;i++)
                {
                    if (players[i] == collisionGameobject)
                    {
                        alreadyAttacked = true;
                        break;
                    }
                }
                if (alreadyAttacked == false)
                {
                    if (playerScript.shielded && doDamage)
                    {
                        playerScript.BreakShield();
                        if (impactVfx != null && doImpact == true && doImpactOnlyOnPlayers)
                        {
                            GameObject vfx = Instantiate(impactVfx);
                            vfx.transform.position = collision.ClosestPoint(myTransform.position);
                            doImpact = false;
                            ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;
                            Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

                            //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

                            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                            shape.rotation = new Vector3(-angle, 90, 0);
                            ParticleSystem.MainModule main = vfx.GetComponent<ParticleSystem>().main;
                            main.startSpeed = 13;
                            Destroy(vfx, 2);
                            if (sfxWhenHitMob && impactSfx != null)
                                AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
                            if (shakeWhenHit)
                                CameraFollow.instance.Shake(0.15f, shakeAmp);
                        }
                        if (canDie)
                        {
                            if (saveParticlesWhenDestroyed)
                            {
                                ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                                if (ps != null)
                                {
                                    ps.transform.parent = null;
                                    ps.transform.localScale = new Vector2(1, 1);
                                    Destroy(ps.gameObject, 2f);
                                }
                            }
                            Destroy(myObj);
                        }

                    }
                    else
                    {
                        bool ok = false;
                        if (ok == false)
                        {
                            float ogDamage = damage;
                            if (applyParentDamage)
                            {
                                if (parentIsWizard)
                                    ogDamage += parentWizard.staffDamage;
                                else if (parentIsKnight)
                                    ogDamage += parentKnight.weaponDamage;
                                else if (parentIsTank)
                                    ogDamage += parentTank.weaponDamage;
                                else if (parentIsMedic)
                                    ogDamage += parentMedic.weaponDamage;
                            }

                            if (doDamage && ((playerScript.isBoss && playerScript.bossScene) == false || playerScript.stunned))
                            {
                                if (parentScript.bot == false && parentScript.finalBoss == false)
                                    if (parentScript.Weakened)
                                        ogDamage /= 2;

                                if (playerDirection)
                                    playerScript.DecreaseHp(ogDamage, -parent.transform.up * knockBackSpeed, parentScript);
                                else if (limbDirection)
                                    playerScript.DecreaseHp(ogDamage, -parentScript.limbsObj.transform.up * knockBackSpeed, parentScript);
                                else if (secondaryPlayerDirection)
                                    playerScript.DecreaseHp(ogDamage, -secondaryParent.transform.up * knockBackSpeed, parentScript);
                                else playerScript.DecreaseHp(ogDamage, -myTransform.up * knockBackSpeed, parentScript);

                                if(parentScript != null)
                                    parentScript.Combo(playerScript.gameObject);

                                if (parentIsMedic)
                                {
                                    parentMedic.AddDamage(ogDamage);
                                }
                                if (parentIsTank)
                                {
                                    parentTank.AddDamage(ogDamage);
                                }

                                if (parentScript.bot == false && parentScript.inDungeon && parentScript.online == false)
                                {
                                    foreach (SelectableItem itemSlot in inventorySpawn.itemSlots)
                                    {
                                        if (itemSlot.containsItem)
                                        {
                                            if (itemSlot.itemType == 0)
                                            {
                                                if (Random.value > 0.88)
                                                {
                                                    parentScript.Heal(ogDamage /5);
                                                }
                                            }
                                            if (itemSlot.itemType == 5 && ice == false)
                                            {
                                                if (Random.Range(1, 999999) % 20 == 0)
                                                {
                                                    playerScript.Ice(2);
                                                }
                                            }
                                            if (itemSlot.itemType == 8 && fire == false)
                                            {
                                                if (Random.Range(1, 999999) % 10 == 0)
                                                {
                                                    playerScript.SetOnFire(parentScript.str + parentScript.bonusStrPoints);
                                                }
                                            }
                                        }
                                    }
                                }

                                player.MobType effect = parentScript.ApplyEffect();
                                switch (effect)
                                {
                                    case player.MobType.Fire:
                                        playerScript.SetOnFire(parentScript.str + parentScript.bonusStrPoints);
                                        break;
                                    case player.MobType.Ice:
                                        playerScript.Ice();
                                        break;
                                    case player.MobType.Plains:
                                        playerScript.Weaken();
                                        break;
                                    case player.MobType.Magic:
                                        playerScript.Silence();
                                        break;
                                    case player.MobType.Stone:
                                        playerScript.Blind();
                                        break;

                                }
                            }
                            players.Add(collisionGameobject);
                            combodPlayers.Add(collisionGameobject);

                            if (fire)
                            {
                                playerScript.SetOnFire(parentScript.str + parentScript.bonusStrPoints);
                            }
                            if (ice)
                            {
                                playerScript.Ice(2);
                            }
                            if (stun && ((playerScript.isBoss && playerScript.bossScene) == false))
                            {
                                playerScript.Stun(stunTime);
                            }
                            if (poison)
                            {
                                PoisonPool();
                                playerScript.Poison(parentScript, parentScript.str + parentScript.bonusStrPoints);
                            }
                            if (poison_pool)
                            {
                                playerScript.Poison(parentScript, parentScript.str + parentScript.bonusStrPoints);
                            }
                            if (slow)
                            {
                                playerScript.Slow(slowTime);
                            }
                            if(impactInRadius)
                            {
                                foreach (player player in FindObjectsOfType<player>())
                                {
                                    Transform playerTransform = player.transform;
                                    GameObject playerGameObject = player.gameObject;
                                    bool ok3 = true;
                                    for(int i=0;i<players.Count;i++)
                                    {
                                        if (players[i] == playerGameObject)
                                            ok3 = false;
                                    }
                                    if (MathUtils.CompareDistances(playerTransform.position, myTransform.position, 5, MathUtils.CompareTypes.LessThan) && playerGameObject != collisionGameobject && playerGameObject != parent && ok3 && player.Animator.GetCurrentAnimatorStateInfo(0).IsName("death") == false && player.team != parentScript.team)
                                    {
                                        if ((playerScript.isBoss && playerScript.bossScene) == false || playerScript.stunned)
                                        {
                                            player.DecreaseHp(damage, -myTransform.up * knockBackSpeed, parentScript);
                                            if(slow)
                                                player.Slow(slowTime);
                                            if (stun && ((player.isBoss && player.bossScene) == false))
                                            {
                                                player.Stun(stunTime);
                                            }
                                        }
                                    }
                                }
                            }
                            bool firstTry = true;
                            if (electricity)
                            {
                                player[] playerList = FindObjectsOfType<player>();
                                for (int i = 0; i < 9999; i++)
                                {
                                    if (i >= players.Count)
                                        break;
                                    else
                                    {

                                        bool ok2 = false;

                                        if (ok2 == false)
                                        {
                                            foreach (player player in playerList)
                                            {
                                                Transform playerTransform = player.transform;
                                                GameObject playerGameObject = player.gameObject;
                                                bool ok3 = true;
                                                for (int j = 0; j < players.Count; j++)
                                                {
                                                    if (players[j] == playerGameObject)
                                                        ok3 = false;
                                                }

                                                Vector3 playerPosition = players[i].transform.position;

                                                if (MathUtils.CompareDistances(playerTransform.position, playerPosition, 5, MathUtils.CompareTypes.LessThan) && playerGameObject != collisionGameobject && playerGameObject != parent && ok3 && player.Animator.GetCurrentAnimatorStateInfo(0).IsName("death") == false && player.team != parentScript.team && players.Contains(playerGameObject) == false)
                                                {
                                                    if ((playerScript.isBoss && playerScript.bossScene) == false || playerScript.stunned)
                                                        player.DecreaseHp(damage, -myTransform.up * knockBackSpeed, parentScript);
                                                    players.Add(playerGameObject);
                                                    GameObject bolt = Instantiate(electricalBolt);
                                                    bolt.GetComponent<hitbox>().parent = parent;
                                                    var dir = playerTransform.position - playerPosition;
                                                    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
                                                    bolt.transform.position = (playerTransform.position + playerPosition) / 2f;
                                                    bolt.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                                                    bolt.transform.localScale = new Vector3(30f, dir.magnitude * 5, 1);
                                                    Destroy(bolt, 0.3f);
                                                }
                                            }
                                        }
                                        firstTry = false;
                                    }
                                }
                            }
                            Impact(collision);
                        }
                    }
                }
            }
            else
            {
                bool containsNeedle = myObj.name.Contains("needle");
                if (heal && ((parentScript.bot && containsNeedle) || containsNeedle == false))
                {
                    bool ok = false;
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i] == collisionGameobject)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (parentScript.bot)
                    {
                        if (healed)
                            ok = true;
                    }


                    if (ok == false)
                    {
                        if (parentScript.bot)
                        {
                            healed = true;
                        }
                        playerScript.Heal(healAmmount + parentScript.maxHealth / 2.5f);
                        players.Add(collisionGameobject);
                    }
                }
                if (shield)
                {
                    bool ok = false;
                    for(int i=0;i<players.Count;i++)
                    {
                        if (players[i] == collisionGameobject)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok == false)
                    {
                        playerScript.Shield();
                        players.Add(collisionGameobject);
                    }
                }
            }
        }
    }

    private void Impact(Collider2D collision = null)
    {
        if (impactVfx != null && doImpact == true )
        {
            GameObject vfx = Instantiate(impactVfx);
            if (collision != null)
                vfx.transform.position = collision.ClosestPoint(myTransform.position);
            else vfx.transform.position = myTransform.position;
            doImpact = false;
            ParticleSystem.ShapeModule shape = vfx.GetComponent<ParticleSystem>().shape;

            Vector2 dir = (collision.ClosestPoint(myTransform.position) - (Vector2)myTransform.position).normalized;

            //float angle = Mathf.Atan2(myTransform.up.y, myTransform.up.x) * Mathf.Rad2Deg;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            shape.rotation = new Vector3(-angle, 90, 0);
            ParticleSystem.MainModule main = vfx.GetComponent<ParticleSystem>().main;
            main.startSpeed = 13;
            Destroy(vfx, 2);
            if (sfxWhenHitMob && impactSfx != null)
                AudioSource.PlayClipAtPoint(impactSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
            if (shakeWhenHit)
                CameraFollow.instance.Shake(0.15f, shakeAmp);
        }
        if (canDie)
        {
            if (saveParticlesWhenDestroyed)
            {
                ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.parent = null;
                    ps.transform.localScale = new Vector2(1, 1);
                    Destroy(ps.gameObject, 2f);
                }
            }
            Destroy(myObj);
        }
    }

    bool healed = false;

    private void OnEnable()
    {
        if (parent != null)
        {
            parentScript = parent.GetComponent<player>();
        }
    }

    public void ForceClear()
    {
        if (parentScript != null && players != null && collider.enabled == false)
        {
            parentScript.ClearComboList(players);
        }
    }


    public void ResetPlayers(bool clear = false)
    {
        healed = false;

        /*if (parentScript != null)
        {
            if (clear)
            {
                parentScript.ClearComboList(combodPlayers);
                combodPlayers.Clear();
            }
        }*/

        players.Clear();
    }

    bool inRoom = true;
}
