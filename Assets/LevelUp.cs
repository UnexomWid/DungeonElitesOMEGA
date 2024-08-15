using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        if(audioSource != null)
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
