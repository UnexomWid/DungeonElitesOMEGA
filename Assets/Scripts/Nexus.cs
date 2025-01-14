using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    public float health;
    public GameObject particles;
    public GameObject particleOrigin;
    public AudioSource damage;
    public AudioSource shutdown;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        health = 7000 * (1+FindObjectOfType<DungeonData>().playerCount*0.3f);
    }

    public void DecreaseHp(float damage)
    {
        if(health>0)
        {
            health -= damage;
            GameObject clone = Instantiate(particles);
            clone.transform.position = particleOrigin.transform.position;
            Destroy(clone, 1f);

            this.damage.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            this.damage.Play();

            if(health <= 0)
            {
                FindObjectOfType<SecondPhase>().AddNexus(this);
                this.shutdown.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
                this.shutdown.Play();
                animator.SetBool("shutdown", true);
            }
        }
    }

    public void Reactivate()
    {
        health = 7000 * (1 + DungeonData.instance.playerCount * 0.3f);
        animator.SetBool("shutdown", false);
    }
}
