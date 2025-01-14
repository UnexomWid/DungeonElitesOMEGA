using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonOnline : MonoBehaviour
{
    public void StartOnlineGame()
    {
        FindObjectOfType<GameHost>().StartGame();
    }
}
