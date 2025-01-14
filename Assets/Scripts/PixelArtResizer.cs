using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelArtResizer : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public int multiplier;
    public SpriteRenderer test;
    // Start is called before the first frame update
    Texture2D Resize(Texture2D texture, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
    void Start()
    {
        foreach(SpriteRenderer spriteRenderer in sprites)
        {
            Sprite sprite = spriteRenderer.sprite;
            Texture2D texture = sprite.texture;
            Texture2D newTexture = Resize(texture, texture.width * multiplier, texture.height * multiplier);
            test.sprite = Sprite.Create(newTexture, new Rect(0,0,newTexture.width,newTexture.height), new Vector2(0.5f,0.5f));
        }
    }
}
