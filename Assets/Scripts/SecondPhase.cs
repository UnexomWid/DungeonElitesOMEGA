using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhase : MonoBehaviour
{
    public player boss;
    public GameObject[] bosses;
    public AstarPath pathfinder;
    public AudioClip morph;

    public GameObject nexusesObj;
    public GameObject map;

    public SpriteRenderer noBlockMap;

    public AudioClip rise;
    public AudioClip impact;

    public AudioClip damage;

    public RoomPlayers block;

    public GameObject music;

    public AudioClip dramaticSound;

    public GameObject thirdPhaseBoss;

    public GameObject playerCamera;
    public GameObject bossCamera;

    public GameObject thirdMapMask;

    public ThirdPhase thirdPhase;

    List<player> bossScripts = new List<player>();

    public void NextPhase()
    {
        foreach (GameObject boss in bosses)
            boss.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = false;
        AudioSource.PlayClipAtPoint(rise, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        nexusesObj.SetActive(false);
        map.SetActive(true);
        fallingMap = true;
        CancelInvoke("MorphBoss");
        CancelInvoke("ActivateNexuses");

        music.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1) / 2;
        music.GetComponent<Music>().halfVolume = true;
    }

    bool fallingMap = false;
    float t = 0;

    private void Update()
    {
        if(fallingMap)
        {
            if(t<1)
            {
                t += PublicVariables.deltaTime / 4;

                noBlockMap.color = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 0, 0, 255), t);

                if(t>=1)
                {
                    noBlockMap.gameObject.SetActive(false);

                    AudioSource.PlayClipAtPoint(impact, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 0.5f));

                    foreach(player player in FindObjectsOfType<player>())
                    {
                        if (block.players.Contains(player) == false)
                        {
                            player.EliminatePlayer();
                            AudioSource.PlayClipAtPoint(damage, Camera.main.transform.position,  PlayerPrefs.GetFloat("SFXVolume", 0.5f));
                        }
                    }

                    GetComponent<PolygonCollider2D>().enabled = false;

                    Invoke("ActivateThirdPhase", 4f);
                }
            }
        }
        else
        {
            foreach(player bossScript in bossScripts)
            {
                if(boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
                {
                    NextPhase();
                }
            }
        }
    }

    void ActivateThirdPhase()
    {
        AudioSource.PlayClipAtPoint(dramaticSound, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 0.5f));

        thirdMapMask.SetActive(true);

        thirdPhaseBoss.SetActive(true);

        FindObjectOfType<Cursor>().transform.parent = bossCamera.transform;

        playerCamera.SetActive(false);

        bossCamera.GetComponent<CameraFollow>().players = new List<GameObject>(playerCamera.GetComponent<CameraFollow>().players);

        bossCamera.SetActive(true);

        foreach(player player in FindObjectsOfType<player>())
        {
            player.cameraFollow = bossCamera.GetComponent<CameraFollow>();
        }

        Invoke("ActivateMusic", 8f);
    }

    void ActivateMusic()
    {
        music.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        music.GetComponent<Music>().halfVolume = false;

        thirdPhase.enabled = true;
        this.enabled = false;
    }






    List<Nexus> nexuses = new List<Nexus>();
    public void AddNexus(Nexus nexus)
    {
        if(nexuses.Contains(nexus) == false)
        {
            nexuses.Add(nexus);
            if(nexuses.Count == 5)
            {
                boss.Stun(6f);
                boss.Animator.Play("idle");
                Invoke("ActivateNexuses", 6f);
                CancelInvoke("MorphBoss");
                Invoke("MorphBoss", 10f);
            }
        }
    }

    void ActivateNexuses()
    {
        foreach(Nexus nexus in nexuses)
        {
            nexus.Reactivate();
        }
        nexuses.Clear();
    }

    private void Start()
    {
        foreach(GameObject boss in bosses)
        {
            bossScripts.Add(boss.GetComponent<player>());
        }
        pathfinder.Scan();
        Invoke("BossDrop", 5f);
    }

    void BossDrop()
    {
        boss = bosses[Random.Range(1, 99999) % bosses.Length].GetComponent<player>();
        boss.gameObject.SetActive(true);
        boss.GetComponent<player>().SetSecondPhaseBossHealth(50000);
        AudioSource.PlayClipAtPoint(morph, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1f));
        Invoke("MorphBoss", 10f);
    }

    void MorphBoss()
    {
        List<GameObject> newBosses = new List<GameObject>(bosses);
        newBosses.Remove(boss.gameObject);

        player newBoss = newBosses[Random.Range(1, 999999) % newBosses.Count].GetComponent<player>();
        newBoss.transform.position = boss.transform.position;

        boss.gameObject.SetActive(false);
        boss = newBoss;

        newBoss.SetBossHp(boss.health);

        newBoss.gameObject.SetActive(true);

        newBoss.SetT(1);

        AudioSource.PlayClipAtPoint(morph, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1f));
        Invoke("MorphBoss", 10f);
    }
}
