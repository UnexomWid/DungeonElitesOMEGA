using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPhase : MonoBehaviour
{
    public GameObject boss;
    public GameObject cannon;
    public GameObject fireball;
    public GameObject[] fireballPositions;
    public GameObject charge;

    public GameObject explosion;

    public GameObject maxPos;
    public GameObject minPos;

    public AudioSource fireballSFX;

    CameraFollow cameraFollow;

    float t;
    int damageIndex;

    private void Start()
    {
        cannon.SetActive(true);

        cameraFollow = FindObjectOfType<CameraFollow>();

        Invoke("Fireball", 0.3f);
    }

    void Fireball()
    {
        for (int i = 1; i <= 1; i++)
        {
            GameObject fb = Instantiate(fireball);
            fb.transform.position = fireballPositions[Random.Range(1, 99999) % fireballPositions.Length].transform.position;

            Vector3 target = new Vector2(Random.Range(maxPos.transform.position.x, minPos.transform.position.x), Random.Range(minPos.transform.position.y, maxPos.transform.position.y));

            Vector2 diff = target - fb.transform.position;
            diff.Normalize();

            fb.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90);

            fb.GetComponent<Rigidbody2D>().velocity = -fb.transform.up * 15;
            hitbox fbHitbox = fb.GetComponent<hitbox>();
            fbHitbox.damage = 200;
            fbHitbox.knockBackSpeed = 5;

            fb.transform.localScale *= 2f;

        }

        fireballSFX.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
        fireballSFX.Play();

        Invoke("Fireball", 0.3f);
    }

    void Target()
    {
        GameObject expl = Instantiate(explosion);
        expl.GetComponent<explosion>().active = true;

        GameObject target = null;

        List<GameObject> targets = new List<GameObject>();

        foreach (player player in cameraFollow.playerScripts)
        {
            if (player != null)
            {
                if (player.gameObject != gameObject)
                {
                    targets.Add(player.gameObject);
                }
            }
        }

        target = targets[UnityEngine.Random.Range(1, 99999) % targets.Count];

        expl.transform.position = target.transform.position;

        Invoke("Target", 1f);
    }

    private void Update()
    {
        if(t<30f)
        {
            t += PublicVariables.deltaTime;

            charge.transform.localScale = new Vector3(1, t / 30f);

            if(t>=30f)
            {
                t = 0f;

                cannon.GetComponent<Cannon>().Shoot();
            }
        }
    }
}
