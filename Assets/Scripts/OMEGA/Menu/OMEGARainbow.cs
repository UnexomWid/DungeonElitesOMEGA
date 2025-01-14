using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace OMEGA
{
    public class OMEGARainbow : MonoBehaviour
    {
        public float speed = 1f;

        float clock;
        TextMeshProUGUI shadow;

        [SerializeField]
        TextMeshProUGUI text;

        void Awake()
        {
            clock = 0f;

            shadow = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            float val = Mathf.Min(clock, 2f - clock);

            text.color = Color.HSVToRGB(0f, 0f, val);

            float shadowVal;

            if (val < 0.35f)
            {
                shadowVal = 1f;
            }
            else if (val < 0.65f)
            {
                // Smooth transition
                shadowVal = MathUtil.Map(val, 0.35f, 0.65f, 1f, 0f);
            }
            else
            {
                shadowVal = 0f;
            }

            shadow.color = Color.HSVToRGB(0f, 0f, shadowVal);

            clock = (clock + speed * Time.deltaTime) % 2f;
        }
    }
}