using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class knight : MonoBehaviour
{

    public GameObject dashEffect;
    public float startWeaponDamage;
    public float startWeaponKnock;
    public float startCloneDamage;
    public float startCloneKnock;
    public float startWaveDamage;
    public float startWaveKnock;
    public float startWaveSpeed;
    public float startWaveStun;
    public float startBladesDamage;
    public float startBladesKnock;
    public float startBladesDuration;
    public float startFuryDuration;
    public float startDashSpeed;
    public float startSecondDashSpeed;
    public float startSpinDamage;
    public float startSpinKnock;
    public float startDashDamage;
    public float startDashKnock;
    public float startSecondDashDamage;
    public float startSecondDashKnock;
    public float startKnockDamage;
    public float startKnockKnock;

    public float bonusWeaponDamage;
    public float bonusWeaponKnock;
    public float bonusCloneDamage;
    public float bonusCloneKnock;
    public float bonusWaveDamage;
    public float bonusWaveKnock;
    public float bonusWaveSpeed;
    public float bonusWaveStun;
    public float bonusBladesDamage;
    public float bonusBladesKnock;
    public float bonusBladesDuration;
    public float bonusFuryDuration;
    public float bonusDashSpeed;
    public float bonusSecondDashSpeed;
    public float bonusSpinDamage;
    public float bonusSpinKnock;
    public float bonusDashDamage;
    public float bonusDashKnock;
    public float bonusSecondDashDamage;
    public float bonusSecondDashKnock;
    public float bonusKnockDamage;
    public float bonusKnockKnock;



    public float spinDamage;
    public float spinKnock;
    public float dashDamage;
    public float dashKnock;
    public float secondDashDamage;
    public float secondDashKnock;
    public float knockDamage;
    public float knockKnock;

    public GameObject wave;
    public GameObject wavePos;
    public float waveSpeed;
    public float waveDamage;
    public float waveKnock;
    public float waveStun;
    public GameObject limbs;

    public AudioSource attack;
    public AudioSource wind;
    public AudioSource stun;
    public AudioSource fury;
    public AudioSource blades;
    public AudioSource dash;

    public float weaponDamage;
    public float weaponKnock;

    public float bladesDuration;
    public float furyDuration;

    public float dashSpeed;
    public float secondDashSpeed;

    public hitbox weapon;
    public hitbox[] clones;
    public hitbox[] bladesBoxes;

    player player;

    private void OnDisable()
    {
        DeactivateWeapon();
        weapon.ResetPlayers();
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

    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D weaponHitbox;
    public void Start()
    {
        weaponHitbox = weapon.GetComponent<BoxCollider2D>();
        player = GetComponent<player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (swords != null)
        {
            try
            {
                foreach (GameObject sword in swords)
                    swordScripts.Add(sword.GetComponent<hitbox>());
            }
            catch
            {

            }
        }

        if (player.bot || player.isBoss || player.finalBoss)
        {
            startWeaponDamage *= player.difficultyPercent;
            startCloneDamage *= player.difficultyPercent;
            startWaveDamage *= player.difficultyPercent;
            startBladesDamage *= player.difficultyPercent;
            startSpinDamage *= player.difficultyPercent;
            startDashDamage *= player.difficultyPercent;
            startSecondDashDamage *= player.difficultyPercent;
            startKnockDamage *= player.difficultyPercent;
        }

        List<setting> settings = new List<setting>();
        settings.Add(new setting("weaponDamage", startWeaponDamage));
        settings.Add(new setting("weaponKnock", startWeaponKnock));
        settings.Add(new setting("cloneDamage", startCloneDamage));
        settings.Add(new setting("cloneKnock", startCloneKnock));
        settings.Add(new setting("waveDamage", startWaveDamage));
        settings.Add(new setting("waveKnock", startWaveKnock));
        settings.Add(new setting("waveStun", startWaveStun));
        settings.Add(new setting("waveSpeed", startWaveSpeed));
        settings.Add(new setting("bladesDamage", startBladesDamage));
        settings.Add(new setting("bladesKnock", startBladesKnock));
        settings.Add(new setting("bladesDuration", startBladesDuration));
        settings.Add(new setting("furyDuration", startFuryDuration));
        settings.Add(new setting("dashSpeed", startDashSpeed));
        settings.Add(new setting("secondDashSpeed", startSecondDashSpeed));
        settings.Add(new setting("spinDamage", startSpinDamage));
        settings.Add(new setting("spinKnock", startSpinKnock));
        settings.Add(new setting("dashDamage", startDashDamage));
        settings.Add(new setting("dashKnock", startDashKnock));
        settings.Add(new setting("secondDashDamage", startSecondDashDamage));
        settings.Add(new setting("secondDashKnock", startSecondDashKnock));
        settings.Add(new setting("knockDamage", startKnockDamage));
        settings.Add(new setting("knockKnock", startKnockKnock));
        SetUpHitBoxes(settings);
        Invoke("Reset", 0.5f);
        if(player.bot == false && player.inDungeon)
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
            settings.Add(new setting("spinDamage", spinDamage + bonusSpinDamage*multiplier));
            settings.Add(new setting("spinKnock", spinKnock + bonusSpinKnock*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack2")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("dashDamage", dashDamage + bonusDashDamage*multiplier));
            settings.Add(new setting("dashKnock", dashKnock + bonusDashKnock*multiplier));
            settings.Add(new setting("dashSpeed", dashSpeed + bonusDashSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack3")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("cloneDamage", clones[0].damage + bonusCloneDamage*multiplier));
            settings.Add(new setting("cloneKnock", clones[0].knockBackSpeed + bonusCloneKnock*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack4")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("waveDamage", waveDamage + bonusWaveDamage*multiplier));
            settings.Add(new setting("waveKnock", waveKnock + bonusWaveKnock*multiplier));
            settings.Add(new setting("waveStun", waveStun + bonusWaveStun*multiplier));
            settings.Add(new setting("waveSpeed", waveSpeed + bonusWaveSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack5")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("secondDashDamage", secondDashDamage + bonusSecondDashDamage*multiplier));
            settings.Add(new setting("secondDashKnock", secondDashKnock + bonusSecondDashKnock*multiplier));
            settings.Add(new setting("secondDashSpeed", secondDashSpeed + bonusSecondDashSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack6")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("furyDuration", furyDuration + bonusFuryDuration*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack7")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("knockDamage", knockDamage + bonusKnockDamage*multiplier));
            settings.Add(new setting("knockKnock", knockKnock + bonusKnockKnock*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack8")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("bladesDamage", bladesBoxes[0].damage + bonusBladesDamage * multiplier));
            settings.Add(new setting("bladesKnock", bladesBoxes[0].knockBackSpeed + bonusBladesKnock * multiplier));
            settings.Add(new setting("bladesDuration", bladesDuration + bonusBladesKnock * multiplier));
            SetUpHitBoxes(settings);
        }
        player.currentCooldowns[abilNumber] -= player.cooldownReduce[abilNumber] * multiplier;
        SetUpText();
    }

    public void SetUpText()
    {
        for (int i = 0; i < player.attacks.Length; i++)
        {
            if (player.attacks[i].Contains("1"))
            {
                player.abilityDescs[i].abilityText.text = "Spins and each spin deals <color=red><b>" + (spinDamage + weaponDamage) + "</b></color> damage to all enemies hit.";
            }
            else if (player.attacks[i].Contains("2"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes with a speed of <color=purple><b>" + (dashSpeed + player.originalSpeed) + "</b></color> and deals <color=red><b>" + (dashDamage + weaponDamage) + " </b></color> damage to all enemies hit.";
            }
            else if (player.attacks[i].Contains("3"))
            {
                player.abilityDescs[i].abilityText.text = "Disappears and spawns 8 clones that brings enemies closer into the center, dealing <color=red><b>" + (clones[0].damage + weaponDamage) + "</b></color> damage.";
            }
            else if (player.attacks[i].Contains("4"))
            {
                player.abilityDescs[i].abilityText.text = "Blows a short wind that deals <color=red><b>" + (waveDamage + weaponDamage) + "</b></color> damage and <color=blue><b>stuns</b></color> the target.";
            }
            else if (player.attacks[i].Contains("5"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes forward with a speed of <color=purple><b>" + (secondDashSpeed + player.originalSpeed) + "</b></color> and ends with an attack that deals <color=red><b>"+ (secondDashDamage + weaponDamage) + "</b></color> damage.";
            }
            else if (player.attacks[i].Contains("6"))
            {
                player.abilityDescs[i].abilityText.text = "Enrages and attacks non-stop for <color=pink><b>" + furyDuration + "</b></color> seconds.";
            }
            else if (player.attacks[i].Contains("7"))
            {
                player.abilityDescs[i].abilityText.text = "Spins once and <color=black><b>knockbacks heavily</b></color> all enemies hit dealing <color=red><b>"+(knockDamage+weaponDamage)+"</b></color> damage.";
            }
            else if (player.attacks[i].Contains("8"))
            {
                player.abilityDescs[i].abilityText.text = "Spawns 8 blades that spin around, each hit dealing <color=red><b>" + (bladesBoxes[0].damage + weaponDamage) + "</b></color> damage.";
            }
            float cd;
            if (SceneManager.GetActiveScene().name == "Shops")
                cd = player.currentCooldowns[i];
            else cd = player.currentCooldowns[i] - player.cooldownReduction / 100 * player.currentCooldowns[i];
            player.abilityDescs[i].abilityText.text += "\n<color=blue><b>Cooldown</b></color>: " + cd.ToString("0.00") + " seconds.";
        }
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
        if(type == 1)
        {
            weapon.damage = weaponDamage + spinDamage;
        }
        else if (type == 2)
        {
            weapon.damage = weaponDamage + dashDamage;
        }
        else if (type == 3)
        {
            weapon.damage = weaponDamage + secondDashDamage;
        }
        else if (type == 4)
        {
            weapon.damage = weaponDamage + knockDamage;
        }
    }
    public void SetKnockType(int type)
    {

        if (type == 1)
        {
            weapon.knockBackSpeed = weaponKnock + spinKnock;
        }
        else if (type == 2)
        {
            weapon.knockBackSpeed = weaponKnock + dashKnock;
        }
        else if (type == 3)
        {
            weapon.knockBackSpeed = weaponKnock + secondDashKnock;
        }
        else if (type == 4)
        {
            weapon.knockBackSpeed = weaponKnock + knockKnock;
        }
    }
    public void SetUpHitBoxes(List<setting> settings)
    {
        foreach (setting setting in settings)
        {
            try
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
                else if (setting.name == "cloneDamage")
                {
                    foreach (hitbox clone in clones)
                    {
                        clone.damage = setting.value;
                    }
                }
                else if (setting.name == "cloneKnock")
                {
                    foreach (hitbox clone in clones)
                    {
                        clone.knockBackSpeed = setting.value;
                    }
                }
                else if (setting.name == "waveDamage")
                {
                    waveDamage = setting.value;
                }
                else if (setting.name == "waveKnock")
                {
                    waveKnock = setting.value;
                }
                else if (setting.name == "waveStun")
                {
                    waveStun = setting.value;
                }
                else if (setting.name == "waveSpeed")
                {
                    waveSpeed = setting.value;
                }
                else if (setting.name == "bladesDamage")
                {
                    foreach (hitbox blade in bladesBoxes)
                    {
                        blade.damage = setting.value;
                    }
                }
                else if (setting.name == "bladesKnock")
                {
                    foreach (hitbox blade in bladesBoxes)
                    {
                        blade.knockBackSpeed = setting.value;
                    }
                }
                else if (setting.name == "bladesDuration")
                {
                    bladesDuration = setting.value;
                }
                else if (setting.name == "furyDuration")
                {
                    furyDuration = setting.value;
                }
                else if (setting.name == "dashSpeed")
                {
                    dashSpeed = setting.value;
                }
                else if (setting.name == "secondDashSpeed")
                {
                    secondDashSpeed = setting.value;
                }
                else if (setting.name == "spinDamage")
                {
                    spinDamage = setting.value;
                }
                else if (setting.name == "spinKnock")
                {
                    spinKnock = setting.value;
                }
                else if (setting.name == "dashDamage")
                {
                    dashDamage = setting.value;
                }
                else if (setting.name == "dashKnock")
                {
                    dashKnock = setting.value;
                }
                else if (setting.name == "secondDashDamage")
                {
                    secondDashDamage = setting.value;
                }
                else if (setting.name == "secondDashKnock")
                {
                    secondDashKnock = setting.value;
                }
                else if (setting.name == "knockDamage")
                {
                    knockDamage = setting.value;
                }
                else if (setting.name == "knockKnock")
                {
                    knockKnock = setting.value;
                }
            }
            catch
            {

            }
        }
    }


    public void PlayAttack()
    {
        attack.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack.Play();
    }
    public void PlayWind()
    {
        wind.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        wind.Play();

    }
    public void PlayShadow()
    {
        stun.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        stun.Play();
    }
    public void PlayFury()
    {
        fury.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        fury.Play();
    }
    public void PlayBlades()
    {
        blades.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        blades.Play();
    }
    public void PlayDash()
    {
        dash.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        dash.Play();
    }

    public void StartFury()
    {
        player.StartNonStop(furyDuration);
    }

    public void SpawnSwords()
    {
        CancelInvoke("StopSwords");
        swordsObj.SetActive(true);
        Invoke("StopSwords", bladesDuration);
    }
    void StopSwords()
    {
        swordsObj.SetActive(false);
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
            rb.velocity = (dashSpeed + player.originalSpeed) * -transform.up * animator.speed;

            if (player.isBoss && player.SceneName == "Boss")
                rb.velocity *= 3;

            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void SecondDash()
    {
        //if (FindObjectOfType<GameClient>() == null || (FindObjectOfType<GameClient>() != null && FindObjectOfType<GameHost>() != null))
        {
            gameObject.layer = 21;

            rb.velocity = (secondDashSpeed + player.originalSpeed) * -transform.up * animator.speed;

            if (player.isBoss && player.SceneName == "Boss")
                rb.velocity *= 3;

            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void SpawnWave()
    {
        GameObject wav = Instantiate(wave);
        wav.transform.position = wavePos.transform.position;
        wav.GetComponent<Rigidbody2D>().velocity = waveSpeed * (-limbs.transform.up);
        wav.transform.eulerAngles = limbs.transform.eulerAngles;
        hitbox waveHitbox = wav.GetComponent<hitbox>();
        waveHitbox.parent = gameObject;
        waveHitbox.damage = waveDamage + weaponDamage;
        waveHitbox.knockBackSpeed = waveKnock;
        waveHitbox.stunTime = waveStun;
        wav.GetComponent<wave>().active = true;
        waveHitbox.applyParentDamage = true;
    }
    public GameObject[] swords;
    public GameObject swordsObj;
    List<hitbox> swordScripts = new List<hitbox>();
    public float rotate;
    void Reset()
    {
        if (player.bot == false)
        {
            //if (swords != null)
            //{
                for (int i = 0; i < swordScripts.Count; i++)
                {
                    try
                    {
                        swordScripts[i].ResetPlayers();
                    }
                    catch
                    {

                    }
                }
            //}
        }
        Invoke("Reset", 0.1f);
    }
    void Update()
    {
        if (player.bot == false)
        {
            if (swordsObj.activeInHierarchy)
                swordsObj.transform.Rotate(0, 0, PublicVariables.deltaTime * rotate * (1+(float)player.dex/10f));
        }
    }
    private void LateUpdate()
    {
        if (player.bot == false)
        {
            if (swordsObj.activeInHierarchy)
                swordRot.transform.eulerAngles = Vector3.zero;
        }
    }

    public GameObject swordRot;
}
