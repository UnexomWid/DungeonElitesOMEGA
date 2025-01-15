using UnityEngine;
using TMPro;

public class MapAttempt : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "Generation Attempt: " + DungeonData.instance.attempt++;  
    }
}
