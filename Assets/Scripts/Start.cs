using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void StartScene()
        {
        FindObjectOfType<MapSelect>().NextMap();
    }

    public void Dungeon()
    {
        bool ok = false;
        foreach(PlayerData data in FindObjectsOfType<PlayerData>())
        {
            if(data.enabled && data.idCaracter >= 1 && data.idCaracter <= 5)
            {
                ok = true;
            }
        }
        if (ok)
        {
            if (PlayerPrefs.GetInt("Intro", 1) == 1)
                SceneManager.LoadScene("Intro");
            else SceneManager.LoadScene("Shops");
        }
    }

}
