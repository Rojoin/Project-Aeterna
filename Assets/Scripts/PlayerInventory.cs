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

    private int randCard = 0;

    void Start()
    {
        currentCards = -1;
        maxCards = 5;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentCards < maxCards) 
        {
            randCard = Random.Range(0, maxCards);
            StartCoroutine(PickUpCard(randCard));
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            RemoveAllCards();
        }
    }

    public IEnumerator PickUpCard(int cardID) 
    {
        for (int i = 0; i < allCards.Count; i++) 
        {
            if (cardID == allCards[i].ID)
            {
                AddCard(allCards[i]);
                break;
            }
        }

        yield return new WaitForSeconds(1);
    }

    public void AddCard(CardSO newCard) 
    {
        playerCardsInventory.Add(newCard);
        currentCards++;
    }

    public void RemoveCard(int cardID) 
    {
        for (int i = 0; i < playerCardsInventory.Count; i++) 
        {
            if (playerCardsInventory[i].ID == cardID && currentCards > 0) 
            {
                currentCards--;
                playerCardsInventory.Remove(playerCardsInventory[i]);
                Debug.Log("Remove card:" + playerCardsInventory[i].ID);
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

        return null;
    }

    public int GetRandomCard() 
    {
        return randCard;
    }

    public void RemoveAllCards()
    {
        currentCards = 0;
        playerCardsInventory.Clear();
    }

    public int GetCurrentCard() 
    {
        return currentCards;
    }
}
