using UnityEngine;
using UnityEngine.UI;

namespace OMEGA
{
    public class UIArtifact : MonoBehaviour
    {
        void Awake()
        {
            if (Data.trialMode != Data.TrialMode.VANILLA)
            {
                var img = GetComponent<Image>();

                img.color = Color.white;
                Log.Info(Data.trialMode.ToPrettyString());
                img.sprite = Resources.Load<Sprite>(string.Format("Sprites/OMEGA/Artifact{0}", Data.trialMode.ToPrettyString()));
            }
        }
    }
}