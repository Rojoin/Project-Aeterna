using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntitySO player;

    [SerializeField] private int maxCards;

    [SerializeField] private int currentCards;

    [SerializeField] private List<CardSO> playerCardsInventory;

    [SerializeField] private List<CardSO> allCards;

    public int rand;

    void Start()
    {
        currentCards = 0;
        maxCards = 5;

        rand = Random.Range(0, GetMaxCards());
    }

    public void PickUpCard(int cardID) 
    {
        if (maxCards > currentCards) 
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].ID == cardID)
                {
                    AddCard(allCards[i]);

                    break;
                }
            }
        }
    }

    public void AddCard(CardSO newCard) 
    {
        playerCardsInventory.Add(newCard);
        currentCards++;

        Debug.Log(currentCards);
    }

    public void RemoveCard(int cardID) 
    {
        for (int i = 0; i < GetMaxCards(); i++) 
        {
            if (playerCardsInventory[i].ID == cardID && currentCards > 0) 
            {
                playerCardsInventory.Remove(playerCardsInventory[i]);
                Debug.Log("Remove card:" + playerCardsInventory[i].ID);
                currentCards--;
            }
        }
    }

    public CardSO GetCardOnInventory(int cardID) 
    {
        for (int i = 0; i < playerCardsInventory.Count; i++)
        {
            if (playerCardsInventory[i].ID == cardID)
            {
                return playerCardsInventory[i];
            }
        }



        return playerCardsInventory[0];
    }

    public int GetRandomCard() 
    {
        return rand;
    }

    public int GetMaxCards() 
    {
        return maxCards;
    }

    public void RemoveAllCards()
    {
        currentCards = 0;
        playerCardsInventory.Clear();
    }

    public int GetCurrentCards() 
    {
        return currentCards;
    }
}
