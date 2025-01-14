using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDungeonSelect : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    public void Enter()
    {
        playerData.Enter();
    }

    public void Return()
    {
        playerData.Return();
    }

    public void Left()
    {
        playerData.Left();
    }

    public void Right()
    {
        playerData.Right();
    }
}
