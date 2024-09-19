using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private int maxCards;

    [SerializeField] private int maxCardsOnSlot;

    [SerializeField] private int currentCards;

    [SerializeField] private int maxCardOnInventory;

    [SerializeField] private List<CardSO> playerCardsInventory;

    [SerializeField] private List<CardSO> allCards;

    public List<CardSO> GetInventory() 
    {
        return playerCardsInventory;
    }

    public void ClearInventory() 
    {
        playerCardsInventory.Clear();
    }

    public int GetMaxCards() 
    {
        return maxCards;
    }

    public void SetMaxCards(int num) 
    {
        maxCards = num;
    }

    public int GetCurrentCards() 
    {
        return currentCards;
    }

    public void SetCurrentCards(int num) 
    {
        currentCards = num;
    }

    public void SetCardsOnSlot(CardSO newCard) 
    {
        for (int i = 0; i < currentCards; i++)
        {
            if (newCard.ID == playerCardsInventory[i].ID && playerCardsInventory[i].cardsOnSlot < maxCardsOnSlot) 
            {
                playerCardsInventory[i].cardsOnSlot++;
                break;
            }
        }
    }

    public List<CardSO> GetAllCard() 
    {
        return allCards;
    }

    public void AddCard(CardSO newCard) 
    {
        playerCardsInventory.Add(newCard);
        currentCards++;
    }

    public void RemoveCard(CardSO newCard) 
    {
        for (int i = 0; i < playerCardsInventory.Count; i++) 
        {
            if (playerCardsInventory[i].ID == newCard.ID) 
            {
                playerCardsInventory.Remove(playerCardsInventory[i]);
            }
        }
    }

    public int GetMaxCardOnInventory() 
    {
        return maxCardOnInventory;
    }

    private void OnDisable()
    {
        ClearInventory();
    }
}
