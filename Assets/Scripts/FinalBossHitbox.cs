using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossHitbox : MonoBehaviour
{
    public AudioSource breaking;
    public AudioSource rise;

    void FinalBossCredits()
    {
        if (SceneManager.GetActiveScene().name.Contains("Shops") == false)
            SceneManager.LoadScene("Credits");
    }

    public void PlayBreak()
    {
        breaking.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        breaking.Play();
    }

    public void Rise()
    {
        rise.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        rise.Play();
    }

    public GameObject bigExpl;
    public GameObject smallExpl;

    public AudioClip smallSfx;
    public AudioClip bigSfx;

    public GameObject minPos;
    public GameObject maxPos;

    public Music music;

    public ThirdPhase thirdPhase;

    int damage = 0;

    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Damage()
    {
        damage++;
        if(damage == OMEGA.Data.GetOMEGAHitTolerance())
        {
            music.gameObject.SetActive(false);

            thirdPhase.CancelInvoke("Fireball");

            foreach(hitbox hitbox in FindObjectsOfType<hitbox>())
            {
                if (hitbox.fire)
                    Destroy(hitbox);
            }

            animator.speed = 0;

            Expl();
        }
        else
        {
            animator.Play("damage");
        }
    }

    void Expl()
    {
        SmallExpl();
        BigExpl();
    }

    void SmallExpl()
    {
        if (index != 5)
        {
            GameObject explosion = Instantiate(smallExpl);
            explosion.transform.position = new Vector2(Random.Range(maxPos.transform.position.x, minPos.transform.position.x), Random.Range(minPos.transform.position.y, maxPos.transform.position.y));

            AudioSource.PlayClipAtPoint(smallSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1f));

            Destroy(explosion, 2f);
            Invoke("SmallExpl", 0.33f);
        }
    }

    int index = 0;

    void BigExpl()
    {
        index++;
        GameObject explosion = Instantiate(bigExpl);
        explosion.transform.position = new Vector2(Random.Range(maxPos.transform.position.x, minPos.transform.position.x), Random.Range(minPos.transform.position.y, maxPos.transform.position.y));

        AudioSource.PlayClipAtPoint(bigSfx, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1f));

        Destroy(explosion, 2f);
        if (index == 5)
        {
            animator.speed = 1;
            animator.Play("death_Boss");
        }
        else Invoke("BigExpl", 1f);
    }
}
