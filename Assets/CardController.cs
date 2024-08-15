using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] GameObject[] row1;
    [SerializeField] GameObject[] row2;

    public player player;

    int xIndex;
    int yIndex;
    bool canDo = true;

    private void Start()
    {
        Select();

        canDo = true;
    }

    void Deselect()
    {
        GameObject obj = null;

        if (xIndex == 0)
            obj = row1[yIndex];
        else obj = row2[yIndex];

        obj.transform.parent.GetComponent<Image>().enabled = false;
    }

    void Select()
    {
        canDo = false;

        StartCoroutine(ActivateJoy());

        GameObject obj = null;

        if (xIndex == 0)
            obj = row1[yIndex];
        else obj = row2[yIndex];

        obj.transform.parent.GetComponent<Image>().enabled = true;
    }

    IEnumerator ActivateJoy()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        canDo = true;
    }

    bool canPress = true;

    private void Update()
    {
        if(player != null)
        {
            if (player.gamePad.leftStick.x > 0.5f && canDo)
            {
                Deselect();

                xIndex++;

                if (xIndex > 1)
                    xIndex = 1;

                if(yIndex >= row1.Length)
                    yIndex = row1.Length;

                Select();
            }
            else if (player.gamePad.leftStick.x < -0.5f && canDo)
            {
                Deselect();

                xIndex--;

                if (xIndex < 0)
                    xIndex = 0;

                Select();
            }
            else if (player.gamePad.leftStick.y < -0.5f && canDo)
            {
                Deselect();

                yIndex++;

                if (xIndex == 0 && yIndex >= row1.Length)
                    yIndex = row1.Length - 1;

                if (xIndex == 1 && yIndex >= row2.Length)
                    yIndex = row2.Length - 1;

                Select();
            }
            else if (player.gamePad.leftStick.y > 0.5f && canDo)
            {
                Deselect();

                yIndex--;

                if (yIndex < 0)
                    yIndex = 0;

                Select();
            }

            if (player.gamePad.A == 1)
            {
                if (canPress)
                {
                    canPress = false;

                    GameObject obj = null;

                    if (xIndex == 0)
                        obj = row1[yIndex];
                    else obj = row2[yIndex];

                    Button button = obj.GetComponent<Button>();

                    if(button != null)
                    {
                        button.onClick.Invoke();
                    }
                    else
                    {
                        AddPoint addPoint = obj.GetComponent<AddPoint>();

                        if(addPoint != null)
                        {
                            addPoint.OnPointerDown(null);
                        }
                        else
                        {
                            PurchasePoint purchasePoint = obj.GetComponent<PurchasePoint>();

                            if (purchasePoint != null)
                            {
                                purchasePoint.OnPointerDown(null);
                            }
                        }
                    }
                }
            }
            else canPress = true;
        }
    }
}
