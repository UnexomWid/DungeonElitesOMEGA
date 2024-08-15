using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeartrapVisibility : MonoBehaviour
{
    public SpriteRenderer sprite;

    int cnt = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<player>() != null)
        {
            cnt++;
            sprite.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<player>() != null)
        {
            cnt--;
            if(cnt==0)
                sprite.enabled = false;
        }
    }
}
