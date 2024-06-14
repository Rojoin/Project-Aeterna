using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableCardManager : MonoBehaviour
{
    [SerializeField] private List<SelectableCardMovement> lookAtMouseList;

    [SerializeField] private GameSettings gameSettings;

    void Update()
    {
        if (!gameSettings.isUsingController) 
        {
            for (int i = 0; i < lookAtMouseList.Count; i++)
            {
                lookAtMouseList[i].SelectableCardMovementWithMouse();
            }
        }

        else 
        {
            for (int i = 0; i < lookAtMouseList.Count; i++)
            {
                lookAtMouseList[i].SelectableCardMovementWithGamepad();
            }
        }
    }
}
