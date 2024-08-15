using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginalColor : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        if (alreadyColor)
            Color = predefinedColor;
        else
        {
            Color = GetComponent<SpriteRenderer>().color;
        }
    }
    public Color32 Color
    {
        get
        {
            return color;
        }
        set
        {
            if(value.r != 255 || value.g != 0 || value.b != 0)
            color = value;
        }
    }

    public Color32 color;

    public Color32 predefinedColor;
    public bool alreadyColor = false;
}
