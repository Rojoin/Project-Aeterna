using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private List<GameObject> cardGO;

    private List<CardSO> invetory;

    void Start()
    {
        invetory = playerInventory.GetInventory();

        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowCardsHud() 
    {
        for (int i = 0; i < playerInventory.GetCurrentCards(); i++) 
        {
            cardGO[i].SetActive(true);
        }



        //invetory = playerInventory.GetInventory();

        //for (int i = slotIndex; i < playerInventory.GetCurrentCards();)
        //{
        //    for (int j = 0; j < invetory.Count; j++) 
        //    {
        //        if (cardSelected.ID != invetory[j].ID)
        //        {
        //            slotIndex += 1;
        //            break;
        //        }
        //    }
        //    break;
        //}

        //for (int i = 0; i < invetory.Count; i++)
        //{
        //    cardDisplay[i].ShowCard(invetory[i]);

        //    cardGO[i].SetActive(true);
        //}
    }

    public void DesactiveCardsGO() 
    {
        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }

        //slotIndex = 0;
    } 
}