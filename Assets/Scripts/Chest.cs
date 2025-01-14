using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    public bool parkourChest;


    public GameObject coinVfx;
    public GameObject destroyVfx;
    public AudioSource chestLoud;
    public AudioSource chestSoft;
    public AudioSource chestDestroy;
    public AudioSource chestDestroy2;

    InventorySpawn inventorySpawn;
    Animator animator;
    private void Start()
    {
        LoudSound();
        Invoke("EnableAnimator", 0.1f);
        Invoke("SoftSound", 0.585f);
        inventorySpawn = FindObjectOfType<InventorySpawn>();
        animator = GetComponent<Animator>();
    }

    void EnableAnimator()
    {
        animator.enabled = true;
    }

    void LoudSound()
    {
        chestLoud.Play();
        chestLoud.volume = PlayerPrefs.GetFloat("SFXVolume", 1)/1.1f;
    }

    void SoftSound()
    {
        chestSoft.Play();
        chestSoft.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
    }
    public void DestroyChest(bool collision = false)
    {
        Vector2 myPosition = transform.position;

        int value = Random.Range(12, 17) * 10;
        if (collision)
            value = Random.Range(30, 40) * 10;
        inventorySpawn.AddCoins(value);
        GameObject vfx = Instantiate(coinVfx);
        vfx.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        vfx.GetComponentInChildren<TextMeshPro>().text = "+" + value.ToString();
        vfx.transform.position = new Vector2(myPosition.x + 6f, myPosition.y);
        vfx.transform.parent = Camera.main.transform;

        chestDestroy.Play();
        chestDestroy.volume = PlayerPrefs.GetFloat("SFXVolume", 1);

        chestDestroy2.Play();
        chestDestroy2.volume = PlayerPrefs.GetFloat("SFXVolume", 1);

        GameObject vfxClone = Instantiate(destroyVfx);
        vfxClone.transform.position = myPosition;
        vfxClone.SetActive(true);

        DungeonData.instance.AddToQuest(2, 1);

        Destroy(vfxClone, 2f);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (parkourChest && player != null)
        {
            if(player.bot == false)
                DestroyChest(true);
        }
    }
}
