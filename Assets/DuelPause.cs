using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelPause : MonoBehaviour
{
    [SerializeField] GameObject pause;
    PlayerData playerData;
    player[] players;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            playerData = FindObjectOfType<PlayerData>();
        }
        catch
        {

        }
        try
        {
            players = FindObjectsOfType<player>();
        }
        catch
        {

        }
    }

    bool pressed2 = false;

    // Update is called once per frame
    void Update()
    {
        bool ok3 = true;

        try
        {
            if (pause.activeInHierarchy)
            {
                Time.timeScale = 0.000001f;
            }
        }
        catch
        {

        }
        foreach (player player in players)
        {

            try
            {
                if (player != null)
                {
                    if (player.bot == false)
                    {
                        if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.start == 1 && player.playerNumber != 5) : false) || ((Input.GetKeyDown(KeyCode.Escape) && player.playerNumber == 5 && player.ableToMove))))
                        {
                            ok3 = false;
                            if (pressed2 == false)
                            {
                                pressed2 = true;
                                if (!(pause.gameObject.activeInHierarchy == true))
                                    Time.timeScale = 0.000001f;
                                else Time.timeScale = 1f;
                                pause.SetActive(!pause.activeInHierarchy);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        if (ok3)
            pressed2 = false;
    }
}
