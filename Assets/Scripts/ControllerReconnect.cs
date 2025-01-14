using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ControllerReconnect : MonoBehaviour
{
    /*public Joystick[] controllers;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        controllers = new Joystick[19];
        pendingControllers = new bool[19];
        pollControllers = new bool[19];
        Task.Run(TestControllers);
    }
    public bool[] pendingControllers;
    public bool[] pollControllers;
    public int currentNum = 0;
    public void TestControllers()
    {

            while (true)
            {

                for (int i = 0; i <= 18; i++)
                {
                    if (pendingControllers[i] == false && pollControllers[i])
                    {
                        pollControllers[i] = false;
                        controllers[i] = null;
                    }
                }

                bool canDo = true;
                for (int i = 0; i <= 4; i++)
                {
                    if (pendingControllers[i])
                    {
                        canDo = false;

                        bool ok = false;
                        try
                        {
                            CustomControls.GetButton(controllers[i], 0);
                            controllers[i].Poll();
                        }
                        catch
                        {
                            pollControllers[i] = true;
                            controllers[i] = null;
                            ok = true;
                        }
                        if (ok || pollControllers[i] == true)
                        {



                            List<SharpDX.DirectInput.Joystick> joystickz = new List<SharpDX.DirectInput.Joystick>();
                            var joystickGuid = Guid.Empty;
                            var di = new DirectInput();
                            IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);

                            if (!(i == 5 && keyboards.Count == 0))
                            {
                                IList <DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);

                                for (int device = 0; device < gamepads.Count; device++)
                                {
                                    try
                                    {
                                        joystickGuid = gamepads[device].InstanceGuid;
                                        joystickz.Add(new Joystick(di, joystickGuid));
                                    }
                                    catch
                                    {

                                    }
                                }
                                IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);

                                for (int device = 0; device < joysticks.Count; device++)
                                {
                                    try
                                    {
                                        joystickGuid = joysticks[device].InstanceGuid;
                                        joystickz.Add(new Joystick(di, joystickGuid));
                                    }
                                    catch
                                    {

                                    }
                                }
                                foreach (Joystick joy in joystickz)
                                {
                                    try
                                    {
                                        bool ok2 = true;
                                        for (int j = 0; j <= 18; j++)
                                        {
                                            try
                                            {
                                                if (controllers[j] != null)
                                                {

                                                    string value1 = joy.Properties.InterfacePath;
                                                    string value2 = controllers[j].Properties.InterfacePath;
                                                    if (value1 == value2)
                                                    {

                                                        ok2 = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        if (ok2)
                                        {
                                            for (int j = 0; j <= 18; j++)
                                            {
                                                if (pendingControllers[j])
                                                {
                                                    pollControllers[j] = false;
                                                    controllers[j] = joy;
                                                    controllers[j].Acquire();
                                                    pendingControllers[j] = false;
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    joystickGuid = keyboards[0].InstanceGuid;
                                    controllers[i] = new Joystick(di, joystickGuid);
                                    controllers[i].Acquire();
                                }
                                catch
                                {

                                }

                            }



                        }

                    }
                }
                if (canDo)
                {
                    for (int i = currentNum; i <= currentNum; i++)
                    {
                        if (controllers[i] != null || pollControllers[i])
                        {
                            bool ok = false;
                            try
                            {
                                CustomControls.GetButton(controllers[i], 0);
                                controllers[i].Poll();
                            }
                            catch
                            {
                                pollControllers[i] = true;
                                controllers[i] = null;
                                ok = true;
                            }
                            if (ok || pollControllers[i] == true)
                            {

                                List<SharpDX.DirectInput.Joystick> joystickz = new List<SharpDX.DirectInput.Joystick>();
                                var joystickGuid = Guid.Empty;
                                var di = new DirectInput();
                                IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);
                                if (!(i <= 2 && keyboards.Count == 0))
                                {
                                    IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
                                    for (int device = 0; device < gamepads.Count; device++)
                                    {
                                        try
                                        {
                                            joystickGuid = gamepads[device].InstanceGuid;
                                            joystickz.Add(new Joystick(di, joystickGuid));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);
                                    for (int device = 0; device < joysticks.Count; device++)
                                    {
                                        try
                                        {
                                            joystickGuid = joysticks[device].InstanceGuid;
                                            joystickz.Add(new Joystick(di, joystickGuid));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    foreach (Joystick joy in joystickz)
                                    {
                                        try
                                        {
                                            bool ok2 = true;
                                            for (int j = 0; j <= 18; j++)
                                            {
                                                try
                                                {
                                                    if (controllers[j] != null)
                                                    {

                                                        string value1 = joy.Properties.InterfacePath;
                                                        string value2 = controllers[j].Properties.InterfacePath;
                                                        if (value1 == value2)
                                                        {

                                                            ok2 = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            if (ok2)
                                            {
                                                pollControllers[i] = false;
                                                controllers[i] = joy;
                                                controllers[i].Acquire();
                                                currentNum++;
                                                break;
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        joystickGuid = keyboards[0].InstanceGuid;
                                        controllers[i] = new Joystick(di, joystickGuid);
                                        controllers[i].Acquire();
                                    }
                                    catch
                                    {

                                    }

                                }



                            }
                        }
                    }
                }

                /*else
                {
                Debug.Log("dungeon");
                    for (int i = 0; i <= 18; i++)
                    {
                        if (controllers[i] != null || pollControllers[i])
                        {
                            bool ok = false;
                            try
                            {
                            CustomControls.GetButton(controllers[i], 0);
                                controllers[i].Poll();
                            }
                            catch
                            {
                                pollControllers[i] = true;
                                controllers[i] = null;
                                ok = true;
                            }
                            if (ok || pollControllers[i] == true)
                            {
                                yield return new WaitForSecondsRealtime(0.2f);

                                List<SharpDX.DirectInput.Joystick> joystickz = new List<SharpDX.DirectInput.Joystick>();
                                var joystickGuid = Guid.Empty;
                                var di = new DirectInput();
                                IList<DeviceInstance> keyboards = di.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly);
                                yield return new WaitForSecondsRealtime(0.2f);
                                if (!(i <= 2 && keyboards.Count == 0))
                                {
                                    IList<DeviceInstance> gamepads = di.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
                                    yield return new WaitForSecondsRealtime(0.2f);
                                    for (int device = 0; device < gamepads.Count; device++)
                                    {
                                        try
                                        {
                                            joystickGuid = gamepads[device].InstanceGuid;
                                            joystickz.Add(new Joystick(di, joystickGuid));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    IList<DeviceInstance> joysticks = di.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly);
                                    yield return new WaitForSecondsRealtime(0.2f);
                                    for (int device = 0; device < joysticks.Count; device++)
                                    {
                                        try
                                        {
                                            joystickGuid = joysticks[device].InstanceGuid;
                                            joystickz.Add(new Joystick(di, joystickGuid));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    foreach (Joystick joy in joystickz)
                                    {
                                        try
                                        {
                                            bool ok2 = true;
                                            for (int j = 0; j <= 18; j++)
                                            {
                                                try
                                                {
                                                    if (controllers[j] != null)
                                                    {

                                                        string value1 = joy.Properties.InterfacePath;
                                                        string value2 = controllers[j].Properties.InterfacePath;
                                                        if (value1 == value2)
                                                        {

                                                            ok2 = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            if (ok2)
                                            {
                                                pollControllers[i] = false;
                                                controllers[i] = joy;
                                                controllers[i].Acquire();
                                                break;
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        joystickGuid = keyboards[0].InstanceGuid;
                                        controllers[i] = new Joystick(di, joystickGuid);
                                        controllers[i].Acquire();
                                    }
                                    catch
                                    {

                                    }

                                }



                            }
                        }
                    }
                }
            }

    
    }
    Update is called once per frame
    void Update()
    {
        
    }
    */
}
