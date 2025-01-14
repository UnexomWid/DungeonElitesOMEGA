using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
    [SerializeField] InventorySpawn inventorySpawn;

    private void Awake()
    {
    if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        gameObject.SetActive(false);

    }

    public void Return()
    {
        inventorySpawn.CloseAll();
    }
}
