using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomControls
{
    /*public static bool GetButton(SharpDX.DirectInput.Joystick joystick, int buttonNumber)
    {
        if (joystick != null)
        {
            JoystickState state = new JoystickState();

            state = joystick.GetCurrentState();

            bool[] buttons = state.Buttons;
            return (buttons[buttonNumber] == true);
        }
        else return false;
        

    }
    public static Axe GetAxis(SharpDX.DirectInput.Joystick joystick)
    {
        if (joystick != null)
        {
            JoystickState state = new JoystickState();
            
            state = joystick.GetCurrentState();

            Axe axis = new Axe();
            axis.Xaxis = state.X;
            axis.Yaxis = state.Y;
            axis.Zaxis = state.Z;

            return axis;
        }
        else return null;
    }

    public static Axe GetSecondAxis(SharpDX.DirectInput.Joystick joystick)
    {
        if (joystick != null)
        {
            JoystickState state = new JoystickState();

            state = joystick.GetCurrentState();

            Axe axis = new Axe();
            axis.Xaxis = state.RotationX;
            axis.Yaxis = state.RotationY;
            axis.Zaxis = state.RotationZ;

            return axis;
        }
        else return null;
    }*/
}
public class Axe
{
    public float Xaxis;
    public float Yaxis;
    public float Zaxis;
}
