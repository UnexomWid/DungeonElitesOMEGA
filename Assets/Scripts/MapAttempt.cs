using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MapAttempt : MonoBehaviour
{
    static TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (SceneManager.GetActiveScene().name == "Boss")
        {
            MapAttempt.Update("I am OMEGA.");
        }
        else if (DungeonData.instance.attempt++ > 1)
        {
            text.text = "Recalculating...";
        }
        else
        {
            text.text = "Generating dungeon...";
        }
    }

    public static void Update(string what)
    {
        Debug.Assert(text != null, "Map attempt is null, but a script wants to update it");

        text.text = what;
    }
}
