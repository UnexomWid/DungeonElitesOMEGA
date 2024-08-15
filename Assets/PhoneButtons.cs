using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneButtons : MonoBehaviour
{
    [SerializeField] InventorySpawn inventorySpawn;

    public void Pause()
    {
        inventorySpawn.TriggerPause();
    }

    public void Megamap()
    {
        inventorySpawn.TriggerMegamap();
    }

    public void Inventory()
    {
        inventorySpawn.TriggerInventory();
    }
}
