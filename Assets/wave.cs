using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wave : MonoBehaviour
{
    public float grow;
    public float timeAlive;
    public float dissapearCoef;
    public bool active = false;
    bool dissapear = false;
    SpriteRenderer spriteRenderer;
    Transform myTransform;
    // Update is called once per frame
    void Start()
    {
        myTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (active)
            Invoke("Dissapear", timeAlive);
    }
    void Dissapear()
    {
        dissapear = true;
    }
    float t = 0;
    void Update()
    {
        if (active)
        {
            float size = myTransform.localScale.x + PublicVariables.deltaTime * grow;

            myTransform.localScale = new Vector3(size, size, 1);
            if(dissapear)
            {
                t += PublicVariables.deltaTime * dissapearCoef;
                if (t >= 1)
                {
                    Destroy(gameObject);
                }
                else spriteRenderer.color = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 0), t);
            }
        }
    }
}
