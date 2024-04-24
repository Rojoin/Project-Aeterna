using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntitySO player;

    [SerializeField] private List<CardSO> cards;

    private CardSO card;

    void Start()
    {

    }

    public void AddCard(CardSO newCard) 
    {
        cards.Add(newCard);
    }

    public void RemoveCard(int cardID) 
    {
        for (int i = 0; i < cards.Count; i++) 
        {
            if (cards[i].ID == cardID) 
            {
                cards.Remove(cards[i]);
            }
        }
    }

    public CardSO GetCard(int cardID) 
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].ID == cardID)
            {
                return cards[i];
            }
        }

        return null;
    }

    public void RemoveAllCards() 
    {
        cards.Clear();
    }
}
