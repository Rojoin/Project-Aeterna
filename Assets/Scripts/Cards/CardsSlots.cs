using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardsSlots : MonoBehaviour
{
    [SerializeField] private int currentCardsInSlot;

    [SerializeField] private int maxCardsInSlot;

    [SerializeField] private TextMeshProUGUI stacksTexts;

    private void Start()
    {
        currentCardsInSlot = 0;
        stacksTexts.text = "";  
    }

    public void ShowCardsStacksUI()
    {
        if (currentCardsInSlot > 1) 
        {
            stacksTexts.text = "x" + currentCardsInSlot.ToString();
        }

        if (currentCardsInSlot >= maxCardsInSlot)
        {
            currentCardsInSlot = maxCardsInSlot;
        }
    }

    public void SetCurrentCardsInSlot(int value) 
    {
        currentCardsInSlot += value;
    }

    public int GetCurrentCardsInSlot() 
    {
        return currentCardsInSlot;
    }
}