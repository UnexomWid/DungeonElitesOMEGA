using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public bool inDungeon = true;
    public bool inShops = false;

    public static CameraFollow instance;

    public GameObject deathEffect;

    public SpriteRenderer damageEffect;
    byte imageAlpha = 0;

    public List<GameObject> players;

    public List<player> playerScripts;

    public float shakeMagnitude = 0.7f;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
        Begin();
        damageEffectTransform = damageEffect.transform;
    }

    Transform myTransform;
    Transform damageEffectTransform;

    public void Begin()
    {
        instance = this;

        playerScripts = new List<player>();
        foreach (GameObject player in players)
            playerScripts.Add(player.GetComponent<player>());

        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    bool shake = false;

    public void RecalculateDamage()
    {
        int totalPercentage = players.Count * 100;
        int currentPercentage = 0;

        for(int i=0;i<playerScripts.Count;i++)
        {
            if (playerScripts[i] != null)
                currentPercentage += (int)(playerScripts[i].health / playerScripts[i].maxHealth * 100);
        }

        if (currentPercentage < totalPercentage/2)
        {
            imageAlpha = (byte)(128 - (float)currentPercentage / (totalPercentage / 2) * 128);
            damageEffect.color = new Color32(255, 255, 255, imageAlpha);
        }
        else
        {
            imageAlpha = 0;
            damageEffect.color = new Color32(255, 255, 255, 0);
        }
    }

    float damageT = 0;
    bool showDamage = false;

    public void Damage(float intensity)
    {
        intensity *= 2;
        Shake(0.25f, intensity);
        RecalculateDamage();
        showDamage = true;
        damageT = 0;
        descend = false;
        t = 0;
    }
    
    public void Shake(float time, float intensity)
    {
        shake = true;
        shakeMagnitude = intensity;
        Invoke("StopShake", time);
    }

    void StopShake()
    {
        shake = false;
    }

    bool descend = true;
    float t = 1;


    // Update is called once per frame
    void Update()
    {
        if (inDungeon || inShops)
        {
            Vector3 pos = Vector3.zero;
            int minus = 0;
            for(int i=0;i<players.Count;i++)
            {
                if (players[i] != null)
                    pos += players[i].transform.position;
                else minus++;
            }

            if (players.Count - minus != 0)
            {
                pos /= players.Count - minus;
                myTransform.position = new Vector3(pos.x, pos.y, -10);
                if (shake)
                {
                    Vector3 rand = Random.insideUnitSphere * shakeMagnitude;
                    myTransform.position += new Vector3(rand.x, rand.y, 0);
                }
            }
        }
        if (inDungeon)
        {
            if (showDamage)
            {
                damageT += PublicVariables.deltaTime * 4;
                if (damageT >= 1)
                {
                    showDamage = false;
                    damageT = 1;
                }
                damageEffect.color = Color32.Lerp(new Color32(255, 255, 255, (byte)(imageAlpha + 100)), new Color32(255, 255, 255, imageAlpha), damageT);
            }

            if (descend)
            {
                t -= PublicVariables.deltaTime / 3;

                float value = Mathf.Lerp(1f, 1.1f, t);

                damageEffectTransform.localScale = new Vector2(value, value);
                if (t < 0)
                {
                    t = 0;
                    descend = false;
                }
            }
            else
            {
                t += PublicVariables.deltaTime / 3;

                float value = Mathf.Lerp(1f, 1.1f, t);

                damageEffectTransform.localScale = new Vector2(value, value);
                if (t > 1)
                {
                    t = 1;
                    descend = true;
                }
            }
        }
    }
}
