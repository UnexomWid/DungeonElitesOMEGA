using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;

public class MainMenuControllers : MonoBehaviour
{
    //List<Joystick> controllers = new List<Joystick>();

    // Start is called before the first frame update
    void Start()
    {
        Input.multiTouchEnabled = true;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            onPhone = true;

        if(onPhone)
        {
            if(PlayerPrefs.GetInt("ToggleMovement", 69420) == 69420)
            {
                PlayerPrefs.SetInt("ToggleMovement", 1);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("ToggleMovement", 69420) == 69420)
            {
                PlayerPrefs.SetInt("ToggleMovement", 0);
            }
        }

        if (PlayerPrefs.GetInt("Difficulty", 69420) == 69420)
        {
            PlayerPrefs.SetInt("Difficulty", 1);
        }

        //Task.Run(GetControllers);
    }

    bool onPhone = false;

    /*void GetControllers()
    {
        while (true)
        {
            try
            {
                Debug.Log(1);
                List<Joystick> newList = new List<Joystick>();

                var joystickGuid = System.Guid.Empty;
                var di = new DirectInput();
                Debug.Log(2);
                IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
                for (int device = 0; device < gamepads.Count; device++)
                {
                    joystickGuid = gamepads[device].InstanceGuid;
                    newList.Add(new Joystick(di, joystickGuid));
                }
                Debug.Log(3);
                IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);
                for (int device = 0; device < joysticks.Count; device++)
                {
                    joystickGuid = joysticks[device].InstanceGuid;
                    newList.Add(new Joystick(di, joystickGuid));
                }
                Debug.Log(4);
                foreach (Joystick joystick in newList)
                {
                    joystick.Acquire();
                }
                List<string> codes = new List<string>();
                List<string> newCodes = new List<string>();
                Debug.Log(5);
                foreach (Joystick joy in controllers)
                {
                    codes.Add(joy.Properties.InterfacePath);
                }
                foreach (Joystick joy in newList)
                {
                    newCodes.Add(joy.Properties.InterfacePath);
                }
                Debug.Log(6);
                bool ok = true;
                foreach (string code in codes)
                {
                    if (newCodes.Contains(code) == false)
                        ok = false;
                }
                foreach (string code in newCodes)
                {
                    if (codes.Contains(code) == false)
                        ok = false;
                }
                if (ok == false)
                {
                    controllers = newList;
                    Debug.Log("assigned");
                }
            }
            catch
            {

            }
            
        }
    }*/

    bool hasFocus = true;
    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
    }

    bool canDo = true;

    void CanDo()
    {
        canDo = true;
    }

    int index = -1;
    public MouseText[] buttons;

    bool pressed = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (onPhone == false)
        {
            bool ok = true;
            int count = 0;
            GameInput gameInput = new GameInput();

            foreach (ControllerInput controller in FindObjectsOfType<ControllerInput>())
            {
                try
                {
                    if (hasFocus && controller.leftStick.y < -0.5 && canDo)
                    {
                        if (index != 3)
                        {
                            if (index != -1)
                                buttons[index].Unfocus();

                            index++;
                            if (index >= buttons.Length)
                            {
                                index = buttons.Length - 1;
                            }

                            buttons[index].Focus();

                            canDo = false;
                            Invoke("CanDo", 0.25f);
                        }
                    }
                    else if (hasFocus && controller.leftStick.y > 0.5 && canDo)
                    {
                        if (index != 0)
                        {
                            if (index != -1)
                                buttons[index].Unfocus();

                            index--;
                            if (index < 0)
                            {
                                index = 0;
                            }

                            buttons[index].Focus();

                            canDo = false;
                            Invoke("CanDo", 0.25f);
                        }
                    }
                    if (hasFocus && controller.A == 1)
                    {
                        ok = false;
                        if (pressed == false)
                        {
                            pressed = true;
                            if (index == -1)
                            {
                                index = 0;

                                buttons[index].Focus();
                            }
                            else
                            {
                                buttons[index].Activate();

                                Destroy(gameObject);
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            if (ok)
            {
                pressed = false;
            }
        }
    }
}
