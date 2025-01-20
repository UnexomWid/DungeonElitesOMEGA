using UnityEngine;

namespace OMEGA
{
    public class ArtifactAvaritia : MonoBehaviour
    {
        bool claimed;

        private void Awake()
        {
            claimed = false;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (claimed)
            {
                return;
            }

            player p = other.gameObject.GetComponent<player>();

            if (p == null || p.bot)
            {
                return;
            }

            Claim(p);
        }

        public void Claim(player p)
        {
            claimed = true;

            Events.OnClaimArtifact();

            p.SpawnPurchaseVfx();

            Destroy(gameObject);
        }
    }
}
