using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SelectCardMenu : MonoBehaviour
{
    [SerializeField] private BoolChannelSO TogglePause;
    [SerializeField] private VoidChannelSO moveCamera;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Reference: UI")]
    [SerializeField] private GameObject SelectCardUI;

    private bool showCardMenu = false;

    [Header("Reference: Player Inventory")]
    [SerializeField] private PlayerInventory playerInventory;

    private List<CardSO> allCards;

    [Header("Reference: Selectable Cards")]
    [SerializeField] private List<SelectableCardMovement> cardsMovements;
    [SerializeField] private List<SelectableCardDisplay> cardsDisplay;

    [Header("Animations")]
    [SerializeField] private List<Animator> cardsAnimator;

    public List<CardSO> cardsToShow;

    [SerializeField] private Animator invertedCardAnimator;
    [SerializeField] private Animator normalCardAnimator;

    [Header("Setup: Cards")]
    public int maxCardsToSelect = 3;

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

    [ContextMenu("TestCard")]
    public void ShowSelectCardMenuDebug()
    {
        ShowSelectCardMenu(true);
    }
    public void ShowSelectCardMenu(bool value)
    {
        showCardMenu = value;
        SelectCardUI.SetActive(value);
        TogglePause.RaiseEvent(value);

        if (showCardMenu)
        {
            PickCardsToShow();
        }
    }

    public CardSO GetRandomCard(int minRangeOfCards, int maxRangeOfCards)
    {
        CardSO card = ScriptableObject.CreateInstance<CardSO>();

        card.ID = Random.Range(minRangeOfCards, maxRangeOfCards);

        DontRepeatCards(card);

        card.isInverted = IsCardInverted();

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i].ID == card.ID)
            {
                card = allCards[i];
            }
        }

        return card;
    }

    private void DontRepeatCards(CardSO card)
    {
        if (cardsToShow.Count != 0)
        {
            for (int i = 0; i < cardsToShow.Count; i++)
            {
                while (card.ID == cardsToShow[i].ID)
                {
                    card.ID = Random.Range(0, playerInventory.GetMaxCards());
                }
            }
        }
    }

    private void PickCardsToShow()
    {
        for (int i = 0; i < maxCardsToSelect; i++)
        {
            if (playerInventory.GetInventory().Count == playerInventory.GetMaxCardOnInventory())
            {
                cardsToShow.Add(playerInventory.GetInventory()[i]);
            }

            if (cardsToShow.Count == 0 || playerInventory.GetInventory().Count != playerInventory.GetMaxCardOnInventory())
            {
                cardsToShow.Add(GetRandomCard(0, playerInventory.GetMaxCards()));
            }

            cardsDisplay[i].ShowCardImage(cardsToShow[i]);

            cardsToShow[i].slotIndex = i;

            PlayInvertedCardAnimation(cardsToShow[i]);
            PlayNormalCardAnimation(cardsToShow[i]);
        }        
    }

    public bool IsCardInverted()
    {
        int maxPercentage = 100;
        int isInverted = 30;

        int cardInvertedChanse = Random.Range(0, maxPercentage);

        if (cardInvertedChanse <= isInverted)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void PlayInvertedCardAnimation(CardSO card)
    {
        if (card.isInverted)
        {
            card.animator = invertedCardAnimator;

            StartCoroutine(ChangeInvertCardAnimation(card));
        }
    }

    public void PlayNormalCardAnimation(CardSO card) 
    {
        if (!card.isInverted) 
        {
            card.animator = normalCardAnimator;
        }
    }

    public IEnumerator ChangeInvertCardAnimation(CardSO card)
    {
        yield return new WaitForSeconds(1);

        card.animator.SetBool("IsPresentationEnd", true);
    }

    private void SetCardSelected(int cardSelected)
    {
        List<CardSO> inventory = playerInventory.GetInventory();
        bool isOnInvetory = false;

        for (int i = 0; i < playerInventory.GetCurrentCards(); i++)
        {
            if (inventory[i] == cardsToShow[cardSelected])
            {
                isOnInvetory = true;

                break;
            }
        }

        if (!isOnInvetory && inventory.Count > 0)
        {
            playerInventory.AddCard(cardsToShow[cardSelected]);
            playerInventory.SetCardsOnSlot(cardsToShow[cardSelected]);
        }

        if (inventory.Count == 0)
        {
            playerInventory.AddCard(cardsToShow[cardSelected]);
            playerInventory.SetCardsOnSlot(cardsToShow[cardSelected]);
        }

        else
        {
            playerInventory.SetCardsOnSlot(cardsToShow[cardSelected]);
        }

        ShowSelectCardMenu(false);
    }
}