using InputControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SelectCardMenu : MonoBehaviour
{
    [SerializeField] private BoolChannelSO TogglePause;
    [SerializeField] private VoidChannelSO moveCamera;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Channel: UI")] [SerializeField]
    private BoolChannelSO ToggleCardDialogCard;

    [SerializeField] private BoolChannelSO ToggleCardDialogDoor;
    [SerializeField] private VoidChannelSO InvokeCardChannel;

    [Header("Reference: UI")] [SerializeField]
    private GameObject SelectCardUI;

    private bool showCardMenu = false;

    [Header("Reference: Player Inventory")] [SerializeField]
    private PlayerInventory playerInventory;

    private List<CardSO> allCards;

    [Header("Reference: Selectable Cards")] [SerializeField]
    private List<SelectableCardMovement> cardsMovements;

    [SerializeField] private List<SelectableCardDisplay> cardsDisplay;

    [Header("Reference: Buff System")] [SerializeField]
    private BuffSystem buffSystem;

    [Header("Animations")] [SerializeField]
    private List<Animator> cardsAnimator;

    [Header("Box")] [SerializeField] private CanvasGroup dialogBoxDoor;
    [SerializeField] private CanvasGroup dialogBoxCard;

    private float timeUntilCardsShowUp = 0.20f;
    private float timeBetweenCards = 0.10f;
    private float timeUntilCardsDissapear = 0.10f;

    [SerializeField] private Animator selectCardMenuAnimator;

    [Header("CardsGO")] [SerializeField] private List<GameObject> cardsGO;

    [Header("Hud")] [SerializeField] private GameObject hud;
    [SerializeField] private GameObject objective;

    public List<CardSO> cardsToShow;

    [Header("Setup: Cards")] public int maxCardsToSelect = 3;


    void Start()
    {
        ShowSelectCardMenu(false);
        allCards = playerInventory.GetAllCard();
        ToggleDialogCard(false);
        ToggleDialogDoor(false);
    }

    private void OnEnable()
    {
        moveCamera.Subscribe(TurnGameOver);
        ToggleCardDialogCard.Subscribe(ToggleDialogCard);
        ToggleCardDialogDoor.Subscribe(ToggleDialogDoor);
        InvokeCardChannel.Subscribe(ShowSelectCardMenuDebug);
        selectCardMenuAnimator.SetTrigger("setStart");
    }

    private void OnDisable()
    {
        InvokeCardChannel.Unsubscribe(ShowSelectCardMenuDebug);
        ToggleCardDialogCard.Unsubscribe(ToggleDialogCard);
        ToggleCardDialogDoor.Unsubscribe(ToggleDialogDoor);
        moveCamera.Unsubscribe(TurnGameOver);
    }

    private void ToggleDialogCard(bool value)
    {
        dialogBoxCard.interactable = value;
        dialogBoxCard.blocksRaycasts = !value;
        dialogBoxCard.alpha = value ? 1 : 0;
    }

    private void ToggleDialogDoor(bool value)
    {
        dialogBoxDoor.interactable = value;
        dialogBoxDoor.blocksRaycasts = !value;
        dialogBoxDoor.alpha = value ? 1 : 0;
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
                    cardsDisplay[i].ShowCardTittle(cardsToShow[i]);
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
        hud.SetActive(!value);
        objective.SetActive(!value);

        showCardMenu = value;
        SelectCardUI.SetActive(value);
        TogglePause.RaiseEvent(value);
        InputController.IsGamePaused = value;
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

            if (cardsToShow.Count == 0 ||
                playerInventory.GetInventory().Count != playerInventory.GetMaxCardOnInventory())
            {
                cardsToShow.Add(GetRandomCard(0, playerInventory.GetMaxCards()));
            }

            cardsToShow[i].slotIndex = i;

            if (cardsToShow[i].isInverted)
            {
                StartCoroutine(ChangeInvertCardAnimation(cardsToShow[i].slotIndex));
            }

            cardsDisplay[i].ShowCardImage(cardsToShow[i]);
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

    public IEnumerator ChangeInvertCardAnimation(int cardID)
    {
        cardsAnimator[cardID].SetBool("IsReverse", true);

        yield return new WaitForSeconds(1);

        cardsAnimator[cardID].SetBool("StartReverseMovement", true);
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

        if (!isOnInvetory)
        {
            playerInventory.AddCard(cardsToShow[cardSelected]);
            playerInventory.SetCardsOnSlot(cardsToShow[cardSelected]);
        }

        if (isOnInvetory)
        {
            playerInventory.SetCardsOnSlot(cardsToShow[cardSelected]);
        }

        if (cardsToShow[cardSelected].cardsOnSlot < 3)
            buffSystem.CheckCardType(cardsToShow[cardSelected]);

        StartCoroutine(DesactiveSelectCardMenu(1));

        for (int i = 0; i < cardsToShow.Count; i++)
        {
            cardsAnimator[i].SetBool("StartReverseMovement", false);
        }
    }

    private IEnumerator DesactiveSelectCardMenu(int time)
    {
        selectCardMenuAnimator.SetBool("IsDesactive", true);

        yield return new WaitForSeconds(time);

        ShowSelectCardMenu(false);
        selectCardMenuAnimator.SetBool("IsDesactive", false);
    }
}