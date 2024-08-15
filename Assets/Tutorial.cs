using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject[] arrows;
    [SerializeField] int[] arrowIndexes;

    public float speed = 50;
    public GameObject tutorialObj;
    public Text text;
    public PhoneAbillities image;
    bool onPhone;

    int letterIndex = 1;

    public string[] tutorialText;

    public int textIndex = 1;

    CameraFollow cameraFollow;
    PlayerData playerData;

    void SpawnArrows(int index)
    {
#if UNITY_ANDROID || UNITY_IPHONE
        for (int i=0;i<arrows.Length;i++)
            {
                arrows[i].SetActive(false);

                if(arrowIndexes[i] == index)
                {
                    arrows[i].SetActive(true);
                }
            }
#endif
    }

    void Start()
    {
        //if(PlayerPrefs.GetInt("Tutorial", 1) == 1)
        //{
            tutorialObj.SetActive(true);
            text.text = tutorialText[0][0].ToString();
            SpawnArrows(0);
            letterIndex = 1;
            //Invoke("NextLetter", speed / 1000);
        //}
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        playerData = FindObjectOfType<PlayerData>();

        StartCoroutine(NextLetter());
    }

    private void OnDisable()
    {
        for (int i = 0; i < arrows.Length; i++)
            arrows[i].SetActive(false);

        StopAllCoroutines();
    }

    bool pressed = false;

    IEnumerator NextLetter()
    {
        while (true)
        {
            if (letterIndex < tutorialText[textIndex - 1].Length)
            {
                text.text += tutorialText[textIndex - 1][letterIndex++].ToString();
            }

            yield return new WaitForSecondsRealtime(speed / 1500);
        }
    }

    public void Disable()
    {
        OnDisable();

        tutorialObj.SetActive(false);

        TutorialFinished?.Invoke();
    }

    public void SetQuote(int index)
    {
        textIndex = index;
        try
        {
            SpawnArrows(textIndex);
            text.text = tutorialText[textIndex++][0].ToString();
            letterIndex = 1;
            //Invoke("NextLetter", speed / 1000);
        }
        catch
        {
            tutorialObj.SetActive(false);

            TutorialFinished?.Invoke();
        }
    }

    public void NextQuote()
    {
        try
        {
            SpawnArrows(textIndex);
            text.text = tutorialText[textIndex++][0].ToString();
            letterIndex = 1;
            //Invoke("NextLetter", speed / 1000);
        }
        catch
        {
            if (FindObjectOfType<MultiplatformTutorial>().shops)
                PlayerPrefs.SetInt("ShopsTutorial", 0);

            tutorialObj.SetActive(false);

            TutorialFinished?.Invoke();
        }
    }

    public delegate void Notify();

    public event Notify TutorialFinished;
}
