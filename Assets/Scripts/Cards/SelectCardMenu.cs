using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SelectCardMenu : MonoBehaviour
{
    [Header("Reference: UI")]
    [SerializeField] private GameObject SelectCardUI;

    private bool showCardMenu = false;

    [Header("Reference: PlayerInventory")]
    [SerializeField] private PlayerInventory playerInventory;

    private List<CardSO> allCards;

    [SerializeField] private BoolChannelSO TogglePause;
    [SerializeField] private VoidChannelSO moveCamera;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Reference: SelectableCards")]
    [SerializeField] private List<SelectableCardMovement> cardsMovements;
    [SerializeField] private List<SelectableCardDisplay> cardsDisplay;

    public List<CardSO> cardsToShow;

    [Header("Setup: Cards")]
    [SerializeField] private int maxCardsToSelect = 3;

    public TextMeshProUGUI description;

    void Start()
    {
        ShowSelectCardMenu(false);
        allCards = playerInventory.GetAllCard();
    }

    private void OnEnable()
    {
        moveCamera.Subscribe(TurnGameOver);
    }

    private void OnDisable()
    {
        moveCamera.Unsubscribe(TurnGameOver);
    }

    private void TurnGameOver()
    {
        gameOverScreen.SetActive(true);
    }

    private void Update()
    {
        PickCardsToShow();

        if (showCardMenu)
        {
            for (int i = 0; i < cardsMovements.Count; i++)
            {
                if (cardsMovements[i].IsSelected())
                {
                    cardsDisplay[i].ShowCardDescription(cardsToShow[i]);
                }
            }
        }

        else
        {
            cardsToShow.Clear();
        }
    }

    public void ShowSelectCardMenu(bool value)
    {
        showCardMenu = value;
        SelectCardUI.SetActive(value);
        TogglePause.RaiseEvent(value);
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
            }
        }

        return card;
    }

    private void PickCardsToShow()
    {
        if (cardsToShow.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                cardsToShow.Add(GetRandomCard());

                cardsDisplay[i].ShowCardImage(cardsToShow[i]);
            }
        }
    }
}