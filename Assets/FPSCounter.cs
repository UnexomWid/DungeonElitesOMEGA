using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public static FPSCounter instance;

    bool showFPS = false;
    public void ShowFPS()
    {
        if (PlayerPrefs.GetInt("FPS", 1) == 1)
            showFPS = false;
        else showFPS = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectsOfType<FPSCounter>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        instance = this;

        ShowFPS();
    }

    GUIStyle guiStyle = new GUIStyle();
    // Update is called once per frame

    float t = 1;
    int lastFps;
    void OnGUI()
    {
        if (showFPS)
        {
            t += Time.unscaledDeltaTime;

            if (t > 1)
            {
                lastFps = (int)(1f / Time.unscaledDeltaTime);
                t = 0;
            }

            guiStyle.fontSize = Screen.width / 90;
            guiStyle.normal.textColor = Color.green;
            GUI.Label(new Rect(25, 25, Screen.width / 9, Screen.width / 9), lastFps + " fps", guiStyle);
        }
    }
}
