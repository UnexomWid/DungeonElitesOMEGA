using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteFog : MonoBehaviour
{
    bool shrinking = true;
    bool shrinking2 = false;
    bool growing = false;
    new SpriteRenderer renderer;

    Transform myTransform;
    private void OnEnable()
    {
        myTransform = transform;
        renderer = GetComponent<SpriteRenderer>();
        shrinking = true;
        shrinking2 = false;
        growing = false;
        t1 = 0;
        t2 = 0;
        t3 = 0;
    }

    float t1 = 0;
    float t2 = 0;
    float t3 = 0;
    private void Update()
    {
        if(shrinking)
        {
            t1 += Time.unscaledDeltaTime * 2;
            myTransform.localScale = Vector2.Lerp(new Vector2(2, 2), new Vector2(1.25f, 1.25f), t1);
            renderer.color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 128), t1);
            if(t1>=1)
            {
                shrinking2 = true;
                shrinking = false;
            }
        }
        else if(shrinking2)
        {
            t2 += Time.unscaledDeltaTime;
            myTransform.localScale = Vector2.Lerp(new Vector2(1.25f, 1.25f), new Vector2(1f, 1f), t2);
            renderer.color = Color32.Lerp(new Color32(255, 255, 255, 128), new Color32(255, 255, 255, 192), t2);
            if (t2 >= 1)
            {
                growing = true;
                shrinking2 = false;
            }
        }
        else if(growing)
        {
            t3 += Time.unscaledDeltaTime * 2;
            myTransform.localScale = Vector2.Lerp(new Vector2(1f, 1f), new Vector2(2f, 2f), t3);
            renderer.color = Color32.Lerp(new Color32(255, 255, 255, 192), new Color32(255, 255, 255, 0), t3);
            if (t3 >= 1)
            {
                growing = false;
                gameObject.SetActive(false);
            }
        }
    }
}
