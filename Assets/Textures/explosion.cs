using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    public GameObject vfx;
    public bool active;
    public AudioSource explosionSfx;
    void Start()
    {
        Invoke("Explode", 2f);
    }
    void ExplosionSfx()
    {

    }
    void Explode()
    {
        if(active)
        {
            foreach(player tudor in FindObjectsOfType<player>())
            {
                if (tudor.bot == false && Vector2.Distance(tudor.transform.position, transform.position) < 2f && tudor.finalBoss == false)
                {
                    tudor.GetComponent<player>().DecreaseHp(100, new Vector2(0,0), null);
                }
            }
            GameObject clone = Instantiate(vfx);
            clone.transform.position = transform.position;
            Destroy(clone, 1f);
            explosionSfx.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
            explosionSfx.Play();
            Destroy(gameObject);

        }
    }
}
