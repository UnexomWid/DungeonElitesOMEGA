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
                Data.ColorPlayers();
            }
        }

        public static void OnClaimArtifact()
        {
            Data.claimedArtifacts++;
            Data.artifact = null;

            if (Data.trialMode == Data.TrialMode.AVARITIA)
            {
                foreach (var menu in Data.itemShop.itemMenus)
                {
                    menu.GetComponent<itemMenu>().UpdatePrice();
                }
            }
        }

        public static void OnResetPermPoints(ref float hp, ref float reg, ref float spd, ref float dex, ref float str, ref float cdr)
        {
            if (Data.trialMode != Data.TrialMode.VANILLA)
            {
                hp = 0f;
                reg = 0f;
                spd = 0f;
                dex = 0f;
                str = 0f;
                cdr = 0f;
            }
        }

        public static void OnSpawnParkourChest(ref GameObject chest)
        {
            if (Data.trialMode == Data.TrialMode.AVARITIA)
            {
                var artifact = GameObject.Instantiate(Prefabs.instance.artifactAvaritia);
                artifact.layer = chest.layer;

                var tform = artifact.GetComponent<Transform>();
                var chestTform = chest.GetComponent<Transform>();

                tform.parent = chestTform.parent;
                tform.position = new Vector3(chestTform.position.x, chestTform.position.y, -5f);

                GameObject.Destroy(chest);

                chest = artifact;

                Data.artifact = artifact;
            }
        }

        public static void OnSpawnMob(player mob)
        {
            if (Data.trialMode == Data.TrialMode.VANILLA || DungeonData.instance == null || mob.isBoss)
            {
                return;
            }

            if (Data.trialMode != Data.TrialMode.AVARITIA)
            {
                // More stuff coming soon
                return;
            }

            // Mob time between attacks is shorter in Avaritia mode
            // (depends on the current level, not on the number of claimed artifacts)

            int level = DungeonData.instance.currentMap;

            mob.timeBetweenAttacks /= level;

            if (level == 5 && mob.caracterId == (int)Data.CharacterID.KNIGHT)
            {
                mob.timeBetweenAttacks = 0f;
            }

            mob.tBA = mob.timeBetweenAttacks;

            // Mobs have more attacks based on the number of claimed chalices

            if (mob.caracterId == (int)Data.CharacterID.SUPPORT)
            {
                // Supports don't attack
                return;
            }

            // Number of attacks
            int attacks = System.Math.Max(Data.claimedArtifacts, 0) + 1; // + Basic attack

            mob.botAttacks = new string[attacks];

            // All available attacks for the mob to choose from
            List<string> attackBook = new List<string>(7);

            switch (mob.caracterId)
            {
                case (int)Data.CharacterID.WIZARD:
                {
                    mob.botAttacks[0] = "attack1"; // Basic attack

                    for (int i = 2; i <= 8; ++i)
                    {
                        attackBook.Add("attack" + i.ToString()); // 2 -> 8
                    }

                    break;
                }
                case (int)Data.CharacterID.KNIGHT:
                {
                    mob.botAttacks[0] = "botBasicAttack"; // Basic attack

                    for (int i = 1; i <= 7; ++i)
                    {
                        attackBook.Add("attack" + i.ToString()); // 1 -> 7
                    }

                    break;
                }
                case (int)Data.CharacterID.ARCHER:
                case (int)Data.CharacterID.TANK:
                {
                    mob.botAttacks[0] = "botAttack"; // Basic attack

                    for (int i = 1; i <= 7; ++i)
                    {
                        attackBook.Add("attack" + i.ToString()); // 1 -> 7
                    }

                    break;
                }
            }

            // More attacks, based on the number of claimed artifacts
            for (int i = 1; i < attacks; ++i)
            {
                int chosen = UnityEngine.Random.Range(0, attackBook.Count);

                mob.botAttacks[i] = attackBook[chosen];

                attackBook.RemoveAt(chosen);
            }
        }

        public static void OnBuyItem(itemMenu menu)
        {
            // TODO: cursed items
            Data.boughtItems++;

            if (Data.trialMode == Data.TrialMode.AVARITIA)
            {
                // Restock
                int index = -1;

                for (int i = 0; i < Data.itemShop.itemMenus.Length; ++i)
                {
                    if (menu.gameObject == Data.itemShop.itemMenus[i])
                    {
                        Data.itemShop.numbers[i] = -1;

                        int num;
                        do
                        {
                            num = UnityEngine.Random.Range(1, 999999) % (Data.itemShop.maxNum + 1);
                        }
                        while (Data.itemShop.numbers.Contains(num));

                        Data.itemShop.numbers[i] = num;

                        menu.SetUp((Items.ID)num);

                        index = i;

                        break;
                    }
                }

                if (index != -1)
                {
                    // Update price for the other items since 'boughtItems' changed
                    for (int i = 0; i < Data.itemShop.itemMenus.Length; ++i)
                    {
                        if (i == index)
                        {
                            continue;
                        }

                        Data.itemShop.itemMenus[i].GetComponent<itemMenu>().UpdatePrice();
                    }
                }
            }
        }

        public static void OnHitOMEGA(FinalBossHitbox boss)
        {
            if (Data.trialMode == Data.TrialMode.AVARITIA && boss.damage == Data.claimedArtifacts && boss.damage < Data.GetOMEGAHitTolerance() && Data.jewel != null)
            {
                Data.jewel.GetComponent<Animator>().SetTrigger("Trigger");
            }
        }

        public static void OnDungeonGenerationRestarted()
        {
            
        }

        public static void OnDungeonGenerated()
        {
            Data.itemShop = GameObject.FindObjectOfType<ItemShop>();
        }
    }
}