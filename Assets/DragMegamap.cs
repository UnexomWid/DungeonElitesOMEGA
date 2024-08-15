using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMegamap : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == 0)
            myTransform.position += (Vector3)eventData.delta;
    }

    InventorySpawn inventorySpawn;
    Transform myTransform;
    bool onPhone;
    CameraFollow cameraFollow;
    void Awake()
    {
        inventorySpawn = FindObjectOfType<InventorySpawn>();
        myTransform = transform;
        onPhone = Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }
    void OnEnable()
    {
        inventorySpawn.megamapCamera.GetComponent<Camera>().Render();
        inventorySpawn.megamapCamera.SetActive(false);
    }

    void Zoom(float amount)
    {
        if (amount > 0 && myTransform.localScale.x != 8)
        {
            Vector2 mousePosition = PublicVariables.mousePosition;

            GameObject mousePos = new GameObject();
            mousePos.transform.parent = myTransform;
            mousePos.transform.position = PublicVariables.mousePosition;

            myTransform.localScale = new Vector2(myTransform.localScale.x + amount * 4, myTransform.localScale.y + amount * 4);

            myTransform.position = myTransform.position - (mousePos.transform.position - (Vector3)mousePosition);

            Destroy(mousePos);
        }
        else if (amount < 0 && myTransform.localScale.x != 1)
        {
            Vector2 mousePosition = PublicVariables.mousePosition;

            GameObject mousePos = new GameObject();
            mousePos.transform.parent = myTransform;
            mousePos.transform.position = PublicVariables.mousePosition;

            myTransform.localScale = new Vector2(myTransform.localScale.x + amount * 4, myTransform.localScale.y + amount * 4);

            myTransform.position = myTransform.position - (mousePos.transform.position - (Vector3)mousePosition);

            Destroy(mousePos);
        }
    }


    PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerData = FindObjectOfType<PlayerData>();
        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        myTransform.localScale = new Vector2(2, 2);
    }
    bool pressed1 = false;
    bool pressed2 = false;
    // Update is called once per frame
    void Update()
    {
        if (onPhone)
        { 
            bool ok = false;
            bool ok2 = false;
            foreach (player player in cameraFollow.playerScripts)
            {

                try
                {
                    if (player != null)
                    {

                        if (player.bot == false)
                        {

                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightBumper == 1 && player.playerNumber != 5) : false)))
                            {
                                ok = true;
                                if (pressed1 == false)
                                {
                                    pressed1 = true;
                                    myTransform.localScale += new Vector3(1f, 1f);
                                }
                            }
                            else if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.leftBumper == 1 && player.playerNumber != 5) : false)))
                            {
                                ok2 = true;
                                if (pressed2 == false)
                                {
                                    pressed2 = true;
                                    myTransform.localScale -= new Vector3(1f, 1f);
                                }
                            }
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightStick.x > 0.5 && player.playerNumber != 5) : false)))
                            {
                                myTransform.position -= new Vector3(600, 0) * Time.unscaledDeltaTime;

                            }
                            else if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightStick.x < -0.5 && player.playerNumber != 5) : false)))
                            {

                                myTransform.position += new Vector3(600, 0) * Time.unscaledDeltaTime;

                            }
                            if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightStick.y < -0.5 && player.playerNumber != 5) : false)))
                            {
                                myTransform.position += new Vector3(0, 600) * Time.unscaledDeltaTime;

                            }
                            else if (playerData.hasFocus && ((player.gamePad != null ? (player.gamePad.rightStick.y > 0.5 && player.playerNumber != 5) : false)))
                            {
                                myTransform.position -= new Vector3(0, 600) * Time.unscaledDeltaTime;

                            }
                        }
                    }
                }
                catch
                {

                }
            }
            if (ok == false)
            {
                pressed1 = false;
            }
            if (ok2 == false)
            {
                pressed2 = false;
            }
            Zoom(Input.GetAxis("Mouse ScrollWheel"));

            if (myTransform.localScale.x < 1)
                myTransform.localScale = new Vector2(1, 1);
            if (myTransform.localScale.x > 8)
                myTransform.localScale = new Vector2(8, 8);


            float minX = -Screen.width / 2 * (myTransform.localScale.x - 1);
            float maxX = Screen.width / 2 * (myTransform.localScale.x - 1);
            float minY = -Screen.height / 2 * (myTransform.localScale.y - 1);
            float maxY = Screen.height / 2 * (myTransform.localScale.y - 1);

            if (myTransform.localPosition.x < minX)
            {
                myTransform.localPosition = new Vector2(minX, myTransform.localPosition.y);
            }
            if (myTransform.localPosition.x > maxX)
            {
                myTransform.localPosition = new Vector2(maxX, myTransform.localPosition.y);
            }
            if (myTransform.localPosition.y < minY)
            {
                myTransform.localPosition = new Vector2(myTransform.localPosition.x, minY);
            }
            if (myTransform.localPosition.y > maxY)
            {
                myTransform.localPosition = new Vector2(myTransform.localPosition.x, maxY);
            }
        }
        else if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the canvas size based on the change in distance between the touches.
            Vector2 scale = myTransform.localScale - new Vector3(deltaMagnitudeDiff * 0.01f, deltaMagnitudeDiff * 0.01f);

            if (scale.x < 1)
                scale = new Vector2(1, 1);
            if (scale.x > 8)
                scale = new Vector2(8, 8);


            float minX = -Screen.width / 2 * (scale.x - 1);
            float maxX = Screen.width / 2 * (scale.x - 1);
            float minY = -Screen.height / 2 * (scale.y - 1);
            float maxY = Screen.height / 2 * (scale.y - 1);

            myTransform.localScale = scale;

            Vector2 position = myTransform.localPosition;

            if (position.x < minX)
            {
                position = new Vector2(minX, position.y);
            }
            if (position.x > maxX)
            {
                position = new Vector2(maxX, position.y);
            }
            if (position.y < minY)
            {
                position = new Vector2(position.x, minY);
            }
            if (position.y > maxY)
            {
                position = new Vector2(position.x, maxY);
            }

            myTransform.localPosition = position;
        }
    }
}
