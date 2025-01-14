using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayControls : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public TextMeshProUGUI text6;
    public TextMeshProUGUI text7;

    //List<Joystick> controllers;

    bool mobile = false;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID||UNITY_IPHONE
        mobile = true;
#endif
        //Task.Run(GetControllers);
    }

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
    void Update()
    {
        bool ok = true;
        if (mobile == false)
        {
            foreach (ControllerInput controller in FindObjectsOfType<ControllerInput>())
            {
                if (hasFocus && controller.leftStick.y < -0.5 && canDo)
                {
                    if (index != 6)
                    {
                        if (index != -1)
                        {
                            if (index == 0)
                                text1.GetComponent<MouseText>().Unfocus();
                            if (index == 1)
                                text2.GetComponent<MouseText>().Unfocus();
                            if (index == 2)
                                text3.GetComponent<MouseText>().Unfocus();
                            if (index == 3)
                                text4.GetComponent<MouseText>().Unfocus();
                            if (index == 4)
                                text5.GetComponent<MouseText>().Unfocus();
                            if (index == 5)
                                text6.GetComponent<MouseText>().Unfocus();
                            if (index == 6)
                                text7.GetComponent<MouseText>().Unfocus();
                        }

                        index++;
                        if (index >= 6)
                        {
                            index = 6;
                        }


                        if (index == 0)
                            text1.GetComponent<MouseText>().Focus();
                        if (index == 1)
                            text2.GetComponent<MouseText>().Focus();
                        if (index == 2)
                            text3.GetComponent<MouseText>().Focus();
                        if (index == 3)
                            text4.GetComponent<MouseText>().Focus();
                        if (index == 4)
                            text5.GetComponent<MouseText>().Focus();
                        if (index == 5)
                            text6.GetComponent<MouseText>().Focus();
                        if (index == 6)
                            text7.GetComponent<MouseText>().Focus();


                        canDo = false;
                        Invoke("CanDo", 0.25f);
                    }
                }
                else if (hasFocus && controller.leftStick.y > 0.5 && canDo)
                {
                    if (index != 0)
                    {
                        if (index != -1)
                        {
                            if (index == 0)
                                text1.GetComponent<MouseText>().Unfocus();
                            if (index == 1)
                                text2.GetComponent<MouseText>().Unfocus();
                            if (index == 2)
                                text3.GetComponent<MouseText>().Unfocus();
                            if (index == 3)
                                text4.GetComponent<MouseText>().Unfocus();
                            if (index == 4)
                                text5.GetComponent<MouseText>().Unfocus();
                            if (index == 5)
                                text6.GetComponent<MouseText>().Unfocus();
                            if (index == 6)
                                text7.GetComponent<MouseText>().Unfocus();
                        }

                        index--;
                        if (index < 0)
                        {
                            index = 0;
                        }



                        if (index == 0)
                            text1.GetComponent<MouseText>().Focus();
                        if (index == 1)
                            text2.GetComponent<MouseText>().Focus();
                        if (index == 2)
                            text3.GetComponent<MouseText>().Focus();
                        if (index == 3)
                            text4.GetComponent<MouseText>().Focus();
                        if (index == 4)
                            text5.GetComponent<MouseText>().Focus();
                        if (index == 5)
                            text6.GetComponent<MouseText>().Focus();
                        if (index == 6)
                            text7.GetComponent<MouseText>().Focus();


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

                            text1.GetComponent<MouseText>().Focus();
                        }
                        else
                        {


                            if (index == 0)
                                text1.GetComponent<MouseText>().Activate();
                            if (index == 1)
                                text2.GetComponent<MouseText>().Activate();
                            if (index == 2)
                                text3.GetComponent<MouseText>().Activate();
                            if (index == 3)
                                text4.GetComponent<MouseText>().Activate();
                            if (index == 4)
                                text5.GetComponent<MouseText>().Activate();
                            if (index == 5)
                                text6.GetComponent<MouseText>().Activate();
                            if (index == 6)
                                text7.GetComponent<MouseText>().Activate();
                        }
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
