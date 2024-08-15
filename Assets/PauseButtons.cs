using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButtons : MonoBehaviour
{
    public GameObject highlight;

    Slider slider;

    public void MoreVolume()
    {
        slider.value += 0.1f;
    }

    public void LessVolume()
    {
        slider.value -= 0.1f;
    }

    public void Highlight()
    {
        highlight.SetActive(true);
    }

    public void Unhighlight()
    {
        highlight.SetActive(false);
    }

    public void Shops()
    {
        Destroy(FindObjectOfType<DungeonData>());
        SceneManager.LoadScene("Shops");
    }

    public void LoadDuel()
    {
        foreach (PlayerData data in FindObjectsOfType<PlayerData>())
        {
            Destroy(data.gameObject);
        }
        Destroy(FindObjectOfType<DungeonData>());
        Destroy(FindObjectOfType<MapSelect>());
        Destroy(FindObjectOfType<ControllerReconnect>());
        SceneManager.LoadScene("Select");
    }

    public void Lobby()
    {
        foreach (PlayerData data in FindObjectsOfType<PlayerData>())
        {
            Destroy(data.gameObject);
        }
        Destroy(FindObjectOfType<DungeonData>());
        Destroy(FindObjectOfType<ControllerReconnect>());
        SceneManager.LoadScene("SelectDungeon");
    }

    public void MainMenu()
    {
        foreach (PlayerData data in FindObjectsOfType<PlayerData>())
        {
            Destroy(data.gameObject);
        }
        Destroy(FindObjectOfType<DungeonData>());
            Destroy(FindObjectOfType<ControllerReconnect>());
        SceneManager.LoadScene("Start");
    }

    public void Resume()
    {
        FindObjectOfType<InventorySpawn>().TriggerPause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public Slider sfx;

    public string type;
    public void Press()
    {
        if (type == "MainMenu")
            MainMenu();
        else if (type == "Quit")
            Quit();
        else if (type == "Shops")
            Shops();
        else if (type == "Lobby")
            Lobby();
        else if (type == "LoadDuel")
            LoadDuel();
        else if (type == "Resume")
            Resume();
    }
    private void Start()
    {
        slider = GetComponent<Slider>();
        if (sfx != null)
        {
            sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        }

        if (music != null)
        {
            music.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        }
    }

    public void SFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfx.value);
    }

    public Slider music;

    public void MusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", music.value);
        Music musicObj = FindObjectOfType<Music>();
        if (musicObj != null)
        {
            if(musicObj.halfVolume)
                musicObj.GetComponent<AudioSource>().volume = music.value/2;
            else musicObj.GetComponent<AudioSource>().volume = music.value;
        }
    }
}
