using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image allCardSprite;
    public Image cardFrame;
    public Image accumulatedCard;
    public Image completedCard;

    private void Start()
    {
        allCardSprite.enabled = false;
        cardFrame.enabled = false;
    }

    public void ShowCard(CardSO card) 
    {
        allCardSprite.enabled = true;
        cardFrame.enabled = true;
        allCardSprite.sprite = card.allCardSprite;
        cardFrame.sprite = card.cardFrame;

        if (card.cardsOnSlot == 2)
        {
            accumulatedCard.enabled = true;
            accumulatedCard.sprite = card.cardAccumulatedFrame;
            completedCard.enabled = false;
        }
        else if (card.cardsOnSlot == 3)
        {
            accumulatedCard.enabled = true;
            accumulatedCard.sprite = card.cardCompleteFrame;
            completedCard.sprite = card.cardCompleteFrame;
            completedCard.enabled = true;
        }
    }
}
