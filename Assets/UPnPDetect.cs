using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UPnPDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            GetComponent<Text>().text = "UPnP: " + FindObjectOfType<GameHost>().upnp;
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }
}
