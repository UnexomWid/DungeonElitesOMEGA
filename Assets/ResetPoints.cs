using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPoints : MonoBehaviour
{
    [SerializeField] CharacterLoader characterLoader;
    [SerializeField] int cardId;
    public void ResetAllPoints()
    {
        characterLoader.ResetPoints(cardId);
    }
}
