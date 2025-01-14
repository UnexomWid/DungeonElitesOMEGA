using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapAttempt : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = "Generation Attempt: " + DungeonData.instance.attempt++;  
    }
}
