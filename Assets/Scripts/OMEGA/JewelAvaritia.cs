using UnityEngine;

namespace OMEGA
{
    public class JewelAvaritia : MonoBehaviour
    {
        void Awake()
        {
            if (Data.trialMode != Data.TrialMode.AVARITIA || Data.claimedArtifacts == 0)
            {
                Destroy(gameObject);
                return;
            }

            Data.jewel = gameObject;
        }
    }
}