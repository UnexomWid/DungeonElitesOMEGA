using UnityEngine;
using TMPro;

public class MapAttempt : MonoBehaviour
{
    static TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (DungeonData.instance.attempt++ > 1)
        {
            text.text = "Recalculating...";
        }

        text.text = "Generating dungeon...";
    }

    public static void Update(string what)
    {
        Debug.Assert(text != null, "Map attempt is null, but a script wants to update it");

        text.text = what;
    }
}
