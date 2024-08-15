using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    public bool active = false;
    public GameObject particles;
    public player parent;
    // Start is called before the first frame update
    void Start()
    {
        if(active)
        {
            Invoke("Explode", 1f);
        }
    }
    void Explode()
    {
        foreach(AudioSource source in Camera.main.GetComponents<AudioSource>())
        {
            if(source.isPlaying==false)
            {
                source.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                source.Play();
                break;
            }
        }
        GameObject vfx = Instantiate(particles);
        Destroy(vfx, 1f);
        foreach(player player in FindObjectsOfType<player>())
        {
            if(player.bot == true && MathUtils.CompareDistances(transform.position, player.transform.position, 12, MathUtils.CompareTypes.LessThan))
            {
                var dir = (player.transform.position - transform.position);
                float wearoff = 6 - (dir.magnitude / 2);
                player.DecreaseHp(250, dir.normalized * 10 * wearoff, parent);
            }
        }
        Destroy(gameObject);
    }
}

public static class MathUtils
{
    public enum CompareTypes
    {
        GreaterThan,
        LessThan
    }
    public static bool CompareDistances(Vector2 v1, Vector2 v2, float distance, CompareTypes compareType)
    {
        switch(compareType)
        {
            case CompareTypes.GreaterThan:
            {
                return (v1 - v2).sqrMagnitude > distance * distance;
            }
            case CompareTypes.LessThan:
            {
                return (v1 - v2).sqrMagnitude < distance * distance;
            }
        }

        return false;
    }
}
