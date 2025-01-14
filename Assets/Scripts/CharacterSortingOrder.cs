using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSortingOrder : MonoBehaviour
{
    public bool updateSort = true;
    public SpriteRenderer[] sprites;
    public int[] originalSortingOrders;
    Transform playerTransform;
    private void Start()
    {
        List<int> originalSortingOrders = new List<int>();
        playerTransform = transform;
        for(int i=0;i<sprites.Length;i++)
        {
            originalSortingOrders.Add(sprites[i].sortingOrder);
        }

        this.originalSortingOrders = originalSortingOrders.ToArray();
    }
    private void Update()
    {
        if (updateSort)
        {
            float yPosition = playerTransform.position.y;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].sortingOrder = -(int)(yPosition * 300 - originalSortingOrders[i]);
            }
        }
    }
}
