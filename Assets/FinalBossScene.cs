using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossScene : MonoBehaviour
{
    public GameObject music;
    public AudioClip shakeSound;
    public GameObject secondFaseMusic;
    public GameObject oldMap;
    public GameObject newMap;
    public SpriteRenderer fog;
    public Collider2D box;

    public GameObject[] players;
    public GameObject[] playerObjs;

    public void InvokeNextFaze()
    {
        music.SetActive(false);
        Invoke("LowShake", 5f);
        Invoke("MediumShake", 6f);
        Invoke("HighShake", 7f);
        Invoke("NextFaze", 8f);
    }

    CameraFollow cameraFollow;
    private void Awake()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
    }

    void LowShake()
    {
        AudioSource.PlayClipAtPoint(shakeSound, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1f));
        cameraFollow.Shake(2f, 0.5f);
    }

    void MediumShake()
    {
        cameraFollow.Shake(2f, 1.25f);
    }

    void HighShake()
    {
        cameraFollow.Shake(2f, 2f);
    }

    void NextFaze()
    {
        fog.color = new Color32(255, 255, 255, 192);
        secondFaseMusic.SetActive(true);
        oldMap.SetActive(false);
        newMap.SetActive(true);
        cameraFollow.Shake(2f, 0f);

        for(int i=0;i<players.Length;i++)
        {
            if(players[i] != null)
            {
                players[i].GetComponent<player>().currentBox = box;
                players[i].transform.position = playerObjs[i].transform.position;
            }
        }
    }
}
