using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCardMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject SelectCardUI;
    [SerializeField] private PlayerInventory playerInventory;

    public List<CardSO> cardsSelected = new List<CardSO>();

    private bool isCardSelected;

    private int cardToSelect;

    public List<CardSO> allCards = new List<CardSO>();

    public List<CardDisplay> cardsDisplays = new List<CardDisplay>();

    void Start()
    {
        ShowSelectCardMenu(false);
        isCardSelected = false;
    }

    public void ShowSelectCardMenu(bool value) 
    {
        SelectCardUI.SetActive(value);

        if (value == true) 
        {
            for (int i = 0; i < cardsDisplays.Count; i++)
            {
                cardsDisplays[i].ShowCard(GetRandomCard());
            }
        }
    }

    public CardSO GetRandomCard() 
    {
        CardSO card = new CardSO();

        card.ID = Random.Range(0, playerInventory.GetMaxCards());

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i].ID == card.ID)
            {
                card = allCards[i];

                cardsSelected.Add(card);
            }
        }

        return card;
    }

    public void SetCardSelected(int value) 
    {
        cardToSelect = value;

        isCardSelected = true;
        ShowSelectCardMenu(false);
    }

    public void SetIsCardSelected(bool value) 
    {
        isCardSelected = value;
    }

    public bool GetIsCardSelected() 
    {
        return isCardSelected;
    }

    public int GetCardSelected() 
    {
        CardSO card = new CardSO();

        switch (cardToSelect) 
        {
            case 0:

                card = cardsSelected[0];

            break;

            case 1:

                card = cardsSelected[1];

            break;

            case 2:

                card = cardsSelected[2];

            break;
        }

        return card.ID;
    }

    public void RefreshCardsSelectedList() 
    {
        cardsSelected.Clear();
    }
}
