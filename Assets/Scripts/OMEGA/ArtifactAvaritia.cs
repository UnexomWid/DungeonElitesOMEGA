using UnityEngine;

namespace OMEGA
{
    public class ArtifactAvaritia : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            player p = other.gameObject.GetComponent<player>();

            if (p == null || p.bot)
            {
                return;
            }

            Claim(p);
        }

        public void Claim(player p)
        {
            Events.OnClaimArtifact();

            p.SpawnPurchaseVfx();

            Destroy(gameObject);
        }
    }
}
