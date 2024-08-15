using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    public float A;
    public float X;
    public float Y;
    public float B;
    public Vector2 leftStick;
    public Vector2 rightStick;
    public float start;
    public float back;
    public float leftStickPress;
    public float rightStickPress;
    public float leftTrigger;
    public float rightTrigger;
    public float leftBumper;
    public float rightBumper;

    public int playerNumber;

    private void Start()
    {
        playerNumber = GetComponent<PlayerInput>().user.index + 1;
        DontDestroyOnLoad(gameObject);
    }

    public void OnA(InputAction.CallbackContext ctx) => A = ctx.ReadValue<float>();
    public void OnB(InputAction.CallbackContext ctx) => B = ctx.ReadValue<float>();
    public void OnX(InputAction.CallbackContext ctx) => X = ctx.ReadValue<float>();
    public void OnY(InputAction.CallbackContext ctx) => Y = ctx.ReadValue<float>();
    public void OnStart(InputAction.CallbackContext ctx) => start = ctx.ReadValue<float>();
    public void OnBack(InputAction.CallbackContext ctx) => back = ctx.ReadValue<float>();
    public void OnLeftStickPress(InputAction.CallbackContext ctx) => leftStickPress = ctx.ReadValue<float>();
    public void OnRightStickPress(InputAction.CallbackContext ctx) => rightStickPress = ctx.ReadValue<float>();
    public void OnLeftTrigger(InputAction.CallbackContext ctx) => leftTrigger = ctx.ReadValue<float>();
    public void OnRightTrigger(InputAction.CallbackContext ctx) => rightTrigger = ctx.ReadValue<float>();
    public void OnLeftBumper(InputAction.CallbackContext ctx) => leftBumper = ctx.ReadValue<float>();
    public void OnRightBumper(InputAction.CallbackContext ctx) => rightBumper = ctx.ReadValue<float>();
    public void OnLeftStick(InputAction.CallbackContext ctx) => leftStick = ctx.ReadValue<Vector2>();
    public void OnRightStick(InputAction.CallbackContext ctx) => rightStick = ctx.ReadValue<Vector2>();

}
