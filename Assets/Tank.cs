using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tank : MonoBehaviour
{
    public float damageDealt = 5000;
    public GameObject dashEffect;
    public GameObject groundParticles;

    public void GroundParticles()
    {
        GameObject groundClones = Instantiate(groundParticles);
        groundClones.transform.position = transform.position;
        Destroy(groundClones, 2f);
    }

    private void OnDisable()
    {
        DeactivateWeapon();
        weapon.ResetPlayers();
    }

    public void AddDamage(float damage)
    {
        if (player.inDungeon)
        {
            damageDealt += damage;
            if (damageDealt > player.tankCharge || isInShops)
                damageDealt = player.tankCharge;
            if (damageDealt < 0)
                damageDealt = 0;

            if (player.inDungeon)
                SetUpText();
            if (damageDealt >= player.tankCharge)
                for (int i = 0; i <= 2; i++)
                {
                    if (player.attacks[i] == "attack6")
                    {
                        player.Invoke("EnableAbil" + (i + 1), 0);
                    }
                }
            else for (int i = 0; i <= 2; i++)
                {
                    if (player.attacks[i] == "attack6")
                    {
                        player.Invoke("ResetCharge" + (i + 1), 0);
                    }
                }
        }
    }

    public AudioSource attack;
    public AudioSource dash;
    public AudioSource walls;
    public AudioSource pool;
    public AudioSource heal;
    public AudioSource land;

    public hitbox weapon;
    public float weaponDamage;
    public float weaponKnock;
    public float knockDamage;
    public float knockKnock;
    public float dashDamage;
    public float dashKnock;
    public float dashSpeed;
    public float dashStun;
    public hitbox jumpBox;
    public hitbox slowBox;
    public hitbox healBox;
    public GameObject wall;
    public float wallSize;
    public float wallsRange;
    public float phaseSpeed;
    public float startWeaponDamage;
    public float startWeaponKnock;
    public float startKnockDamage;
    public float startKnockKnock;
    public float startDashDamage;
    public float startDashKnock;
    public float startDashSpeed;
    public float startDashStun;
    public float startJumpDamage;
    public float startJumpStun;
    public float startWallSize;
    public float startSlowAmount;
    public float startHealAmount;
    public float startPhaseSpeed;
    public float startWallsRange;

    public float bonusWeaponDamage;
    public float bonusWeaponKnock;
    public float bonusKnockDamage;
    public float bonusKnockKnock;
    public float bonusDashDamage;
    public float bonusDashKnock;
    public float bonusDashSpeed;
    public float bonusDashStun;
    public float bonusJumpDamage;
    public float bonusJumpStun;
    public float bonusWallSize;
    public float bonusSlowAmount;
    public float bonusHealAmount;
    public float bonusPhaseSpeed;
    public float bonusWallsRange;

    public void ResetCh(bool resetDamage)
    {
        if (resetDamage)
        {
            SetWeaponDamage(1);
            SetWeaponKnock(1);
        }
        weaponHitbox.enabled = false;
        DeactivateLimbDirection();
        ActivatePlayerDirection();
    }

    public void Dash()
    {
        //if (FindObjectOfType<GameClient>() == null || (FindObjectOfType<GameClient>() != null && FindObjectOfType<GameHost>() != null))
        {
            rb.velocity = (dashSpeed + player.originalSpeed) * -transform.up * animator.speed;

            if (player.isBoss && player.SceneName == "Boss")
                rb.velocity *= 3;

            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void Phase()
    {
        //if (FindObjectOfType<GameClient>() == null || (FindObjectOfType<GameClient>() != null && FindObjectOfType<GameHost>() != null))
        {
            gameObject.layer = 21;

            rb.velocity = (phaseSpeed + player.originalSpeed) * -transform.up * animator.speed;

            if (player.isBoss && player.SceneName == "Boss")
                rb.velocity *= 3;

            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void ActivateWeapon()
    {
        weaponHitbox.enabled = true;
    }
    public void DeactivateWeapon()
    {
        weaponHitbox.enabled = false;
    }
    public void SetWeaponDamage(float multiplier)
    {
        weapon.damage = weaponDamage * multiplier;
    }
    public void SetWeaponKnock(float multiplier)
    {
        weapon.knockBackSpeed = weaponKnock * multiplier;
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("basic1") || stateInfo.IsName("basic2") || stateInfo.IsName("basic3") || stateInfo.IsName("basic1_nonstop"))
            weapon.knockBackSpeed += (player.originalSpeed - player.baseSpeed);
    }
    public void SetDamageType(int type)
    {
        if (type == 1)
        {
            weapon.damage = weaponDamage + knockDamage;
        }
        else if (type == 2)
        {
            weapon.damage = weaponDamage + dashDamage;
        }
    }
    public void SetKnockType(int type)
    {
        if (type == 1)
        {
            weapon.knockBackSpeed = weaponKnock + knockKnock;
        }
        else if (type == 2)
        {
            weapon.knockBackSpeed = weaponKnock + dashKnock;
        }
    }
    public void ActivateLimbDirection()
    {
        weapon.limbDirection = true;
    }
    public void DeactivateLimbDirection()
    {
        weapon.limbDirection = false;
    }
    public void ActivatePlayerDirection()
    {
        weapon.playerDirection = true;
    }
    public void DeactivatePlayerDirection()
    {
        weapon.playerDirection = false;
    }
    public float healAmount;
    player player;
    Rigidbody2D rb;
    Animator animator;
    BoxCollider2D weaponHitbox;
    string sceneName;
    bool isInShops;
    public void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        isInShops = sceneName.Contains("Shops");
        weaponHitbox = weapon.GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<player>();
        damageDealt = player.tankCharge;

        if (player.bot || player.isBoss || player.finalBoss)
        {
            startWeaponDamage *= player.difficultyPercent;
            startKnockDamage *= player.difficultyPercent;
            startDashDamage *= player.difficultyPercent;
            startJumpDamage *= player.difficultyPercent;
        }

        List<setting> settings = new List<setting>();
        settings.Add(new setting("weaponDamage", startWeaponDamage));
        settings.Add(new setting("weaponKnock", startWeaponKnock));
        settings.Add(new setting("knockDamage", startKnockDamage));
        settings.Add(new setting("knockKnock", startKnockKnock));
        settings.Add(new setting("dashDamage", startDashDamage));
        settings.Add(new setting("dashKnock", startDashKnock));
        settings.Add(new setting("dashSpeed", startDashSpeed));
        settings.Add(new setting("dashStun", startDashStun));
        settings.Add(new setting("jumpDamage", startJumpDamage));
        settings.Add(new setting("jumpStun", startJumpStun));
        settings.Add(new setting("wallSize", startWallSize));
        settings.Add(new setting("slowAmount", startSlowAmount));
        settings.Add(new setting("healAmount", startHealAmount));
        settings.Add(new setting("phaseSpeed", startPhaseSpeed));
        settings.Add(new setting("wallsRange", startWallsRange));
        SetUpHitBoxes(settings);
        if (player.bot == false && player.inDungeon)
            SetUpText();
        player.baseDamage = startWeaponDamage;
    }

    public void LevelAbil(int abilNumber, bool perfect)
    {
        int multiplier = 1;
        if (perfect)
            multiplier = 3;
        if (player.attacks[abilNumber] == "attack1")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("knockDamage", knockDamage + bonusKnockDamage*multiplier));
            settings.Add(new setting("knockKnock", knockKnock + bonusKnockKnock*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack2")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("dashDamage", dashDamage + bonusDashDamage*multiplier));
            settings.Add(new setting("dashKnock", dashKnock + bonusDashKnock*multiplier));
            settings.Add(new setting("dashSpeed", dashSpeed + bonusDashSpeed * multiplier));
            settings.Add(new setting("dashStun", dashStun + bonusDashStun*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack3")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("jumpDamage", jumpBox.damage + bonusJumpDamage*multiplier));
            settings.Add(new setting("jumpStun", jumpBox.stunTime + bonusJumpStun*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack4")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("wallSize", wallSize + bonusWallSize*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack5")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("slowAmount", slowBox.slowTime + bonusSlowAmount*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack6")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("healAmount", healAmount + bonusHealAmount*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack7")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("phaseSpeed", phaseSpeed + bonusPhaseSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack8")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("wallsRange", wallsRange + bonusWallsRange*multiplier));
            SetUpHitBoxes(settings);
        }
        player.currentCooldowns[abilNumber] -= player.cooldownReduce[abilNumber] * multiplier;
        if(player.bot == false)
        SetUpText();
    }
    public void SetUpHitBoxes(List<setting> settings)
    {
        foreach (setting setting in settings)
        {
            if (setting.name == "weaponDamage")
            {
                weaponDamage = setting.value;
                weapon.damage = weaponDamage;
            }
            else if (setting.name == "weaponKnock")
            {
                weaponKnock = setting.value;
                weapon.knockBackSpeed = weaponKnock;
            }
            else if (setting.name == "knockDamage")
            {
                knockDamage = setting.value;
            }
            else if (setting.name == "knockKnock")
            {
                knockKnock = setting.value;
            }
            else if (setting.name == "dashDamage")
            {
                dashDamage = setting.value;
            }
            else if (setting.name == "dashKnock")
            {
                dashKnock = setting.value;
            }
            else if (setting.name == "dashSpeed")
            {
                dashSpeed = setting.value;
            }
            else if (setting.name == "dashStun")
            {
                dashStun = setting.value;
                weapon.stunTime = dashStun;
            }
            else if (setting.name == "jumpDamage")
            {
                jumpBox.damage = setting.value;
            }
            else if (setting.name == "jumpStun")
            {
                jumpBox.stunTime = setting.value;
            }
            else if (setting.name == "wallSize")
            {
                wallSize = setting.value;
                wall.transform.localScale = new Vector3(wallSize, wallSize, 1);
            }
            else if (setting.name == "slowAmount")
            {
                slowBox.slowTime = setting.value;
            }
            else if (setting.name == "healAmount")
            {
                healAmount = setting.value;
            }
            else if (setting.name == "phaseSpeed")
            {
                phaseSpeed = setting.value;
            }
            else if (setting.name == "wallsRange")
            {
                wallsRange = setting.value;
            }
        }
    }
    public void SetUpText()
    {
        for (int i = 0; i < player.attacks.Length; i++)
        {
            if (player.attacks[i].Contains("1"))
            {
                player.abilityDescs[i].abilityText.text = "Spins <color=black><b>heavily knocking</b></color> all enemies hit and dealing <color=red><b>" + (knockDamage + weaponDamage) + "</b></color> damage.";
            }
            else if (player.attacks[i].Contains("2"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes with a speed of <color=purple><b>" + (dashSpeed + player.originalSpeed) + "</b></color> and ending with an attack that deals <color=red><b>"+ (dashDamage+weaponDamage)+ "</b></color> damage, <color=blue><b>stunning</b></color> it's enemies hit.";
            }
            else if (player.attacks[i].Contains("3"))
            {
                player.abilityDescs[i].abilityText.text = "Jumps up. Upon landing, <color=blue><b>stuns</b></color> all enemies hit and deals <color=red><b>" + (jumpBox.damage + weaponDamage) + "</b></color> damage.";
            }
            else if (player.attacks[i].Contains("4"))
            {
                player.abilityDescs[i].abilityText.text = "Creates a wall and brings it closer to the center.";
            }
            else if (player.attacks[i].Contains("5"))
            {
                player.abilityDescs[i].abilityText.text = "Creates a square that <color=brown><b>slows</b></color> it's enemies for <color=red><b>"+slowBox.slowTime+"</b></color> seconds.";
            }
            else if (player.attacks[i].Contains("6"))
            {
                player.abilityDescs[i].abilityText.text = "<color=lime><b>Heals</b></color> for <color=red><b>"+(healAmount + player.maxHealth / 100) + "</b></color> health. Charge: <color=blue><b>" + ((int)(damageDealt / (player.tankCharge/100))) + "%</b></color>";
            }
            else if (player.attacks[i].Contains("7"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes, while going through walls, with a speed of <color=purple><b>" + (phaseSpeed + player.originalSpeed) + "</b></color>.";
            }
            else if (player.attacks[i].Contains("8"))
            {
                player.abilityDescs[i].abilityText.text = "Spawns walls around, in the shape of a square, blocking everything from leaving.";
            }
            if (player.attacks[i].Contains("6") == false)
            {
                float cd;
                if (sceneName == "Shops")
                    cd = player.currentCooldowns[i];
                else cd = player.currentCooldowns[i] - player.cooldownReduction / 100 * player.currentCooldowns[i];
                player.abilityDescs[i].abilityText.text += "\n<color=blue><b>Cooldown</b></color>: " + cd.ToString("0.00") + " seconds.";
            }
        }
    }
    public void PlayAttack()
    {
        attack.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack.Play();
    }
    public void PlayDash()
    {
        dash.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        dash.Play();
    }
    public void PlayWalls()
    {
        walls.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        walls.Play();
    }
    public void PlayPool()
    {
        pool.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        pool.Play();
    }
    public void PlayHeal()
    {
        heal.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        heal.Play();

    }
    public void PlayLand()
    {
        land.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        land.Play();
    }
}
