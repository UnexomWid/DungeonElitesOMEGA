using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public bool fog = false;
    public Vector2 startSize;
    public Vector2 endSize;
    public float growMultiplier;
    float t;

    public static Fog instance;

    SpriteRenderer spriteRenderer;
    Transform myTransform;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        myTransform = transform;

        instance = this;
    }

    public void BeginFog()
    {
        fog = true;
    }
    public void StopFog()
    {
        fog = false;
    }
    void Update()
    {
        if(fog)
        {
            t += PublicVariables.deltaTime * growMultiplier;
            if (t >= 1)
                t = 1;
            myTransform.localScale = Vector2.Lerp(startSize, endSize, t);
            spriteRenderer.color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), t);
        }
        else
        {
            t -= PublicVariables.deltaTime * growMultiplier;
            if (t <= 0)
                t = 0;
            myTransform.localScale = Vector2.Lerp(startSize, endSize, t);
            spriteRenderer.color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), t);
        }
    }
}
