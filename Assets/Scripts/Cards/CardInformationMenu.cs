using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInformationMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup cardInformationMenu;

    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private List<CardInformationDisplay> cardsInformation;
    
    [SerializeField] private VoidChannelSO toggleCardInformationMenu;

    [SerializeField] private BuffSystem buffSystem;

    private bool isActive = false;

    private void OnEnable()
    {
        toggleCardInformationMenu.Subscribe(Toggle);
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
                cardsInformation[i].ShowCardDescription(buffSystem.GetShortDescription(inventory[i]));
            }
        }
    }

    public void Toggle() 
    {
        isActive = !isActive;

        cardInformationMenu.SetCanvas(isActive);

        if (isActive)
        {
            ShowCards();
        }
    }

    public void Toggle(bool value)
    {
        isActive = value;

        cardInformationMenu.SetCanvas(isActive);

        if (isActive)
        {
            ShowCards();
        }
    }

    private void OnDisable()
    {
        toggleCardInformationMenu.Unsubscribe(Toggle);
    }
}
