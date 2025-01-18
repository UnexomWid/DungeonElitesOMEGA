using UnityEngine;

namespace OMEGA
{
    public class ArtifactAvaritia : MonoBehaviour
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

            Events.OnClaimArtifact();

            p.SpawnPurchaseVfx();

            Destroy(gameObject);
        }
    }
}
