using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlider : MonoBehaviour
{
    public float speed;
    public GameObject dest;
    Vector2 start;

    Transform myTransform;

    DungeonData dungeonData;
    Collider2D collider;
    private void Start()
    {
        myTransform = transform;
        start = myTransform.position;
        collider = GetComponent<Collider2D>();

        dungeonData = FindObjectOfType<DungeonData>();
    }

    List<GameObject> players = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collObj = collision.gameObject;
        player player = collObj.GetComponent<player>();
        if (player != null && players.Contains(collObj) == false)
        {
            player.DecreaseHp(100 * dungeonData.currentMap, Vector2.zero, null);
            players.Add(collObj);
            StartCoroutine(RemovePlayer(collObj));
        }
     }

    IEnumerator RemovePlayer(GameObject player)
    {
        float t = 0;
        while (t < 1f)
        {
            t += PublicVariables.deltaTime;
            yield return null;
        }

        collider.enabled = false;
        collider.enabled = true;
        players.Remove(player);
    }

    bool movingForward = true;

    private void Update()
    {
       if(movingForward)
        {
            myTransform.position = Vector2.MoveTowards(myTransform.position, dest.transform.position, speed * PublicVariables.deltaTime);
            if (Vector2.Distance(myTransform.position, dest.transform.position) < 0.5f)
            {
                movingForward = false;
            }
        }
       else
        {
            myTransform.position = Vector2.MoveTowards(myTransform.position, start, speed * PublicVariables.deltaTime);
            if (Vector2.Distance(myTransform.position, start) < 0.5f)
            {
                movingForward = true;
            }
        }
    }
}
