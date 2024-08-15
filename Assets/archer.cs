using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class archer : MonoBehaviour
{
    public GameObject dashEffect;
    
    public GameObject arrow;
    public GameObject[] positions;
    public GameObject[] rotations;
    public float arrowSpeed;
    public float arrowDamage;
    public float arrowKnock;
    public float dashSpeed;
    public AudioSource attack;
    public AudioSource attack2;
    public AudioSource boost;
    public AudioSource dash;

    public float startArrowSpeed;
    public float startArrowDamage;
    public float startArrowKnock;
    public float startDashSpeed;
    public float startTripleDamage;
    public float startTripleKnock;
    public float startTripleSpeed;
    public float startRapidDamage;
    public float startRapidKnock;
    public float startRapidSpeed;
    public float startPoisonDamage;
    public float startPoisonKnock;
    public float startPoisonSpeed;
    public float startStunDamage;
    public float startStunKnock;
    public float startStunSpeed;
    public float startStunTime;
    public float startKnockDamage;
    public float startKnockSpeed;
    public float startKnockPower;
    public float startBackDamage;
    public float startBackKnock;
    public float startBackSpeed;
    public float startBackDashPower;
    public float startBonusSpeedSpeed;
    public float startBonusSpeedTime;

    public float bonusArrowSpeed;
    public float bonusArrowDamage;
    public float bonusArrowKnock;
    public float bonusDashSpeed;
    public float bonusTripleDamage;
    public float bonusTripleKnock;
    public float bonusTripleSpeed;
    public float bonusRapidDamage;
    public float bonusRapidKnock;
    public float bonusRapidSpeed;
    public float bonusPoisonDamage;
    public float bonusPoisonKnock;
    public float bonusPoisonSpeed;
    public float bonusStunDamage;
    public float bonusStunKnock;
    public float bonusStunSpeed;
    public float bonusStunTime;
    public float bonusKnockDamage;
    public float bonusKnockSpeed;
    public float bonusKnockPower;
    public float bonusBackDamage;
    public float bonusBackKnock;
    public float bonusBackSpeed;
    public float bonusBackDashPower;
    public float bonusBonusSpeedSpeed;
    public float bonusBonusSpeedTime;

    player player;
    public void Start()
    {
        player = GetComponent<player>();
        arrow.GetComponent<hitbox>().saveParticlesWhenDestroyed = true;
        poisonArrow.GetComponent<hitbox>().saveParticlesWhenDestroyed = true;
        stunArrow.GetComponent<hitbox>().saveParticlesWhenDestroyed = true;
        pushArrow.GetComponent<hitbox>().saveParticlesWhenDestroyed = true;

        if (player.bot || player.isBoss || player.finalBoss)
        {
            startArrowDamage *= player.difficultyPercent;
            startTripleDamage *= player.difficultyPercent;
            startRapidDamage *= player.difficultyPercent;
            startPoisonDamage *= player.difficultyPercent;
            startStunDamage *= player.difficultyPercent;
            startKnockDamage *= player.difficultyPercent;
            startBackDamage *= player.difficultyPercent;
        }

        List<setting> settings = new List<setting>();
        settings.Add(new setting("arrowDamage", startArrowDamage));
        settings.Add(new setting("arrowKnock", startArrowKnock));
        settings.Add(new setting("arrowSpeed", startArrowSpeed));
        settings.Add(new setting("tripleDamage", startTripleDamage));
        settings.Add(new setting("tripleKnock", startTripleKnock));
        settings.Add(new setting("tripleSpeed", startTripleSpeed));
        settings.Add(new setting("rapidDamage", startRapidDamage));
        settings.Add(new setting("rapidKnock", startRapidKnock));
        settings.Add(new setting("rapidSpeed", startRapidSpeed));
        settings.Add(new setting("poisonDamage", startPoisonDamage));
        settings.Add(new setting("poisonKnock", startPoisonKnock));
        settings.Add(new setting("poisonSpeed", startPoisonSpeed));
        settings.Add(new setting("stunDamage", startStunDamage));
        settings.Add(new setting("stunKnock", startStunKnock));
        settings.Add(new setting("stunSpeed", startStunSpeed));
        settings.Add(new setting("stunTime", startStunTime));
        settings.Add(new setting("knockDamage", startKnockDamage));
        settings.Add(new setting("knockPower", startKnockPower));
        settings.Add(new setting("knockSpeed", startKnockSpeed));
        settings.Add(new setting("dashPower", startDashSpeed));
        settings.Add(new setting("backDamage", startBackDamage));
        settings.Add(new setting("backKnock", startBackKnock));
        settings.Add(new setting("backSpeed", startBackSpeed));
        settings.Add(new setting("backDashPower", startBackDashPower));
        settings.Add(new setting("bonusSpeedSpeed", startBonusSpeedSpeed));
        settings.Add(new setting("bonusSpeedTime", startBonusSpeedTime));
        SetUpHitBoxes(settings);
        if(player.bot == false)
        SetUpText();
        player.baseDamage = startArrowDamage;
    }

    public void LevelAbil(int abilNumber, bool perfect)
    {
        int multiplier = 1;
        if (perfect)
            multiplier = 3;
        if (player.attacks[abilNumber] == "attack1")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("tripleDamage", tripleDamage + bonusTripleDamage*multiplier));
            settings.Add(new setting("tripleKnock", tripleKnock + bonusTripleKnock*multiplier));
            settings.Add(new setting("tripleSpeed", tripleSpeed + bonusTripleSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack2")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("rapidDamage", rapidDamage + bonusRapidDamage*multiplier));
            settings.Add(new setting("rapidKnock", rapidKnock + bonusRapidKnock*multiplier));
            settings.Add(new setting("rapidSpeed", rapidSpeed + bonusRapidSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack3")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("poisonDamage", poisonDamage + bonusPoisonDamage*multiplier));
            settings.Add(new setting("poisonKnock", poisonKnock + bonusPoisonKnock*multiplier));
            settings.Add(new setting("poisonSpeed", poisonSpeed + bonusPoisonSpeed * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack4")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("dashPower", dashSpeed + bonusDashSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack5")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("knockDamage", knockDamage + bonusKnockDamage*multiplier));
            settings.Add(new setting("knockPower", knockPower + bonusKnockPower*multiplier));
            settings.Add(new setting("knockSpeed", knockSpeed + bonusKnockSpeed*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack6")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("stunDamage", stunDamage + bonusStunDamage*multiplier));
            settings.Add(new setting("stunKnock", stunKnock + bonusStunKnock*multiplier));
            settings.Add(new setting("stunSpeed", stunSpeed + bonusStunSpeed*multiplier));
            settings.Add(new setting("stunTime", stunTime + bonusStunTime*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack7")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("backDamage", backDamage + bonusBackDamage*multiplier));
            settings.Add(new setting("backKnock", backKnock + bonusBackKnock*multiplier));
            settings.Add(new setting("backSpeed", backSpeed + bonusBackSpeed*multiplier));
            settings.Add(new setting("backDashPower", backDashPower+ bonusBackDashPower*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack8")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("bonusSpeedSpeed", bonusSpeedSpeed + bonusBonusSpeedSpeed*multiplier));
            settings.Add(new setting("bonusSpeedTime", bonusSpeedTime + bonusBonusSpeedTime*multiplier));
            SetUpHitBoxes(settings);
        }
        player.currentCooldowns[abilNumber] -= player.cooldownReduce[abilNumber] * multiplier;
        if(player.bot == false && player.inDungeon)
            SetUpText();
    }

    public float tripleDamage;
    public float tripleKnock;
    public float tripleSpeed;
    public float rapidDamage;
    public float rapidKnock;
    public float rapidSpeed;
    public float poisonDamage;
    public float poisonKnock;
    public float poisonSpeed;
    public float stunDamage;
    public float stunKnock;
    public float stunSpeed;
    public float stunTime;
    public float knockDamage;
    public float knockSpeed;
    public float knockPower;
    public float backDamage;
    public float backKnock;
    public float backSpeed;
    public float backDashPower;
    public float bonusSpeedSpeed;
    public float bonusSpeedTime;
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
    public void SetUpHitBoxes(List<setting> settings)
    {
        foreach (setting setting in settings)
        {
            if (setting.name == "arrowDamage")
            {
                arrowDamage = setting.value;
            }
            else if (setting.name == "arrowKnock")
            {
                arrowKnock = setting.value;
            }
            else if (setting.name == "arrowSpeed")
            {
                arrowSpeed = setting.value;
            }
            else if (setting.name == "tripleDamage")
            {
                tripleDamage = setting.value;
            }
            else if (setting.name == "tripleKnock")
            {
                tripleKnock = setting.value;
            }
            else if (setting.name == "tripleSpeed")
            {
                tripleSpeed = setting.value;
            }
            else if (setting.name == "rapidDamage")
            {
                rapidDamage = setting.value;
            }
            else if (setting.name == "rapidKnock")
            {
                rapidKnock = setting.value;
            }
            else if (setting.name == "rapidSpeed")
            {
                rapidSpeed = setting.value;
            }
            else if (setting.name == "poisonDamage")
            {
                poisonDamage = setting.value;
            }
            else if (setting.name == "poisonKnock")
            {
                poisonKnock = setting.value;
            }
            else if (setting.name == "poisonSpeed")
            {
                poisonSpeed = setting.value;
            }
            else if (setting.name == "stunDamage")
            {
                stunDamage = setting.value;
            }
            else if (setting.name == "stunKnock")
            {
                stunKnock = setting.value;
            }
            else if (setting.name == "stunSpeed")
            {
                stunSpeed = setting.value;
            }
            else if (setting.name == "stunTime")
            {
                stunTime = setting.value;
            }
            else if (setting.name == "knockDamage")
            {
                knockDamage = setting.value;
            }
            else if (setting.name == "knockSpeed")
            {
                knockSpeed = setting.value;
            }
            else if (setting.name == "knockPower")
            {
                knockPower = setting.value;
            }
            else if (setting.name == "dashPower")
            {
                dashSpeed = setting.value;
            }
            else if (setting.name == "backDamage")
            {
                backDamage = setting.value;
            }
            else if (setting.name == "backKnock")
            {
                backKnock = setting.value;
            }
            else if (setting.name == "backSpeed")
            {
                backSpeed = setting.value;
            }
            else if (setting.name == "backDashPower")
            {
                backDashPower = setting.value;
            }
            else if (setting.name == "bonusSpeedSpeed")
            {
                bonusSpeedSpeed = setting.value;
            }
            else if (setting.name == "bonusSpeedTime")
            {
                bonusSpeedTime = setting.value;
            }
        }
    }

       

    public void SetUpText()
    {
        for (int i = 0; i < player.attacks.Length; i++)
        {
            if (player.attacks[i].Contains("1"))
            {
                player.abilityDescs[i].abilityText.text = "Shoots 3 arrows at slightly different angles, each dealing <color=red><b>" + (tripleDamage + arrowDamage) + "</b></color> damage";
            }
            else if (player.attacks[i].Contains("2"))
            {
                player.abilityDescs[i].abilityText.text = "Rapidly shoots 6 consecutive arrows, each dealing <color=red><b>" + (rapidDamage + arrowDamage) + "</b></color> damage.";
            }
            else if (player.attacks[i].Contains("3"))
            {
                player.abilityDescs[i].abilityText.text = "Shoots an arrow that deals <color=red><b>" + (poisonDamage+arrowDamage) + "</b></color> damage and creates a pool of poison that <color=brown><b>slows</b></color> it's enemies on impact and <color=green><b>poisons</b></color> it's target.";
            }
            else if (player.attacks[i].Contains("4"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes with a speed of <color=purple><b>" + (dashSpeed + player.originalSpeed) + "</b></color>. Can do this 3 times in the span of 3 seconds.";
            }
            else if (player.attacks[i].Contains("5"))
            {
                player.abilityDescs[i].abilityText.text = "Shoots an arrow that <color=black><b>heavily knockbacks</b></color> it's target dealing <color=red><b>"+(knockDamage+arrowDamage)+"</b></color> damage.";
            }
            else if (player.attacks[i].Contains("6"))
            {
                player.abilityDescs[i].abilityText.text = "Shoots an arrow that deals <color=red><b>" + (stunDamage+arrowDamage) + "</b></color> damage and <color=blue><b>stuns</b></color> it's target.";
            }
            else if (player.attacks[i].Contains("7"))
            {
                player.abilityDescs[i].abilityText.text = "Dashes backwards with a speed of <color=purple><b>"+(backDashPower+player.originalSpeed)+ "</b></color>, shooting an arrow that deals <color=red><b>" + (backDamage+arrowDamage)+"</b></color> damage.";
            }
            else if (player.attacks[i].Contains("8"))
            {
                player.abilityDescs[i].abilityText.text = "Gains a speed of <color=purple><b>" + (bonusSpeedSpeed) + "</b></color> in the span of " + bonusSpeedTime+" seconds.";
            }
            float cd;
            if (player.SceneName == "Shops")
                cd = player.currentCooldowns[i];
            else cd = player.currentCooldowns[i] - player.cooldownReduction / 100 * player.currentCooldowns[i];
            player.abilityDescs[i].abilityText.text += "\n<color=blue><b>Cooldown</b></color>: " + cd.ToString("0.00") + " seconds.";
        }
    }
    public void PlayAttack()
    {
        attack2.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack2.Play();

    }
    public void PlayAttack2()
    {
        attack2.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack2.Play();
    }
    public void PlayBoost()
    {
        boost.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        boost.Play();
    }
    public void PlayDash()
    {
        dash.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        dash.Play();
    }
    public void SidewaysDash()
    {
        //if (FindObjectOfType<GameClient>() == null || (FindObjectOfType<GameClient>() != null && FindObjectOfType<GameHost>() != null))
        {
            player.rb.velocity = (backDashPower + player.originalSpeed) * transform.up * player.animator.speed;
            GameObject dashClone = Instantiate(dashEffect);
            dashClone.transform.position = transform.position;
            dashClone.transform.eulerAngles = -transform.eulerAngles;
            Destroy(dashClone, 2f);
        }
    }
    public void StopDash()
    {
        player.rb.velocity = Vector2.zero;
    }
    public void SpawnNormalArrow()
    {
        GameObject arr = Instantiate(arrow);
        arr.transform.position = transform.position - transform.up * 2;
        arr.transform.eulerAngles = transform.eulerAngles;
        arr.GetComponent<Rigidbody2D>().velocity = arrowSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.damage = arrowDamage;
        arrHitbox.knockBackSpeed = arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }
    public void SpawnRapidArrow()
    {
        GameObject arr = Instantiate(arrow);
        arr.transform.position = positions[0].transform.position;
        arr.transform.rotation = rotations[0].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = rapidSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.damage = rapidDamage + arrowDamage;
        arrHitbox.knockBackSpeed = rapidKnock + arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }
    public void SpawnBackArrow()
    {
        GameObject arr = Instantiate(arrow);
        arr.transform.position = positions[0].transform.position;
        arr.transform.rotation = rotations[0].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = backSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.damage = backDamage + arrowDamage;
        arrHitbox.knockBackSpeed = backKnock + arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }

    public void TripleShot()
    {
        GameObject arr = Instantiate(arrow);
        arr.transform.position = positions[1].transform.position;
        arr.transform.rotation = rotations[1].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = tripleSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.damage = tripleDamage + arrowDamage;
        arrHitbox.knockBackSpeed = tripleKnock + arrowKnock;
        GameObject arr2 = Instantiate(arrow);
        arr2.transform.position = positions[2].transform.position;
        arr2.transform.rotation = rotations[2].transform.rotation;
        arr2.GetComponent<Rigidbody2D>().velocity = tripleSpeed * (-arr2.transform.up);
        hitbox arrHitbox2 = arr2.GetComponent<hitbox>();
        arrHitbox2.parent = gameObject;
        arrHitbox2.damage = tripleDamage + arrowDamage;
        arrHitbox2.knockBackSpeed = tripleKnock + arrowKnock;
        GameObject arr3 = Instantiate(arrow);
        arr3.transform.position = positions[3].transform.position;
        arr3.transform.rotation = rotations[3].transform.rotation;
        arr3.GetComponent<Rigidbody2D>().velocity = tripleSpeed * (-arr3.transform.up);
        hitbox arrHitbox3 = arr2.GetComponent<hitbox>();
        arrHitbox3.parent = gameObject;
        arrHitbox3.damage = tripleDamage + arrowDamage;
        arrHitbox3.knockBackSpeed = tripleKnock + arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
            arr2.transform.localScale *= 2;
            arr3.transform.localScale *= 2;
        }
    }
    public GameObject poisonArrow;
    public void PoisonArrow()
    {
        GameObject arr = Instantiate(poisonArrow);
        arr.transform.position = positions[4].transform.position;
        arr.transform.rotation = rotations[4].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = poisonSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.damage = poisonDamage + arrowDamage;
        arrHitbox.knockBackSpeed = poisonKnock + arrowKnock;
        arrHitbox.poison = true;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }
    public GameObject stunArrow;
    public void StunArrow()
    {
        GameObject arr = Instantiate(stunArrow);
        arr.transform.position = positions[6].transform.position;
        arr.transform.rotation = rotations[6].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = stunSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.shakeWhenHit = true;
        arrHitbox.shakeAmp = 0.25f;
        arrHitbox.damage = stunDamage + arrowDamage;
        arrHitbox.stunTime = stunTime;
        arrHitbox.knockBackSpeed = stunKnock + arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }
    public GameObject pushArrow;
    public float pushArrowPush;
    public void PushArrow()
    {
        GameObject arr = Instantiate(pushArrow);
        arr.transform.position = positions[4].transform.position;
        arr.transform.rotation = rotations[4].transform.rotation;
        arr.GetComponent<Rigidbody2D>().velocity = knockSpeed * (-arr.transform.up);
        hitbox arrHitbox = arr.GetComponent<hitbox>();
        arrHitbox.parent = gameObject;
        arrHitbox.shakeWhenHit = true;
        arrHitbox.shakeAmp = 0.25f;
        arrHitbox.damage = knockDamage + arrowDamage;
        arrHitbox.knockBackSpeed = knockPower + arrowKnock;
        if (player.isBoss)
        {
            arr.transform.localScale *= 2;
        }
    }
}
