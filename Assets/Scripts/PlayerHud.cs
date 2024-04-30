using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private CardDisplay[] cardDisplay; 

    public GameObject[] cardGO;

    private CardSO[] card;

    private int cardIndex;

    void Start()
    {
        card = new CardSO[playerInventory.GetMaxCards()];

        for (int i = 0; i < cardGO.Length; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowHud() 
    {
        if (playerInventory.GetCurrentCards() <= playerInventory.GetMaxCards()) 
        {
            for (int i = cardIndex; cardIndex < playerInventory.GetCurrentCards();)
            {
                card[i] = playerInventory.GetCardOnInventory(playerInventory.rand);

                cardDisplay[i].ShowCard(card[i]);

                cardGO[i].SetActive(true);

                cardIndex++;

                break;
            }
        }
    }

    public void DesactiveCardsGO() 
    {
        for (int i = 0; i < cardGO.Length; i++)
        {
            cardGO[i].SetActive(false);
        }
    } 
}
