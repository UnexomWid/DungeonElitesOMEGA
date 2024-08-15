using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PhoneController : MonoBehaviour
{
    [SerializeField] PhoneAbillities down;
    [SerializeField] PhoneAbillities right;
    [SerializeField] PhoneAbillities up;
    [SerializeField] PhoneAbillities left;
    [SerializeField] bool abillities = false;

    [SerializeField] RectTransform parent;

    [SerializeField] string horizontal;
    [SerializeField] string vertical;

    Transform myTransform;

    bool transformNull = true;
    bool active = false;

    private void Start()
    {
        myTransform = transform;
        transformNull = false;
        active = true;
    }

    public Vector2 dir
    {
        get
        {
            /*if (transformNull)
                return Vector2.zero;
            return myTransform.localPosition / 36;*/

            if (active == false)
                return Vector2.zero;

            return new Vector2(CrossPlatformInputManager.GetAxis(horizontal), CrossPlatformInputManager.GetAxis(vertical));
        }
    }

    bool triggered = false;
    public void TriggerClick()
    {
        triggered = true;
    }

    public void StopTrigger()
    {
        try
        {
            if (Input.GetTouch(id).phase == TouchPhase.Ended)
                triggered = false;
        }
        catch
        {
            triggered = false;
        }
    }

    int id;
    private void Update()
    {
        /*int touchCount = Input.touchCount;

        if (touchCount > 0 && triggered)
        {
            Touch touch = new Touch();

            touch.phase = TouchPhase.Ended;

            if(id != -1)
            {
                touch = Input.GetTouch(id);
            }

            if (touch.phase != TouchPhase.Ended)
            {
                myTransform.position = Input.GetTouch(id).position;
            }
            else
            {
                float closest = 99999;

                Vector2 position = myTransform.parent.position;

                Vector2 parentPosition = position;

                Touch[] touches = Input.touches;

                for (int i = 0; i < touchCount; i++)
                {
                    float dist = Vector2.Distance(parentPosition, touches[i].position);

                    if (dist < closest)
                    {
                        closest = dist;
                        id = touches[i].fingerId;
                        position = touches[i].position;
                    }
                }

                /*float dist = Vector3.Distance(Input.GetTouch(id).position, parent.myTransform.position);

                if (dist < 360)
                {
                    myTransform.position = Input.GetTouch(id).position;
                }
                else myTransform.localPosition = Vector3.zero;*/

                /*myTransform.position = position;
            }
        }
        else
        {
            myTransform.localPosition = Vector3.zero;
            triggered = false;
            id = -1;
        }

        float distance = myTransform.localPosition.magnitude;

        if (distance > 36) 
        {
            Vector3 fromOriginToObject = myTransform.localPosition - Vector3.zero; 
            fromOriginToObject *= 36 / distance;
            myTransform.localPosition = fromOriginToObject;
        }*/

        if(abillities)
        {
            right.attack = false;
            up.attack = false;
            down.attack = false;
            left.attack = false;

            if (dir.normalized.x > 0.5)
                right.attack = true;
            if (dir.normalized.x < -0.5)
                left.attack = true;
            if (dir.normalized.y > 0.5)
                up.attack = true;
            if (dir.normalized.y < -0.5)
                down.attack = true;
        }
    }
}
