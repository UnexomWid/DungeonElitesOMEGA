using System.Collections.Generic;
using UnityEngine;

namespace OMEGA
{
    public static class Data
    {
        public enum TrialMode
        {
            VANILLA,

            AVARITIA,
            ACEDIA,
            SUPERBIA,
            IRA,
            INVIDIA,
            GULA,
            LUXURIA
        }

        public enum CharacterID
        {
            WIZARD  = 1,
            KNIGHT  = 2,
            ARCHER  = 3,
            TANK    = 4,
            SUPPORT = 5
        }

        public static TrialMode trialMode = TrialMode.VANILLA;

        public static List<player> players;

        public static void Reset()
        {
            trialMode = TrialMode.VANILLA;
            players = new List<player>();
        }

        public static void OnEnterSceneWithPlayers(List<GameObject> list)
        {
            players.Clear();

            foreach (GameObject p in list)
            {
                if (p != null)
                {
                    var comp = p.GetComponent<player>();

                    if (comp != null)
                    {
                        players.Add(comp);
                    }
                }
            }

            if (trialMode != TrialMode.VANILLA)
            {
                UnlockMaxAvailableStatPoints();
            }
        }

        static void UnlockMaxAvailableStatPoints()
        {
            foreach (player p in players)
            {
                p.pointsAvailable = 30;

                // Golden player color
                if (p.limbColors != null)
                {
                    foreach (OriginalColor limb in p.limbColors)
                    {
                        if (limb != null)
                        {
                            limb.color = new Color32(246, 217, 0, 255);
                            limb.predefinedColor = limb.color;
                            limb.GetComponent<SpriteRenderer>().color = limb.color;
                        }
                    }
                }
            }

            Log.Info("Unlocked player max available stat points");
        }
    }
}