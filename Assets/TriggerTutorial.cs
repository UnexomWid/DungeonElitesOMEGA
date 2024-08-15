using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    [SerializeField] bool buy = false;
    bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(triggered == false && collision.gameObject.GetComponent<player>() != null)
        {
            try
            {
                TutorialScene.instance.movement.GetComponentInChildren<Tutorial>().Disable();
            }
            catch
            {

            }
            try
            {
                TutorialScene.instance.inventory.GetComponentInChildren<Tutorial>().Disable();
             }
            catch
            {

            }
            try
            {
                TutorialScene.instance.megamap.GetComponentInChildren<Tutorial>().Disable();
            }
            catch
            {

            }
            if (buy)
            {
                TutorialScene.instance.EnableInteractIcon();
                TutorialScene.instance.interact.Begin();
            }
            else
            {
                TutorialScene.instance.EnableMegamapIcon();
                TutorialScene.instance.megamap.Begin();
            }
        }
    }
}
