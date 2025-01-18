using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public const int STAT_SHOP_BASE_COST = 100;
        public const int STAT_SHOP_AVARITIA_COST_MODIFIER = 10;

        public const int STAT_SHOP_BASE_MAX = 10;
        public const int STAT_SHOP_AVARITIA_MAX = 30;

        public const int ITEM_SHOP_BASE_PRICE = 300;

        public const int BEAR_TRAP_BASE_DAMAGE = 200;

        public const int BOSS_BASE_MAX_ATTACKS = 7;
        public const int OMEGA_BASE_HIT_TOLERANCE = 5;

        public const float AVARITIA_NEGATIVE_FACTOR_PER_CHALICE = 1.5f;
        public const float AVARITIA_POSITIVE_FACTOR_PER_CHALICE = 2f;

        public const int AVARITIA_ITEM_PRICE_INCREASE = 50;
        public const float AVARITIA_ITEM_PRICE_INCREASE_FACTOR = 2f;

        public const int AVARITIA_BOSS_MAX_ATTACKS_INCREASE = 1;
        public const int AVARITIA_OMEGA_HIT_TOLERANCE_INCREASE = 1;

        public static TrialMode trialMode = TrialMode.VANILLA;

        public static List<player> players;

        public static int claimedArtifacts;

        public static void Reset()
        {
            trialMode = TrialMode.VANILLA;
            players = new List<player>();
            claimedArtifacts = 0;
        }

        public static void UnlockMaxAvailableStatPoints()
        {
            foreach (player p in players)
            {
                p.pointsAvailable = 30;
            }

            Log.Info("Unlocked player max available stat points");
        }

        public static int GetCoinIncome(int count)
        {
            switch (trialMode)
            {
                case TrialMode.AVARITIA:
                {
                    return (int)(count * GetGoodAvaritiaFactor()) * players.Count;
                }
            }

            return count;
        }

        public static int GetStatShopCost(player p)
        {
            int cost = STAT_SHOP_BASE_COST;

            if (trialMode == TrialMode.AVARITIA)
            {
                int totalPoints = 0;

                totalPoints += p.hp;
                totalPoints += p.reg;
                totalPoints += p.spd;
                totalPoints += p.dex;
                totalPoints += p.str;
                totalPoints += p.cdr;
                totalPoints += p.pointsLeft;

                cost += STAT_SHOP_AVARITIA_COST_MODIFIER * totalPoints;
                cost = (int)(cost * GetBadAvaritiaFactor());
            }

            return cost;
        }

        public static int GetStatShopMax()
        {
            if (trialMode == TrialMode.AVARITIA)
            {
                return STAT_SHOP_AVARITIA_MAX;
            }

            return STAT_SHOP_BASE_MAX;
        }

        public static float GetBearTrapDamage()
        {
            float dmg = (float)(BEAR_TRAP_BASE_DAMAGE * DungeonData.instance.currentMap);

            if (trialMode == TrialMode.AVARITIA)
            {
                dmg *= GetBadAvaritiaFactor();
            }

            return dmg;
        }

        public static int GetMobThreatLevel()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Boss":
                {
                    return 5;
                }
                case "TutorialScene":
                {
                    return 1;
                }
                case "Shops":
                {
                    return 2;
                }
                default:
                {
                    switch (trialMode)
                    {
                        case TrialMode.AVARITIA:
                        {
                            return System.Math.Min(DungeonData.instance.currentMap + claimedArtifacts, 5);
                        }
                        default:
                        {
                            return DungeonData.instance.currentMap;
                        }
                    }
                }
            }
        }

        public static int GetBossMaxAttacks()
        {
            int max = BOSS_BASE_MAX_ATTACKS;

            if (trialMode == TrialMode.AVARITIA)
            {
                max += claimedArtifacts * AVARITIA_BOSS_MAX_ATTACKS_INCREASE;
            }

            return max;
        }

        // OMEGA's health scales with the number of claimed artifacts
        public static int GetOMEGAHitTolerance()
        {
            int tolerance = OMEGA_BASE_HIT_TOLERANCE;

            tolerance += claimedArtifacts * AVARITIA_OMEGA_HIT_TOLERANCE_INCREASE;

            return tolerance;
        }

        // For bad things like prices and debuffs
        public static float GetBadAvaritiaFactor()
        {
            return Mathf.Pow(AVARITIA_NEGATIVE_FACTOR_PER_CHALICE, claimedArtifacts);
        }

        // For good things like income
        public static float GetGoodAvaritiaFactor()
        {
            return Mathf.Pow(AVARITIA_POSITIVE_FACTOR_PER_CHALICE, claimedArtifacts);
        }

        public static void ColorPlayers()
        {
            switch (trialMode)
            {
                case TrialMode.AVARITIA:
                {
                    ColorPlayers(new Color32(246, 217, 0, 255));
                    break;
                }
            }
        }

        static void ColorPlayers(Color color)
        {
            foreach (player p in players)
            {
                if (p.limbColors != null)
                {
                    foreach (OriginalColor limb in p.limbColors)
                    {
                        if (limb != null)
                        {
                            limb.color = color;
                            limb.predefinedColor = limb.color;
                            limb.GetComponent<SpriteRenderer>().color = limb.color;
                        }
                    }
                }
            }
        }
    }
}