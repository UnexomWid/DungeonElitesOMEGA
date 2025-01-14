using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public bool halfVolume = false;

    public bool dontDestroy = false;
    public bool noEffect = false;
    public AudioClip intro;
    public AudioClip loop;

    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetFloat("SFXVolume", 5) == 5)
            PlayerPrefs.SetFloat("SFXVolume", 0.5f);
        if (PlayerPrefs.GetFloat("MusicVolume", 5) == 5)
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        if (noEffect == false)
        {
            if (intro != null)
            {
                GetComponent<AudioSource>().clip = intro;
                source = GetComponent<AudioSource>();
                source.Play();
                source.loop = false;
            }

            if (dontDestroy)
            {
                if (FindObjectsOfType<Music>().Length >= 2)
                    Destroy(gameObject);
                else DontDestroyOnLoad(gameObject);
            }
        }
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    private void Update()
    {
        if(source != null && noEffect == false)
        {
            if(source.isPlaying == false)
            {
                source.clip = loop;
                source.Play();
                source.loop = true;
                this.enabled = false;
            }
        }
    }
}
