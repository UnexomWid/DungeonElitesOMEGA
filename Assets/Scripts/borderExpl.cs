using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class borderExpl : MonoBehaviour
{
    public Sprite readySprite;
    public Sprite explSprite;
    public AudioSource explSfx;

    SpriteRenderer sprite;
    PolygonCollider2D collider;
    hitbox hitbox;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<PolygonCollider2D>();
        hitbox = GetComponent<hitbox>();
        Invoke("Mark", 3.5f);
    }

    public void StopBorder()
    {
        StopExpl();
        CancelInvoke("Mark");
        CancelInvoke("Explode");
        CancelInvoke("StopExpl");
    }

    void Mark()
    {
        sprite.sprite = readySprite;
        Invoke("Explode", 1.5f);
    }

    void Explode()
    {
        sprite.sprite = explSprite;
        collider.enabled = true;
        if (explSfx != null)
        {
            explSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            explSfx.Play();
        }
        Invoke("StopExpl", 0.25f);
    }

    void StopExpl()
    {
        sprite.sprite = null;
        collider.enabled = false;
        hitbox.ResetPlayers();
        Invoke("Mark", 3.5f);
    }
}
