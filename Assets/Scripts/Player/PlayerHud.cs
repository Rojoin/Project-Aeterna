using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private SelectCardMenu selectCardMenu;

    [SerializeField] private List<CardDisplay> cardDisplay;

    public List<GameObject> cardGO;

    private CardSO card;

    private int cardIndex;

    void Start()
    {
        card = new CardSO();

        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowHud(int cardSelected) 
    {
        if (playerInventory.GetCurrentCards() <= playerInventory.GetMaxCards()) 
        {
            for (int i = cardIndex; cardIndex < playerInventory.GetCurrentCards();)
            {
                card = playerInventory.GetCardOnInventory(cardSelected);

                cardDisplay[i].ShowCard(card);

                cardGO[i].SetActive(true);

                cardIndex++;

                break;
            }
        }
    }

    public void DesactiveCardsGO() 
    {
        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }

        cardIndex = 0;
    } 
}
