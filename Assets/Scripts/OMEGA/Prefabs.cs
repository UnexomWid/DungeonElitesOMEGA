using UnityEngine;

namespace OMEGA
{
    public class Prefabs : MonoBehaviour
    {
        public static Prefabs instance;

        public GameObject artifactAvaritia;

        void Awake()
        {
            // DebugMaster already calls DontDestroyOnLoad on the same GameObject
            instance = this;
        }
    }
}