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

    private void Start()
    {
        maxCards = allCards.Count;
    }

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
        if (playerCardsInventory.Count > 0) 
        {
            CorruptCards(newCard);
        }

        else 
        {
            playerCardsInventory.Add(newCard);
            currentCards++;
        }
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

    public void CorruptCards(CardSO newCard) 
    {
        List<CardSO> inventory = playerCardsInventory;

        CardSO card = newCard;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (newCard.isInverted && newCard.cardType == inventory[i].cardType) 
            {
                int newCardOnSlot = inventory[i].cardsOnSlot + 1;

                inventory[i] = newCard;
                inventory[i].cardsOnSlot = newCardOnSlot;

                if (inventory[i].cardsOnSlot > 3) 
                {
                    inventory[i].cardsOnSlot = 3;
                }

                card = inventory[i];
                return;
            }

            if(!newCard.isInverted && newCard.cardType == inventory[i].cardType)
            {
                card = inventory[i];
                return;
            }
        }

        playerCardsInventory.Add(card);
        currentCards++;
    }

    public void ResetCardsStats() 
    {
        for (int i = 0;i < allCards.Count; i++) 
        {
            allCards[i].ResetStacks();
        }
    }

    private void OnDisable()
    {
        ClearInventory();
    }
}
