using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class wizard : MonoBehaviour
{
    public float startFireballSpeed;
    public float startFireballDamage;
    public float startFireballKnock;

    public float startStaffDamage;
    public float startStaffKnock;

    public float startSmallFireballSpeed;
    public float startSmallFireballDamage;
    public float startSmallFireballKnock;

    public float startSquareDamage;

    public float startPortalDamage;
    public float startTeleportDistance;

    public float startWindDamage;

    public float startRockDamage;

    public float startElectricityDamage;


    public float bonusFireballSpeed;
    public float bonusFireballDamage;
    public float bonusFireballKnock;

    public float bonusStaffDamage;
    public float bonusStaffKnock;

    public float bonusSmallFireballSpeed;
    public float bonusSmallFireballDamage;
    public float bonusSmallFireballKnock;

    public float bonusSquareDamage;

    public float bonusPortalDamage;
    public float bonusTeleportDistance;

    public float bonusWindDamage;

    public float bonusRockDamage;

    public float bonusElectricityDamage;

    public float bonusIceSpeed;
    public float bonusIceDamage;
    public float bonusIceKnock;

    public float startIceSpeed;
    public float startIceDamage;
    public float startIceKnock;
    public float fireballSpeed;
    public float fireballDamage;
    public float fireballKnock;

    public float staffDamage;
    public float staffKnock;

    public float smallFireballSpeed;
    public float smallFireballDamage;
    public float smallFireballKnock;

    public AudioSource electricity;
    public AudioSource attack;
    public AudioSource fireballSFX;
    public AudioSource teleport;
    public AudioSource wind;
    public AudioSource rocks;
    public AudioSource sq_spawn;
    public AudioSource sq_explode;

    public hitbox squareBox;
    public hitbox portalBox;
    public hitbox windBox;
    public hitbox rockBox;
    public hitbox electricityBox;
    public hitbox weapon;
    public hitbox[] rockHitboxes;

    public void SetWeaponDamage(float multiplier)
    {
        weapon.damage = staffDamage * multiplier;
    }
    public void SetWeaponKnock(float multiplier)
    {
        weapon.knockBackSpeed = staffKnock * multiplier;

        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("basic1") || stateInfo.IsName("basic2") || stateInfo.IsName("basic3") || stateInfo.IsName("basic1_nonstop"))
            weapon.knockBackSpeed += (player.originalSpeed - player.baseSpeed );
    }

    public void ActivateWeapon()
    {
        weaponHitbox.enabled = true;
    }
    public void DeactivateWeapon()
    {
        weaponHitbox.enabled = false;
    }

    private void OnDisable()
    {
        DeactivateWeapon();
        weapon.ResetPlayers();
    }

    public void LevelAbil(int abilNumber, bool perfect)
    {
        int multiplier = 1;
        if (perfect)
            multiplier = 3;
        if(player.attacks[abilNumber] == "attack1")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("fireballDamage", fireballDamage+bonusFireballDamage*multiplier));
            settings.Add(new setting("fireballSpeed", fireballSpeed+bonusFireballSpeed * multiplier));
            settings.Add(new setting("fireballKnock", fireballKnock+bonusFireballKnock * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack2")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("squareDamage", squareBox.damage+bonusSquareDamage * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack3")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("portalDamage", portalBox.damage + bonusPortalDamage * multiplier));
            settings.Add(new setting("portalTeleport", teleportDistance + bonusTeleportDistance * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack4")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("smallFireballDamage", smallFireballDamage + bonusSmallFireballDamage * multiplier));
            settings.Add(new setting("smallFireballSpeed", smallFireballSpeed + bonusSmallFireballSpeed * multiplier));
            settings.Add(new setting("smallFireballKnock", smallFireballKnock + bonusSmallFireballKnock * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack5")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("iceDamage", iceDamage + bonusIceDamage*multiplier));
            settings.Add(new setting("iceSpeed", iceSpeed + bonusIceSpeed*multiplier));
            settings.Add(new setting("iceKnock", iceKnock + bonusIceKnock * multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack6")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("windDamage", windBox.damage + bonusWindDamage*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack7")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("rockDamage", rockBox.damage + bonusRockDamage*multiplier));
            SetUpHitBoxes(settings);
        }
        else if (player.attacks[abilNumber] == "attack8")
        {
            List<setting> settings = new List<setting>();
            settings.Add(new setting("electricityDamage", electricityBox.damage + bonusElectricityDamage*multiplier));
            SetUpHitBoxes(settings);
        }
        player.currentCooldowns[abilNumber] -= player.cooldownReduce[abilNumber] * multiplier;
        if (player.bot == false)
            SetUpText();
    }


    player player;
    BoxCollider2D weaponHitbox;
    bool notInShops;
    bool notInTutorial;
    Transform myTransform;
    public void Start()
    {
        myTransform = transform;
        notInShops = SceneManager.GetActiveScene().name != "Shops";
        notInTutorial = SceneManager.GetActiveScene().name != "TutorialScene";
        weaponHitbox = weapon.GetComponent<BoxCollider2D>();
        player = GetComponent<player>();

        if (player.bot || player.isBoss || player.finalBoss)
        {
            startFireballDamage *= player.difficultyPercent;
            startSmallFireballDamage *= player.difficultyPercent;
            startSquareDamage *= player.difficultyPercent;
            startPortalDamage *= player.difficultyPercent;
            startWindDamage *= player.difficultyPercent;
            startRockDamage *= player.difficultyPercent;
            startElectricityDamage *= player.difficultyPercent;
            startIceDamage *= player.difficultyPercent;
            startStaffDamage *= player.difficultyPercent;
        }

        List<setting> settings = new List<setting>();
        settings.Add(new setting("fireballDamage", startFireballDamage));
        settings.Add(new setting("fireballSpeed", startFireballSpeed));
        settings.Add(new setting("fireballKnock", startFireballKnock));
        settings.Add(new setting("smallFireballDamage", startSmallFireballDamage));
        settings.Add(new setting("smallFireballSpeed", startSmallFireballSpeed));
        settings.Add(new setting("smallFireballKnock", startSmallFireballKnock));
        settings.Add(new setting("squareDamage", startSquareDamage));
        settings.Add(new setting("portalDamage", startPortalDamage));
        settings.Add(new setting("portalTeleport", startTeleportDistance));
        settings.Add(new setting("windDamage", startWindDamage));
        settings.Add(new setting("rockDamage", startRockDamage));
        settings.Add(new setting("electricityDamage", startElectricityDamage));
        settings.Add(new setting("iceDamage", startIceDamage));
        settings.Add(new setting("iceSpeed", startIceSpeed));
        settings.Add(new setting("iceKnock", startIceKnock));
        settings.Add(new setting("staffDamage", startStaffDamage));
        settings.Add(new setting("staffKnock", startStaffKnock));

        SetUpHitBoxes(settings);
        if(player.bot == false && player.inDungeon)
            SetUpText();

        fireball.GetComponent<hitbox>().shakeWhenHit = true;
        fireball.GetComponent<hitbox>().shakeAmp = 0.1f;
        fireball.GetComponent<hitbox>().useRBVelForParticles = true;
        ParticleSystem.ShapeModule shape = fireball.GetComponent<hitbox>().impactVfx.GetComponent<ParticleSystem>().shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.radius = 1;
        shape.angle = 30;
        ParticleSystem.MainModule main = fireball.GetComponent<hitbox>().impactVfx.GetComponent<ParticleSystem>().main;
        main.startSpeed = 10;

        iceShard.GetComponent<hitbox>().shakeWhenHit = true;
        iceShard.GetComponent<hitbox>().shakeAmp = 0.06f;
        iceShard.GetComponent<hitbox>().useRBVelForParticles = true;
        iceShard.GetComponent<hitbox>().sfxWhenHitMob = false;
        ParticleSystem.ShapeModule shape2 = iceShard.GetComponent<hitbox>().impactVfx.GetComponent<ParticleSystem>().shape;
        shape2.shapeType = ParticleSystemShapeType.Cone;
        shape2.radius = 1;
        shape2.angle = 30;
        ParticleSystem.MainModule main2 = iceShard.GetComponent<hitbox>().impactVfx.GetComponent<ParticleSystem>().main;
        main2.startSpeed = 10;
        player.baseDamage = startStaffDamage;
    }
    public void SetUpText()
    {
        try
        {
            for (int i = 0; i < player.attacks.Length; i++)
            {
                if (player.attacks[i].Contains("1"))
                {
                    player.abilityDescs[i].abilityText.text = "Shoots a fireball. Upon impact, deals <color=red><b>" + (fireballDamage + staffDamage) + "</b></color> damage and <color=orange><b>ignites</b></color> the target.";
                }
                else if (player.attacks[i].Contains("2"))
                {
                    player.abilityDescs[i].abilityText.text = "Creates a square around and detonates it dealing <color=red><b>" + (squareBox.damage + staffDamage) + "</b></color> damage to all enemies hit.";
                }
                else if (player.attacks[i].Contains("3"))
                {
                    player.abilityDescs[i].abilityText.text = "Teleports <color=purple><b>" + (teleportDistance + player.originalSpeed) + "</b></color> units forward and deals <color=red><b>" + (portalBox.damage + staffDamage) + "</b></color> damage to nearby enemies when landing.";
                }
                else if (player.attacks[i].Contains("4"))
                {
                    player.abilityDescs[i].abilityText.text = "Shoots a barrage of fireballs all around, each dealing <color=red><b>" + (smallFireballDamage + staffDamage) + "</b></color> damage and <color=orange><b>igniting</b></color> the target.";
                }
                else if (player.attacks[i].Contains("5"))
                {
                    player.abilityDescs[i].abilityText.text = "Shoots 3 icicles, each dealing <color=red><b>" + (iceDamage + staffDamage) + "</b></color> damage and <color=aqua><b>freezing</b></color> the target.";
                }
                else if (player.attacks[i].Contains("6"))
                {
                    player.abilityDescs[i].abilityText.text = "Blows wind dealing <color=red><b>" + (windBox.damage + staffDamage) + "</b></color> damage and pushing aside all enemies hit.";
                }
                else if (player.attacks[i].Contains("7"))
                {
                    player.abilityDescs[i].abilityText.text = "Spawns 3 rocks creating a triangle and brings them together crushing all enemies hit and dealing <color=red><b>" + (rockBox.damage + staffDamage) + "</b></color> damage.";
                }
                else if (player.attacks[i].Contains("8"))
                {
                    player.abilityDescs[i].abilityText.text = "Fires lightning dealing <color=red><b>" + (electricityBox.damage + staffDamage) + "</b></color> damage. Enemies close to the <color=yellow><b>electrocuted</b></color> targets will also be hit.";
                }
                float cd;
                if (SceneManager.GetActiveScene().name == "Shops")
                    cd = player.currentCooldowns[i];
                else cd = player.currentCooldowns[i] - player.cooldownReduction / 100 * player.currentCooldowns[i];
                player.abilityDescs[i].abilityText.text += "\n<color=blue><b>Cooldown</b></color>: " + cd.ToString("0.00") + " seconds.";
            }
        }
        catch
        {

        }
    }
    public void SetUpHitBoxes(List<setting> settings)
    {
        foreach (setting setting in settings)
        {
            if(setting.name == "fireballDamage")
            {
                fireballDamage = setting.value;
            }
            else if (setting.name == "fireballSpeed")
            {
                fireballSpeed = setting.value;
            }
            else if (setting.name == "fireballKnock")
            {
                fireballKnock = setting.value;
            }
            else if (setting.name == "smallFireballDamage")
            {
                smallFireballDamage = setting.value;
            }
            else if (setting.name == "smallFireballSpeed")
            {
                smallFireballSpeed = setting.value;
            }
            else if (setting.name == "smallFireballKnock")
            {
                smallFireballKnock = setting.value;
            }
            else if (setting.name == "squareDamage")
            {
                squareBox.damage = setting.value;
            }
            else if (setting.name == "portalDamage")
            {
                portalBox.damage = setting.value;
            }
            else if (setting.name == "portalTeleport")
            {
                teleportDistance = setting.value;
            }
            else if (setting.name == "windDamage")
            {
                windBox.damage = setting.value;
            }
            else if (setting.name == "rockDamage")
            {
                rockBox.damage = setting.value;
            }
            else if (setting.name == "electricityDamage")
            {
                electricityBox.damage = setting.value;
            }
            else if (setting.name == "iceDamage")
            {
                iceDamage = setting.value;
            }
            else if (setting.name == "iceSpeed")
            {
                iceSpeed = setting.value;
            }
            else if (setting.name == "iceKnock")
            {
                iceKnock = setting.value;
            }
            else if (setting.name == "staffDamage")
            {
                staffDamage = setting.value;
                weapon.damage = staffDamage;
            }
            else if (setting.name == "staffKnock")
            {
                staffKnock = setting.value;
                weapon.knockBackSpeed = staffKnock;
            }
        }
    }

    public void PlaySqSpawn()
    {
        sq_spawn.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        sq_spawn.Play();
    }
    public void PlaySqExplode()
    {
        sq_explode.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        sq_explode.Play();
    }
    public void PlayElectricity()
    {
        electricity.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        electricity.Play();
    }
    public void PlayAttack()
    {
        attack.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        attack.Play();
    }
    public void PlayFireball()
    {
        fireballSFX.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        fireballSFX.Play();
    }
    public void PlayTeleporty()
    {
        teleport.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        teleport.Play();
    }
    public void PlayWind()
    {
        wind.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        wind.Play();
    }
    public void PlayRocks()
    {
        rocks.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        rocks.Play();
    }


    public GameObject fireball;
    public GameObject fbPos;
    public float teleportDistance;
    public GameObject limbs;
    public float iceSpeed;
    public float iceDamage;
    public float iceKnock;
    public GameObject iceShard;
    public GameObject icePos1;
    public GameObject icePos2;
    public GameObject icePos3;
    public void ResetCh(bool resetDamage)
    {
        if (resetDamage)
        {
            SetWeaponDamage(1);
            SetWeaponKnock(1);
        }
        weaponHitbox.enabled = false;
    }
    public void SpawnFireball(int bigger = 0)
    {
        GameObject fb = Instantiate(fireball);
        fb.transform.position = myTransform.position - limbs.transform.up * 2;
        fb.GetComponent<Rigidbody2D>().velocity = fireballSpeed * (-limbs.transform.up);
        fb.transform.eulerAngles = limbs.transform.eulerAngles;
        hitbox fbHitbox = fb.GetComponent<hitbox>();
        fbHitbox.parent = gameObject;
        fbHitbox.damage = fireballDamage + staffDamage;
        fbHitbox.knockBackSpeed = fireballKnock;
        fb.transform.localScale = fb.transform.localScale*2;
        if (bigger == 0)
        {
            fbHitbox.damage = smallFireballDamage + staffDamage;
            fbHitbox.knockBackSpeed = smallFireballKnock;
            fb.transform.localScale = fb.transform.localScale/2;
            fb.GetComponent<Rigidbody2D>().velocity = smallFireballSpeed * (-limbs.transform.up);
        }
        if(player.isBoss)
        {
            fb.transform.localScale *= 2;
        }
    }
    public void SpawnIceShards()
    {
        GameObject ice = Instantiate(iceShard);
        ice.transform.position = icePos1.transform.position;
        ice.transform.eulerAngles = limbs.transform.eulerAngles;
        ice.GetComponent<hitbox>().parent = gameObject;
        is1 = ice;
        if (player.isBoss)
        {
            ice.transform.localScale *= 2;
        }
        ice.GetComponent<iceshard_appear>().active = true;
        if (player.bot == false || player.isBoss)
        {
            GameObject ice2 = Instantiate(iceShard);
            ice2.transform.position = icePos2.transform.position;
            ice2.transform.eulerAngles = limbs.transform.eulerAngles;
            ice2.GetComponent<hitbox>().parent = gameObject;
            is2 = ice2;
            if (player.isBoss)
            {
                ice2.transform.localScale *= 2;
            }
            ice2.GetComponent<iceshard_appear>().active = true;
            GameObject ice3 = Instantiate(iceShard);
            ice3.transform.position = icePos3.transform.position;
            ice3.transform.eulerAngles = limbs.transform.eulerAngles;
            ice3.GetComponent<hitbox>().parent = gameObject;
            is3 = ice3;
            if (player.isBoss)
            {
                ice3.transform.localScale *= 2;
            }
            ice3.GetComponent<iceshard_appear>().active = true;
            Destroy(ice2, 3f);
            Destroy(ice3, 3f);
        }
        Destroy(ice, 3f);
    }
    GameObject is1;
    GameObject is2;
    GameObject is3;
    public void ShootIceShards()
    {
        try
        {
            is1.GetComponent<hitbox>().knockBackSpeed = iceKnock;
            is1.GetComponent<hitbox>().damage = iceDamage + staffDamage;
            is1.GetComponent<BoxCollider2D>().enabled = true;
            is1.GetComponent<Rigidbody2D>().velocity = iceSpeed * (-is1.transform.up);
        }
        catch
        {

        }
        if (player.bot == false || player.isBoss)
        {
            try
            {
                is2.GetComponent<hitbox>().knockBackSpeed = iceKnock;
                is2.GetComponent<hitbox>().damage = iceDamage + staffDamage;
                is2.GetComponent<BoxCollider2D>().enabled = true;
                is2.GetComponent<Rigidbody2D>().velocity = iceSpeed * (-is1.transform.up);
            }
            catch
            {

            }
            try
            {
                is3.GetComponent<hitbox>().knockBackSpeed = iceKnock;
                is3.GetComponent<hitbox>().damage = iceDamage + staffDamage;
                is3.GetComponent<BoxCollider2D>().enabled = true;
                is3.GetComponent<Rigidbody2D>().velocity = iceSpeed * (-is1.transform.up);
            }
            catch
            {

            }
        }
        is1 = null;
        is2 = null;
        is3 = null;
    }

    public GameObject grParticles;
    public GameObject grParticlesPos;

    public void SpawnGroundParticles()
    {
        GameObject groundParticles = Instantiate(grParticles);
        groundParticles.transform.position = grParticlesPos.transform.position;
        if (player.isBoss)
        {
            groundParticles.transform.localScale *= 2;
        }
        Destroy(groundParticles, 1f);
    }

    public GameObject portalParticles;

    public void SpawnPortalParticles()
    {
        GameObject prPar = Instantiate(portalParticles);
        prPar.transform.position = myTransform.position;
        if (player.isBoss)
        {
            prPar.transform.localScale *= 2;
        }
        Destroy(prPar, 1f);
    }

    public void Teleport()
    {
        float dist = teleportDistance;
        if(player.online)
        {
            myTransform.position = myTransform.position - myTransform.up * (dist + player.originalSpeed);
        }
        else while(dist + player.originalSpeed > 0)
        {
            if (CheckGoodPos(dist) == true||(player.inDungeon == false&&notInShops))
            {
                myTransform.position = myTransform.position - myTransform.up * (dist + player.originalSpeed);
                player.oldPos = player.newPos = myTransform.position;
                break;
            }
            else dist -= 0.5f;
        }
        
    }
    public bool CheckGoodPos(float dist)
    {
        Collider2D[] boxes = Physics2D.OverlapBoxAll(myTransform.position - myTransform.up * (dist + player.originalSpeed), new Vector2(0.0001f, 0.0001f), 0f);
        //Collider2D[] boxes2 = Physics2D.OverlapBoxAll(myTransform.position, new Vector2(0.0001f, 0.0001f), 0f);
        bool ok = false;
        if(!notInShops || !notInTutorial)
        {
            foreach (Collider2D box in boxes)
            {
                if (box.GetComponent<DestroyBullets>() != null && box.isTrigger == false)
                {
                    return false;
                }
            }
            return true;
        }
        else if (player.locked)
        {
            foreach (Collider2D box in boxes)
            {
                if (box==player.currentBox)
                {
                    ok = true;
                }
                if(box.gameObject.name.Contains("door"))
                {
                    ok = false;
                    break;
                }
            }
        }
        else
        {
            /*bool breakable = false;
            GameObject currentRoom = null;
            for(int i=0;i<boxes2.Length;i++)
            {
                room boxRoom = boxes2[i].GetComponent<room>();
                if (boxRoom != null)
                {
                    if (boxRoom.lockable)
                    {
                        currentRoom = boxes2[i].gameObject;
                        break;
                    }
                }
            }*/

            for(int i=0;i<boxes.Length;i++)
                {
                    if (boxes[i].GetComponent<room>() != null && boxes[i].transform.name.Contains("deadEnd") == false)
                    {
                        ok = true;
                    }
                }

            if (ok)
            {
                Collider2D[] boxes3 = Physics2D.OverlapBoxAll(myTransform.position - myTransform.up * (dist + player.originalSpeed - 0.001f), new Vector2(0.0001f, 0.0001f), 0f);

                ok = false;

                for (int i = 0; i < boxes3.Length; i++)
                {
                    if (boxes3[i].GetComponent<room>() != null && boxes[i].transform.name.Contains("deadEnd") == false)
                    {
                        ok = true;
                    }
                }
            }

        }
        return ok;
    }
}
