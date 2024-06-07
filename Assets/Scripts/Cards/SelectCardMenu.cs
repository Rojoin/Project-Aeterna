using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCardMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject SelectCardUI;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private BoolChannelSO TogglePause;
    private List<CardSO> cardsSelected = new List<CardSO>();

    private bool isCardSelected;

    private int cardToSelect;

    [Header("Setup")]
    [SerializeField] private int maxCardsToSelect = 6;

    public List<CardSO> allCards = new List<CardSO>();

    public List<CardDisplay> cardsDisplays = new List<CardDisplay>();

    public bool isCardActivated = false;

    void Start()
    {
        allCards = playerInventory.GetAllCardsList();

        ShowSelectCardMenu(false);
        isCardSelected = false;
    }

    public void ShowSelectCardMenu(bool value) 
    {
        SelectCardUI.SetActive(value);
        isCardActivated = value;
        TogglePause.RaiseEvent(value);
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
        CardSO card = ScriptableObject.CreateInstance<CardSO>();

        card.ID = Random.Range(0, maxCardsToSelect);

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
Debug.Log("Card has been Selected");
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

    public CardSO GetCardSelected() 
    {
        CardSO card = ScriptableObject.CreateInstance<CardSO>();

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

        return card;
    }

    public void RefreshCardsSelectedList() 
    {
        cardsSelected.Clear();
    }
}
