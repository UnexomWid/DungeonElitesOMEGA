using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public AudioSource impact;

    private void Start()
    {
        PlayImpact();
    }

    public void PlayImpact()
    {
        impact.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        impact.Play();
    }
    public void LoadFinalScene()
    {
        SceneManager.LoadScene("Outro");
    }
}
