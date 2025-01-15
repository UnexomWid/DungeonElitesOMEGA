using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    //List<Joystick> controllers;

    // Start is called before the first frame update
    bool onPhone = false;
    void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        onPhone = true;
#endif

        //Task.Run(GetControllers);
    }

    /*void GetControllers()
    {
        while (true)
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
        }
        //Invoke("GetControllers", 1f);
    }*/

    bool hasFocus = true;
    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
    }

    bool canDo = true;

    public IEnumerator CanDo()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        canDo = true;
    }

    public int index = -1;
    public GameObject[] buttons;

    bool pressed = false;

    bool menuFocused = false;

    private void OnDisable()
    {
        if (index < 0 || index >= buttons.Length)
        {
            return;
        }

        buttons[index].GetComponent<PauseButtons>().Unhighlight();
        menuFocused = false;
    }
    private void LateUpdate()
    {
        if (onPhone == false)
        {
            bool ok = true;
            foreach (ControllerInput controller in FindObjectsOfType<ControllerInput>())
            {
                if (hasFocus && controller.leftStick.y < -0.5 && canDo)
                {
                    if (menuFocused == false)
                    {
                        menuFocused = true;

                        index = 0;
                        buttons[index].GetComponent<PauseButtons>().Highlight();
                    }
                    else
                    {
                        buttons[index].GetComponent<PauseButtons>().Unhighlight();
                        index++;
                        if (index > buttons.Length - 1)
                        {
                            index = buttons.Length - 1;
                        }
                        buttons[index].GetComponent<PauseButtons>().Highlight();
                    }

                    canDo = false;
                    StartCoroutine(CanDo());
                }
                else if (hasFocus && controller.leftStick.y > 0.5 && canDo)
                {
                    if (menuFocused == false)
                    {
                        menuFocused = true;

                        index = 0;
                        buttons[index].GetComponent<PauseButtons>().Highlight();
                    }
                    else
                    {
                        buttons[index].GetComponent<PauseButtons>().Unhighlight();

                        index--;
                        if (index < 0)
                        {
                            index = 0;
                        }
                        buttons[index].GetComponent<PauseButtons>().Highlight();
                    }
                    canDo = false;
                    StartCoroutine(CanDo());
                }
                if (hasFocus && controller.A == 1)
                {
                    ok = false;
                    if (pressed == false)
                    {
                        pressed = true;
                        if (menuFocused == false)
                        {
                            menuFocused = true;

                            index = 0;
                            buttons[index].GetComponent<PauseButtons>().Highlight();
                        }
                        else if (index != 2 && index != 3)
                        {
                            controller.A = 0;
                            buttons[index].GetComponent<PauseButtons>().Press();
                        }
                    }
                }
                if (index == 2 || index == 3)
                {
                    if (hasFocus && controller.leftStick.x > 0.5 && canDo)
                    {

                        buttons[index].GetComponent<PauseButtons>().MoreVolume();

                        canDo = false;
                        StartCoroutine(CanDo());
                    }
                    else if (hasFocus && controller.leftStick.x < -0.5 && canDo)
                    {
                        buttons[index].GetComponent<PauseButtons>().LessVolume();

                        canDo = false;
                        StartCoroutine(CanDo());
                    }
                }
            }
            if (ok)
            {
                pressed = false;
            }
        }
    }

    
}
