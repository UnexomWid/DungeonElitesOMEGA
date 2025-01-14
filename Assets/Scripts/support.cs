using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class support : MonoBehaviour
{
    public AudioClip stunInjection;

    public float damageDealt = 8000;
    public GameObject dashEffect;

    public void AddDamage(float damage)
    {
        if (player.inDungeon)
        {
            damageDealt += damage;
            if (damageDealt > player.suppCharge || isInShops)
                damageDealt = player.suppCharge;
            if (damageDealt < 0)
                damageDealt = 0;

            if (player.inDungeon)
                SetUpText();
            if (damageDealt >= player.suppCharge)
                for (int i = 0; i <= 2; i++)
                {
                    if (player.attacks[i] == "attack3")
                    {
                        player.Invoke("EnableAbil" + (i + 1), 0);
                    }
                }
            else for (int i = 0; i <= 2; i++)
                {
                    if (player.attacks[i] == "attack3")
                    {
                        player.Invoke("ResetCharge" + (i + 1), 0);
                    }
                }
        }
    }

    private void OnDisable()
    {
        DeactivateWeapon();
        weapon.ResetPlayers();
    }

    public float startWeaponDamage;
    public float startPotDamage;
    public float startPotSpeed;
    public float startWeaponKnock;
    public float startPotKnock;
    public float startPotTime;
    public float startStunTime;
    public float startStunDamage;
    public float startPoisonDamage;
    public float startPoisonKnock;
    public float startPoisonSpeed;
    public float startJealAmount;
    public float startKnockDamage;
    public float startStunInjectionDamage;
    public float startStunInjectionTime;
    public float startDashSpeed;
    public float startStunInjectionKnock;

    public float bonusWeaponDamage;
    public float bonusPotDamage;
    public float bonusPotSpeed;
    public float bonusWeaponKnock;
    public float bonusPotKnock;
    public float bonusPotTime;
    public float bonusStunTime;
    public float bonusStunDamage;
    public float bonusPoisonDamage;
    public float bonusPoisonKnock;
    public float bonusPoisonSpeed;
    public float bonusJealAmount;
    public float bonusKnockDamage;
    public float bonusStunInjectionDamage;
    public float bonusStunInjectionTime;
    public float bonusDashSpeed;
    public float bonusStunInjectionKnock;

    public hitbox aoeBox;
    public GameObject potion;
    public GameObject potPos;
    public GameObject pushObj;

    public AudioSource attack;
    public AudioSource dash;
    public AudioSource square;
    public AudioSource potThrow;
    public AudioSource stunWave;

    public hitbox weapon;
    public float weaponDamage;
    public float weaponKnock;
    public float potSpeed;
    public float potDamage;
    public float potKnock;
    public float potTime;
    public float stunDamage;
    public float stunTime;
    public float healAmount;
    public float knockDamage;
    public float poisonDamage;
    public float poisonKnock;
    public float stunInjectionDamage;
    public float stunInjectionKnock;
    public float stunInjectionTime;
    public float dashSpeed;
    public hitbox healBox;
    public hitbox knockBox;
    public hitbox stunBox;

    public void LevelAbil(int abilNumber, bool perfect)
    {
        int multiplier = 1;
        if (perfect)
            multiplier = 3;
        if (player.attacks[abilNumber] == "attack1")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("potSpeed", potSpeed + bonusPotSpeed*multiplier));
            settings.Add(new setting("potDamage", potDamage + bonusPotDamage*multiplier));
            settings.Add(new setting("potKnock", potKnock + bonusPotKnock*multiplier));
            settings.Add(new setting("potTime", potTime + bonusPotTime*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack2")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("stunTime", stunBox.stunTime+bonusStunTime*multiplier));
            settings.Add(new setting("stunDamage", stunBox.damage+bonusStunDamage*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack3")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("healAmount", healAmount+bonusJealAmount*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack4")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("knockDamage", knockDamage + bonusKnockDamage*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack6")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("poisonDamage", poisonDamage + bonusPoisonDamage*multiplier));
            settings.Add(new setting("poisonKnock", poisonKnock + bonusPoisonKnock*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack7")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("stunInjectionDamage", stunInjectionDamage + bonusStunInjectionDamage*multiplier));
            settings.Add(new setting("stunInjectionKnock", stunInjectionKnock + bonusStunInjectionKnock*multiplier));
            settings.Add(new setting("stunInjectionTime", stunInjectionTime + bonusStunInjectionTime*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack8")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("dashSpeed", dashSpeed+bonusDashSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        player.currentCooldowns[abilNumber] -= player.cooldownReduce[abilNumber] * multiplier;
        if (player.bot == false)
            SetUpText();
    }

    public player player;
    BoxCollider2D weaponHitbox;
    bool isInShops;
    public void Start()
    {
        isInShops = SceneManager.GetActiveScene().name.Contains("Shops");
        weaponHitbox = weapon.GetComponent<BoxCollider2D>();
        player = GetComponent<player>();
        damageDealt = player.suppCharge;

        if (player.bot || player.isBoss || player.finalBoss)
        {
            startWeaponDamage *= player.difficultyPercent;
            startPotDamage *= player.difficultyPercent;
            startStunDamage *= player.difficultyPercent;
            startKnockDamage *= player.difficultyPercent;
            startStunInjectionDamage *= player.difficultyPercent;
        }

        List<setting> settings = new List<setting>();
        settings.Add(new setting("weaponDamage", startWeaponDamage));
        settings.Add(new setting("weaponKnock", startWeaponKnock));
        settings.Add(new setting("potSpeed", startPotSpeed));
        settings.Add(new setting("potDamage", startPotDamage));
        settings.Add(new setting("potKnock", startPotKnock));
        settings.Add(new setting("potTime", startPotTime));
        settings.Add(new setting("stunTime", startStunTime));
        settings.Add(new setting("stunDamage", startStunDamage));
        settings.Add(new setting("poisonDamage", startPoisonDamage));
        settings.Add(new setting("poisonKnock", startPoisonKnock));
        settings.Add(new setting("poisonSpeed", startPoisonSpeed));
        settings.Add(new setting("healAmount", startJealAmount));
        settings.Add(new setting("knockDamage", startKnockDamage));
        settings.Add(new setting("stunInjectionDamage", startStunInjectionDamage));
        settings.Add(new setting("stunInjectionKnock", startStunInjectionKnock));
        settings.Add(new setting("stunInjectionTime", startStunInjectionTime));
        settings.Add(new setting("dashSpeed", startDashSpeed));
        SetUpHitBoxes(settings);
        BoxCollider2D box = null;
        foreach (BoxCollider2D b in pushObj.GetComponents<BoxCollider2D>())
        {
            if (b.isTrigger == false)
                box = b;
        }
        foreach (player pl in FindObjectsOfType<player>())
        {
            if (pl.team == player.team)
            {
                try
                {
                    Physics2D.IgnoreCollision(pl.GetComponent<CircleCollider2D>(), box);
                }
                catch
                {

                }
            }
        }
        if (player.bot == false && player.inDungeon)
            SetUpText();
        player.baseDamage = startWeaponDamage;
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
            player.rb.velocity = (dashSpeed + player.originalSpeed) * -transform.up * player.animator.speed;

            if (player.isBoss && player.SceneName == "Boss")
                player.rb.velocity *= 3;

            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void SetDamageType(int type)
    {
        if (type == 1)
        {
            aoeBox.damage = poisonDamage;
            aoeBox.knockBackSpeed = poisonKnock;
        }
        else if (type == 2)
        {
            aoeBox.damage = stunInjectionDamage;
            aoeBox.knockBackSpeed = stunInjectionKnock;
            aoeBox.knockBackSpeed = stunInjectionTime;
        }
    }
    public void SetKnockType(int type)
    {
        if (type == 1)
        {
            weapon.knockBackSpeed = weaponKnock + poisonKnock;
        }
        else if (type == 2)
        {
            weapon.knockBackSpeed = weaponKnock + stunInjectionKnock;
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
    public void SetUpHitBoxes(List<setting> settings)
    {
        foreach (setting setting in settings)
        {
            if (setting.name == "weaponDamage")
            {
                weaponDamage = setting.value;
                weapon.damage = setting.value;
            }
            else if (setting.name == "weaponKnock")
            {
                weaponKnock = setting.value;
                weapon.knockBackSpeed = setting.value;
            }
            else if (setting.name == "potSpeed")
            {

                    potSpeed = setting.value;
                
            }
            else if (setting.name == "potDamage")
            {

                    potDamage = setting.value;
                
            }
            else if (setting.name == "potKnock")
            {
                potKnock = setting.value;
            }
            else if (setting.name == "potTime")
            {
                potTime = setting.value;
            }
            else if (setting.name == "stunTime")
            {
                stunBox.stunTime = setting.value;
            }
            else if (setting.name == "stunDamage")
            {
                stunBox.damage = setting.value;
            }
            else if (setting.name == "healAmount")
            {
                healAmount = setting.value;
                healBox.healAmmount = setting.value;
                player.healValue = healAmount + player.maxHealth / 2.5f;
            }
            else if (setting.name == "knockDamage")
            {
                knockBox.damage = setting.value;
            }
            else if (setting.name == "poisonDamage")
            {
                poisonDamage = setting.value;
                weapon.healAmmount = setting.value;
            }
            else if (setting.name == "poisonKnock")
            {
                poisonKnock = setting.value;
                weapon.knockBackSpeed = setting.value;
            }
            else if (setting.name == "stunInjectionDamage")
            {
                stunInjectionDamage = setting.value;
            }
            else if (setting.name == "stunInjectionKnock")
            {
                stunInjectionKnock = setting.value;
            }
            else if (setting.name == "stunInjectionTime")
            {
                weapon.stunTime = setting.value;
            }
            else if (setting.name == "dashSpeed")
            {
                dashSpeed = setting.value;
            }
        }
    }
    public void SetUpText()
    {
        if (player.bot == false)
        {
            for (int i = 0; i < player.attacks.Length; i++)
            {
                if (player.attacks[i].Contains("1"))
                {
                    player.abilityDescs[i].abilityText.text = "Throws a potion that upon impact deals <color=red><b>" + (potDamage + weaponDamage) + "</b></color> damage and <color=brown><b>slows</b></color> it's enemies.";
                }
                else if (player.attacks[i].Contains("2"))
                {
                    player.abilityDescs[i].abilityText.text = "Shoots a wave that deals <color=red><b>" + (stunBox.damage + weaponDamage) + "</b></color> and <color=blue><b>stuns</b></color> all enemies hit.";
                }
                else if (player.attacks[i].Contains("3"))
                {
                    player.abilityDescs[i].abilityText.text = "Creates a square that <color=lime><b>heals</b></color> all alies hit for <color=red><b>" + (healBox.healAmmount + player.maxHealth / 2.5f) + "</b></color> health. Charge: <color=blue><b>" + ((int)(damageDealt / (player.suppCharge / 100))) + "%</b></color>";
                }
                else if (player.attacks[i].Contains("4"))
                {
                    player.abilityDescs[i].abilityText.text = "Creates a square that <color=black><b>knockbacks</b></color> all enemies hit and dealing <color=red><b>" + (knockBox.damage + weaponDamage) + "</b></color> damage.";
                }
                else if (player.attacks[i].Contains("5"))
                {
                    player.abilityDescs[i].abilityText.text = "Creates a square that grants a <color=gray><b>shield</b></color> to all allies hit.";
                }
                else if (player.attacks[i].Contains("6"))
                {
                    player.abilityDescs[i].abilityText.text = "Attacks forward. Enemies hit are <color=green><b>poisoned</b></color> and take <color=red><b>" + (poisonDamage + weaponDamage) + "</b></color> damage, while allies hit are <color=lime><b>healed</b></color> for <color=red><b>" + (poisonDamage + player.maxHealth / 50) + "</b></color> health.";
                }
                else if (player.attacks[i].Contains("7"))
                {
                    player.abilityDescs[i].abilityText.text = "Attacks forward. Enemies hit are <color=blue><b>stunned</b></color> and take <color=red><b>" + (stunInjectionDamage + weaponDamage) + "</b></color> damage.";
                }
                else if (player.attacks[i].Contains("8"))
                {
                    player.abilityDescs[i].abilityText.text = "Dashes with a speed of <color=purple><b>" + (dashSpeed + player.originalSpeed) + "</b></color>.";
                }
                if (player.attacks[i].Contains("3") == false)
                {
                    float cd;
                    if (SceneManager.GetActiveScene().name == "Shops")
                        cd = player.currentCooldowns[i];
                    else cd = player.currentCooldowns[i] - player.cooldownReduction / 100 * player.currentCooldowns[i];
                    player.abilityDescs[i].abilityText.text += "\n<color=blue><b>Cooldown</b></color>: " + cd.ToString("0.00") + " seconds.";
                }
            }
        }
    }
    public void PlayAttack()
    {
        attack.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack.Play();
    }
    public void PlayInjection()
    {
        AudioSource.PlayClipAtPoint(stunInjection, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }
    public void PlayDash()
    {
        dash.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        dash.Play();
    }
    public void PlaySquare()
    {
        square.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        square.Play();
    }
    public void PlayPotThrow()
    {
        potThrow.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        potThrow.Play();
    }



    public void SpawnPot()
    {
        GameObject pot = Instantiate(potion);
        pot.transform.position = potPos.transform.position;
        pot.transform.rotation = potPos.transform.rotation;
        pot.GetComponent<Rigidbody2D>().velocity = potSpeed * (-transform.up);
        hitbox potHitbox = pot.GetComponent<hitbox>();
        potHitbox.parent = gameObject;
        potHitbox.damage = potDamage + weaponDamage;
        potHitbox.knockBackSpeed = potKnock;
        potHitbox.slowTime = potTime;

        if(player.isBoss)
            pot.transform.localScale *= 3;
    }

    public void MobRevive()
    {
        if (player.mobToRevive != null && player.canRevive)
        {
            player.mobToRevive.GetComponent<player>().Revive();
            player.mobToRevive = null;
        }
    }
}
