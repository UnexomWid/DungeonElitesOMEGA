using UnityEngine;
using TMPro;

public class GameVer : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "Dungeon Elites " + Application.version;
    }
}
