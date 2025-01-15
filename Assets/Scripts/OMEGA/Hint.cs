using UnityEngine;
using TMPro;

namespace OMEGA {
    public class Hint : MonoBehaviour
    {
        static readonly string[] hints =
        {
            "The Fairy Tooth can save your soul up to 3 times",
            "Dexterity also affects cast time",
            "Enemy tanks may not be knocked back",
            "OMEGA's fireballs are deadly",
            "Traps inside the Snail Room deal damage based on MaxHP",
            "The Blood Potion can be used to revive a dungeon mate",
            "Enemies might spawn ghosts which block projectiles when they die"
        };

        void Awake()
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

            text.text = string.Format("{0}.", hints[Random.Range(0, hints.Length)]);
        }
    }
}