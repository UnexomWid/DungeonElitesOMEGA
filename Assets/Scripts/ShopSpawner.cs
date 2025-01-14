using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopSpawner : MonoBehaviour
{
    public GameObject[] mobs;

    public GameObject[] spawnLocations;

    player[] spawnedMobs = new player[0];

    List<player> enteredPlayers = new List<player>();


    bool isInTutorial = false;
    private void Start()
    {
        isInTutorial = SceneManager.GetActiveScene().name == "TutorialScene";
    }

    public GameObject GetHideSpot(GameObject target, GameObject except = null)
    {
        float maxDist = 0f;
        GameObject hideSpot = null;

        List<GameObject> hidingList = new List<GameObject>(spawnLocations);
        if (except != null)
            hidingList.Remove(except);

        return hidingList[Random.Range(1, 999999) % hidingList.Count];
    }

    public GameObject GetHideSpotClosest(Vector2 mobPos, Vector2 playerPos)
    {
        GameObject currHideSpot = null;

        float maxAngle = 0f;
        for(int i=0;i<spawnLocations.Length;i++)
        {
            GameObject hideSpot = spawnLocations[i];
            float angle = Angle360(playerPos, mobPos, hideSpot.transform.position);

            if (angle < 45 && angle > maxAngle)
            {
                currHideSpot = hideSpot;
                maxAngle = angle;
            }
        }

        return currHideSpot;
    }

    public static float Angle360(Vector2 playerPos, Vector2 mobPos, Vector2 hidePos)
    {
        return Vector2.Angle(playerPos - mobPos, hidePos - mobPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            player player = collision.gameObject.GetComponent<player>();
            if (player.bot == false && enteredPlayers.Contains(player) == false)
                enteredPlayers.Add(player);
        }
        catch
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        try
        {
            if (collision.enabled)
            {
                enteredPlayers.Remove(collision.gameObject.GetComponent<player>());
                if (enteredPlayers.Count == 0)
                {;
                    t = 0;
                    int count = 0;
                    foreach (player mob in spawnedMobs)
                    {
                        if (mob != null)
                        {
                            count++;
                        }
                    }

                    foreach (player mob in spawnedMobs)
                    {
                        if (mob != null)
                        {
                            try
                            {
                                mob.DecreaseHp(99999, Vector2.zero, null, count);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }
        catch
        {

        }
    }

    float t = 0;
    private void Update()
    {
        if(enteredPlayers.Count > 0)
        {
            t += PublicVariables.deltaTime;
            if(t>=1 && spawnedMobs.Length == 0)
            {
                List<player> spawnedMobs = new List<player>();
                for (int i = 0; i < spawnLocations.Length; i++)
                {
                    GameObject enemy = Instantiate(mobs[Random.Range(1, 99999) % mobs.Length]);
                    enemy.transform.position = spawnLocations[i].transform.position;
                    enemy.transform.localScale *= 1.5f;
                    enemy.gameObject.SetActive(true);
                    player mobScript = enemy.GetComponent<player>();
                    mobScript.currentBox = GetComponent<Collider2D>();
                    mobScript.Stats.SetActive(true);
                    spawnedMobs.Add(mobScript);
                }

                this.spawnedMobs = spawnedMobs.ToArray();
            }
            else if(t>=1 && isInTutorial == false)
            {
                bool ok = false;
                for(int i=0;i<spawnedMobs.Length;i++)
                {
                    if (spawnedMobs[i] != null)
                        ok = true;
                }
                /*foreach(player mob in spawnedMobs)
                {
                    if (mob != null)
                        ok = true;
                }*/

                if(ok == false)
                {
                    spawnedMobs = new player[0];
                    t = 0;
                }
            }
        }
    }
}
