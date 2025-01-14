using UnityEngine;

namespace OMEGA
{
    public class ShopChalice : MonoBehaviour
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

            //AvaritiaData.OnEnableAvaritiaMode();

            p.SpawnPurchaseVfx();

            Destroy(gameObject);
        }
    }
}
