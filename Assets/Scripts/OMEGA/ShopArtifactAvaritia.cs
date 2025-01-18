using UnityEngine;

namespace OMEGA
{
    public class ShopArtifactAvaritia : MonoBehaviour
    {
        void Awake()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            player p = other.gameObject.GetComponent<player>();

            if (p == null || p.bot)
            {
                return;
            }

            Data.trialMode = Data.TrialMode.AVARITIA;

            p.SpawnPurchaseVfx();

            Data.ColorPlayers();

            Destroy(gameObject);
        }
    }
}
