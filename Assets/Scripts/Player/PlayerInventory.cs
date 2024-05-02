using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth player;

    [SerializeField] private List<CardsCounter> cardsCounter;

    [SerializeField] private int maxCards;

    [SerializeField] private int currentCards;

    [SerializeField] private List<CardSO> playerCardsInventory;

    [SerializeField] private List<CardSO> allCards;

    private float aux = 0;
    private float newHealth = 0;
    private float newDamage = 0;

    void Start()
    {
        currentCards = 0;
        maxCards = 5;
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
                    UpdatePlayerStacks();
                    break;
                }
            }
        }
    }

    public void AddCard(CardSO newCard) 
    {
        if(currentCards > 0) 
        {
            for (int i = 0; i < playerCardsInventory.Count; i++)
            {
                if (newCard == playerCardsInventory[i])
                {
                    cardsCounter[i].SetCurrentCardsInSlot(1);

                    break;
                }

                else if (cardsCounter[i + 1].GetCurrentCardsInSlot() == 0) 
                {
                    playerCardsInventory.Add(newCard);
                    cardsCounter[currentCards].SetCurrentCardsInSlot(1);
                    currentCards++;

                    break;
                }
            }
        }

        else 
        {
            playerCardsInventory.Add(newCard);
            cardsCounter[currentCards].SetCurrentCardsInSlot(1);
            currentCards++;
        }
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

    public void UpdatePlayerStacks()
    {
        newHealth = player.GetHealth();
        newDamage = player.GetDamage();

        if (currentCards > 0) 
        {
            for (int i = 0; i < playerCardsInventory.Count; i++)
            {
                if (cardsCounter[i].GetCurrentCardsInSlot() >= 0 && cardsCounter[i].GetCurrentCardsInSlot() < 2)
                {
                    newHealth += playerCardsInventory[i].helath;
                    newDamage += playerCardsInventory[i].damage;
                }

                if (cardsCounter[i].GetCurrentCardsInSlot() == 2) 
                {
                    aux = 2;

                    newHealth += playerCardsInventory[i].helath * aux;
                    newDamage += playerCardsInventory[i].damage * aux;
                }

                if (cardsCounter[i].GetCurrentCardsInSlot() == 3)
                {
                    aux = 3;

                    newHealth += playerCardsInventory[i].helath * aux;
                    newDamage += playerCardsInventory[i].damage * aux;
                }
            }
        }

        player.SetHealth(newHealth);
        player.SetDamage(newDamage);
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

    public List<CardsCounter> GetCardsCounterList() 
    {
        return cardsCounter;
    }

    public List<CardSO> GetAllCardsList() 
    {
        return allCards;
    }
}
