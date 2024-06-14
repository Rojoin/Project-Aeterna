using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableCardManager : MonoBehaviour
{
    [SerializeField] private List<LookAtMouse> lookAtMouseList;

    public bool isOnPC = true;

    void Update()
    {
        if (isOnPC) 
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
