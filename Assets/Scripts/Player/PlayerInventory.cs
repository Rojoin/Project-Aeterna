using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private List<CardsSlots> cardsSlots;

    [SerializeField] private List<CardSO> allCards;

    [SerializeField] private List<CardSO> playerCardsInventory;

    [SerializeField] private int maxCards;

    [SerializeField] private int currentCards;

    private int upgradeValue = 0;
    private float newHealth = 0;
    private float newDamage = 0;
    private float newSpeed = 0;

    public void PickUpCard(CardSO card)
    {
        if (maxCards > currentCards)
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].ID == card.ID)
                {
                    AddCard(allCards[i]);

                    if (allCards[i].cardsOnSlot < 3) 
                    {
                        allCards[i].cardsOnSlot++;

                        CheckPlayerInventory(allCards[i]);
                    }

                    break;
                }
            }
        }
    }

    public void AddCard(CardSO newCard)
    {
        if (currentCards > 0)
        {
            for (int i = 0; i < playerCardsInventory.Count; i++)
            {
                if (newCard == playerCardsInventory[i])
                {
                    cardsSlots[i].SetCurrentCardsInSlot(1);

                    if (playerCardsInventory[i].cardsOnSlot < 3)
                    {
                        currentCards++;
                    }

                    break;
                }

                else if (cardsSlots[i + 1].GetCurrentCardsInSlot() == 0)
                {
                    playerCardsInventory.Add(newCard);
                    cardsSlots[i + 1].SetCurrentCardsInSlot(1);

                    if (playerCardsInventory[i].cardsOnSlot < 3)
                    {
                        currentCards++;
                    }

                    break;
                }
            }
        }

        else
        {
            playerCardsInventory.Add(newCard);
            cardsSlots[currentCards].SetCurrentCardsInSlot(1);
            currentCards++;
        }
    }

    public void CheckPlayerInventory(CardSO newCard)
    {
        if (currentCards > 0)
        {
            switch (newCard.cardsOnSlot)
            {
                case 1:
                    upgradeValue = 1;
                    UpdatePlayerStacks(newCard, upgradeValue);
                break;

                case 2:
                    upgradeValue = 2;
                    UpdatePlayerStacks(newCard, upgradeValue);
                break;

                case 3:
                    upgradeValue = 3;
                    UpdatePlayerStacks(newCard, upgradeValue);
                break;
            }
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

    public CardSO GetCardOnInventory(CardSO card)
    {
        for (int i = 0; i < playerCardsInventory.Count; i++)
        {
            if (playerCardsInventory[i].ID == card.ID)
            {
                return playerCardsInventory[i];
            }
        }

        return playerCardsInventory[0];
    }

    public void UpdatePlayerStacks(CardSO newCard, int valueToUpgrade)
    {
        newHealth = playerHealth.GetHealth();
        newDamage = playerHealth.GetDamage();
        newSpeed = playerHealth.GetSpeed();

        for (int i = 0; i < valueToUpgrade; i++)
        {
            switch (newCard.cardType)
            {
                case CardSO.CardType.Attack:

                    newDamage += newCard.damage;

                    break;

                case CardSO.CardType.Health:

                    newHealth += newCard.health;

                    break;

                case CardSO.CardType.Speed:

                    if (newSpeed >= playerMovement.maxSpeed)
                    {
                        newSpeed = playerMovement.maxSpeed;
                    }

                    else
                    {
                        newSpeed += newCard.speed;
                    }

                    break;
            }
        }

        playerHealth.SetHealth(newHealth);
        playerHealth.SetDamage(newDamage);
        playerHealth.SetSpeed(newSpeed);
    }

    public int GetMaxCards()
    {
        return maxCards;
    }

    public int GetCurrentCards()
    {
        return currentCards;
    }

    public List<CardsSlots> GetCardsCounterList()
    {
        return cardsSlots;
    }

    public List<CardSO> GetAllCardsList()
    {
        return allCards;
    }

    public List<CardSO> GetPlayerCardsInventoryList() 
    {
        return playerCardsInventory;
    }

    private void OnDestroy()
    {
        currentCards = 0;
        playerCardsInventory.Clear();
    }

    private void OnDisable()
    {
        for (int i = 0; i < allCards.Count; i++) 
        {
            allCards[i].ResetStacks();
        }
    }
}
