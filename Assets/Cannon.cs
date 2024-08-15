using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject cannonBall;
    public GameObject shootPos;
    public AudioClip shoot;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        animator.Play("shoot");

        GameObject ball = Instantiate(cannonBall);
        ball.transform.position = shootPos.transform.position;
        ball.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 10);

        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));
    }
}
