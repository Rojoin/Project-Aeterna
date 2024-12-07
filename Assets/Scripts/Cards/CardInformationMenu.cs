using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInformationMenu : MonoBehaviour
{
    [SerializeField] private GameObject cardInformationMenu;

    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private List<CardInformationDisplay> cardsInformation;


    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab)) 
        {
            ActiveCardInformationMenu(true);
        }
    }

    private void ShowCards() 
    {
        List<CardSO> inventory = new List<CardSO>();

        inventory = playerInventory.GetInventory();

        if (inventory.Count > 0) 
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                cardsInformation[i].ShowCardImage(inventory[i]);
                cardsInformation[i].ShowCardDescription(inventory[i]);
            }
        }
    }

    public void ActiveCardInformationMenu(bool value) 
    {
        cardInformationMenu.SetActive(value);

        if (value)
        {
            ShowCards();
        }
    }
}
