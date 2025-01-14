using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossPad : MonoBehaviour
{
    
    public Color32[] colors;
    public Image[] maps;
    public GameObject arrow;
    public GameObject map;
    void Awake()
    {
        DungeonData dungeonData = FindObjectOfType<DungeonData>();

        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].color = colors[dungeonData.maps[i] - 1];
        }
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();

        arrowRect.position = new Vector2(arrowRect.position.x + 320 * (dungeonData.currentMap - 1), arrowRect.position.y);
        GetComponent<Animator>().Play("arrive");
    }
}
