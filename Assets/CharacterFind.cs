using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFind : MonoBehaviour
{
    public GameObject[] characters;
    private void Start()
    {
        if (FindObjectsOfType<CharacterFind>().Length >= 2)
            Destroy(gameObject);
        else
        {
            characters = new GameObject[5] { Resources.Load("Prefabs/wizard") as GameObject, Resources.Load("Prefabs/knight") as GameObject, Resources.Load("Prefabs/archer") as GameObject, Resources.Load("Prefabs/tank") as GameObject, Resources.Load("Prefabs/support") as GameObject };
            DontDestroyOnLoad(gameObject);
        }
    }
}
