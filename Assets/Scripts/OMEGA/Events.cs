using System.Collections.Generic;
using UnityEngine;

namespace OMEGA
{
    public static class Events
    {
        public static void OnEnterSceneWithPlayers(List<GameObject> list)
        {
            Data.players.Clear();

            foreach (GameObject p in list)
            {
                if (p != null)
                {
                    var comp = p.GetComponent<player>();

                    if (comp != null)
                    {
                        Data.players.Add(comp);
                    }
                }
            }

            if (Data.trialMode != Data.TrialMode.VANILLA)
            {
                Data.UnlockMaxAvailableStatPoints();
            }
        }

        public static void OnDungeonGenerationRestarted()
        {
            
        }

        public static void OnDungeonGenerated()
        {

        }
    }
}