using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceshard_appear : MonoBehaviour
{
    public bool active = false;
    bool appear = true;
    public float timeMultiplier;
    float t = 0;

    SpriteRenderer spriteRenderer;
    // Update is called once per frame
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (active)
            spriteRenderer.color = new Color32(255, 255, 255, 0);
    }
    void Update()
    {
        if(appear)
        {
            t += PublicVariables.deltaTime*timeMultiplier;
            if(t>=1)
            {
                t = 1;
                appear = false;
            }
            spriteRenderer.color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), t);
        }
    }
}
