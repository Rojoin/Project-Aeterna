using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private int maxCardsToSelect = 3;

    private List<CardSO> cardList = new List<CardSO>();

    public List<CardSO> allCards = new List<CardSO>();

    public List<SelectableCardDisplay> seletableCardsDisplays = new List<SelectableCardDisplay>();

    public List<SelectableCardMovement> selectableCardMovements = new List<SelectableCardMovement>();

    public bool isCardActivated = false;

    public TextMeshProUGUI description;

    void Start()
    {
        allCards = playerInventory.GetAllCardsList();

        ShowSelectCardMenu(false);
        isCardSelected = false;
    }

    private void Update()
    {
        if (isCardActivated) 
        {
            for (int i = 0; i < selectableCardMovements.Count; i++)
            {
                if (selectableCardMovements[i].IsSelected())
                {
                    seletableCardsDisplays[i].ShowCardDescription(cardList[i]);
                }
            }
        }
    }

    public void ShowSelectCardMenu(bool value) 
    {
        SelectCardUI.SetActive(value);
        isCardActivated = value;
        TogglePause.RaiseEvent(value);

        if (value == true) 
        {
            cardList.Clear();

            for (int i = 0; i < seletableCardsDisplays.Count; i++)
            {
                CardSO card = GetRandomCard();

                cardList.Add(card);

                seletableCardsDisplays[i].ShowCardImage(card);
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
        isCardSelected = true;

        for (int i = 0; i < selectableCardMovements.Count; i++)
        {
            selectableCardMovements[i].isSelected = false;
            selectableCardMovements[i].canMove = true;
        }

        description.text = "";

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
