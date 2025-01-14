using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escape : MonoBehaviour
{
    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "OnlineLobby")
            {
                try
                {
                    FindObjectOfType<GameClient>().Disconnect();
                    Destroy(FindObjectOfType<GameClient>().gameObject);
                    FindObjectOfType<GameHost>().Disconnect();
                    Destroy(FindObjectOfType<GameHost>().gameObject);
                }
                catch
                {

                }
            }
            foreach(PlayerData data in FindObjectsOfType<PlayerData>())
            {
                Destroy(data.gameObject);
            }
            SceneManager.LoadScene("Start");
        }

    }*/
}
