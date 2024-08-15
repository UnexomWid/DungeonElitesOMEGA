using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float currentTime = 1;
    public AudioSource slowMoSfx;

    public static TimeController instance;

    private void Start()
    {
        instance = this;
    }

    public void SlowMotion()
    {
        CameraFollow.instance.deathEffect.SetActive(false);
        CameraFollow.instance.deathEffect.SetActive(true);
        Time.timeScale = currentTime = 0.1f;
        foreach(AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (source.GetComponent<Music>() == null)
                source.volume = 0;
            else source.volume = PlayerPrefs.GetFloat("MusicVolume", 1) / 2;
        }
        slowMoSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        slowMoSfx.Play();
        Invoke("UndoSlowMo", 0.2f);
    }

    void UndoSlowMo()
    {
        CameraFollow.instance.deathEffect.SetActive(false);
        Time.timeScale = currentTime = 1f;
        FindObjectOfType<Music>().GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1);
    }
}
