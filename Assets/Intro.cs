using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    public Sprite[] images;
    int index = 0;
    bool canPress = true;

    Animator animator;
    Image image;

    bool onPhone = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponentInChildren<Image>();
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            onPhone = true;
        //GetControllers();
    }

    //List<Joystick> controllers;

    /*void GetControllers()
    {
        if (gameObject.activeInHierarchy)
        {
            controllers = new List<Joystick>();

            var joystickGuid = System.Guid.Empty;
            var di = new DirectInput();

            IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
            for (int device = 0; device < gamepads.Count; device++)
            {
                joystickGuid = gamepads[device].InstanceGuid;
                controllers.Add(new Joystick(di, joystickGuid));
            }
            IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);
            for (int device = 0; device < joysticks.Count; device++)
            {
                joystickGuid = joysticks[device].InstanceGuid;
                controllers.Add(new Joystick(di, joystickGuid));
            }

            foreach (Joystick joystick in controllers)
            {
                joystick.Acquire();
            }
        }
        Invoke("GetControllers", 1f);
    }*/

    public void NextPic()
    {
        canPress = true;
        image.sprite = images[index++];
        if(index>=images.Length)
        {

            if (SceneManager.GetActiveScene().name == "Intro")
            {
                PlayerPrefs.SetInt("Intro", 0);
                if (PlayerPrefs.GetInt("Tutorial", 1) == 1)
                    SceneManager.LoadScene("TutorialScene");
                else SceneManager.LoadScene("Shops");
            }
            else
            {
                Destroy(FindObjectOfType<Music>().gameObject);
                SceneManager.LoadScene("Start");
            }
        }
    }

    bool hasFocus = true;

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
    }

    private void Update()
    {
        if(canPress&&(Input.GetKeyDown(KeyCode.Return)||(onPhone && Input.touchCount>0)))
        {
            canPress = false;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("next1"))
                animator.Play("next2");
            else animator.Play("next1");
        }

        if (onPhone)
            return;

        foreach (Gamepad controller in Gamepad.all)
        {
            if (hasFocus && controller.aButton.isPressed)
            {
                if (canPress == true)
                {
                    canPress = false;
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("next1"))
                        animator.Play("next2");
                    else animator.Play("next1");
                }
            }
        }
    }
}
