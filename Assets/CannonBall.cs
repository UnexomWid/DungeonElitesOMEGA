using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject vfx;
    public AudioClip expl;
    public AudioClip stun;

    Transform myTransform;
    private void Start()
    {
        myTransform = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FinalBossHitbox bossHitbox = collision.gameObject.GetComponent<FinalBossHitbox>();
        if (bossHitbox != null)
        {
            bossHitbox.Damage();
            GameObject particles = Instantiate(vfx);
            particles.transform.position = myTransform.position;
            Destroy(particles, 2f);

            AudioSource.PlayClipAtPoint(expl, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));

            AudioSource.PlayClipAtPoint(stun, Camera.main.transform.position, PlayerPrefs.GetFloat("SFXVolume", 1));

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float size = myTransform.localScale.x + 6 * PublicVariables.deltaTime;

        if (size > 32)
            myTransform.localScale = new Vector3(32, 32);
        else myTransform.localScale = new Vector3(size, size);

        myTransform.Rotate(0, 0, 90 * PublicVariables.deltaTime);
    }
}
