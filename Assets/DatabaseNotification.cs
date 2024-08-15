using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseNotification : MonoBehaviour
{
    private void Start()
    {
        if(PlayerPrefs.GetInt("DatabaseNotification", 0) == 0)
        {
            Invoke("Notify", 1.5f);
        }
    }

    void Notify()
    {
        GetComponent<Animator>().Play("notify");
    }

    public void StopNotify()
    {
        PlayerPrefs.SetInt("DatabaseNotification", 1);

        GetComponent<Animator>().Play("stopNotify");
    }
}
