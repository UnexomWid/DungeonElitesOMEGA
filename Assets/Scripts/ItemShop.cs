using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShop : MonoBehaviour
{
    public int maxNum;
    public List<int> numbers;
    public GameObject[] itemMenus;
    void Start()
    {
        numbers = new List<int>();
        foreach(GameObject menu in itemMenus)
        {
            int num = -1;
            do
            {
                num = Random.Range(1, 999999) % (maxNum+1);
            }
            while (numbers.Contains(num));
            numbers.Add(num);
            menu.GetComponent<itemMenu>().SetUp((OMEGA.Items.ID)num);
        }
    }
}
