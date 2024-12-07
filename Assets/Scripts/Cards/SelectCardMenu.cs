using InputControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SelectCardMenu : MonoBehaviour
{
    [SerializeField] private PlayerEntitySO player;
    [SerializeField] private BoolChannelSO TogglePause;
    [SerializeField] private BoolChannelSO moveCamera;
    [SerializeField] private CanvasGroup gameOverScreen;
    [SerializeField] private GameObject gameOverButton;
    [SerializeField] private CanvasGroup winScreen;
    [SerializeField] private CanvasGroup playerHud;
    [SerializeField] private PauseMenu pause;
    [SerializeField] private CardInformationMenu cardInformationMenu;

    [Header("Channel: UI")] 
    [SerializeField] private BoolChannelSO ToggleCardDialogCard;

    [SerializeField] private BoolChannelSO ToggleCardDialogDoor;
    [SerializeField] private VoidChannelSO OnTutorialComplete;
    [SerializeField] private VoidChannelSO InvokeCardChannel;

    [Header("Reference: UI")] 
    [SerializeField] private GameObject SelectCardUI;

    private bool showCardMenu = false;

    [Header("Reference: Player Inventory")] 
    [SerializeField] private PlayerInventory playerInventory;

    private List<CardSO> allCards;

    [Header("Reference: Selectable Cards")] 
    [SerializeField] private List<SelectableCardMovement> cardsMovements;

    [SerializeField] private List<SelectableCardDisplay> cardsDisplay;

    [Header("Reference: Buff System")] 
    [SerializeField] private BuffSystem buffSystem;

    [Header("Animations")] 
    [SerializeField] private List<Animator> cardsAnimator;

    [Header("Box")]
    [SerializeField] private CanvasGroup dialogBoxDoor;
    [SerializeField] private CanvasGroup dialogBoxCard;
    [SerializeField] private CanvasGroup tutorialBox;
    [SerializeField] private CanvasGroup objectiveBox;

    private float timeUntilCardsShowUp = 0.20f;
    private float timeBetweenCards = 0.10f;
    private float timeUntilCardsDissapear = 0.10f;
    private float timeUntilMenuAppears = 0.50f;
    private float timeUntilGoBackToMenu = 5.0f;
    [SerializeField] private float timeUntilGameOverAppears = 5.0f;

    [SerializeField] private Animator selectCardMenuAnimator;

    [Header("CardsGO")]
    [SerializeField] private List<GameObject> cardsGO;

    [Header("Hud")] [SerializeField] private GameObject hud;
    [SerializeField] private GameObject objective;

    public List<CardSO> cardsToShow;

    [Header("Setup: Cards")] public int maxCardsToSelect = 3;


    void Start()
    {
        ShowSelectCardMenu(false);

        cardInformationMenu = GetComponent<CardInformationMenu>();

        cardInformationMenu.Toggle(false);

        allCards = playerInventory.GetAllCard();
        dialogBoxCard.SetCanvas(false);
        dialogBoxDoor.SetCanvas(false);
        objectiveBox.SetCanvas(false);
        StartCoroutine(tutorialBox.FadeCanvas(true, timeUntilMenuAppears));
    }

    private void OnEnable()
    {
        moveCamera.Subscribe(TurnGameOver);
        ToggleCardDialogCard.Subscribe(ToggleDialogCard);
        ToggleCardDialogDoor.Subscribe(ToggleDialogDoor);
        InvokeCardChannel.Subscribe(ShowSelectCardMenuDebug);
        OnTutorialComplete.Subscribe(ChangeTutorialManager);
        selectCardMenuAnimator.SetTrigger("setStart");
    }

    private void OnDisable()
    {
        OnTutorialComplete.Unsubscribe(ChangeTutorialManager);
        InvokeCardChannel.Unsubscribe(ShowSelectCardMenuDebug);
        ToggleCardDialogCard.Unsubscribe(ToggleDialogCard);
        ToggleCardDialogDoor.Unsubscribe(ToggleDialogDoor);
        moveCamera.Unsubscribe(TurnGameOver);
    }


    private void ToggleDialogCard(bool value)
    {
        StartCoroutine(dialogBoxCard.FadeCanvas(value, timeUntilMenuAppears));
    }

    private void ChangeTutorialManager()
    {
        StartCoroutine(tutorialBox.FadeCanvas(false, timeUntilMenuAppears));
        StartCoroutine(objectiveBox.FadeCanvas(true, timeUntilCardsDissapear));
    }

    private void ToggleDialogDoor(bool value)
    {
        StartCoroutine(dialogBoxDoor.FadeCanvas(value, timeUntilMenuAppears));
    }

    private void TurnGameOver(bool value)
    {
        if (value)
        {
            cardInformationMenu.Toggle(false);
            playerHud.SetCanvas(false);
            dialogBoxDoor.SetCanvas(false);
            dialogBoxCard.SetCanvas(false);
            tutorialBox.SetCanvas(false);
            objectiveBox.SetCanvas(false);
            playerInventory.ResetCardsStats();
            player.ResetPlayerStats();
            StartCoroutine(ActivateGameOver());
            EventSystem.current.SetSelectedGameObject(gameOverButton);
        }
        else
        {
            StartCoroutine(winScreen.FadeCanvas(true, timeUntilCardsDissapear));
            playerInventory.ResetCardsStats();
            player.ResetPlayerStats();
            Invoke(nameof(GoBackToMenu),timeUntilGoBackToMenu);
        }
    }
    private IEnumerator ActivateGameOver()
    {
        yield return new WaitForSecondsRealtime(timeUntilGameOverAppears);
        Time.timeScale = 0;
        StartCoroutine(gameOverScreen.FadeCanvas(true, timeUntilCardsDissapear));
        EventSystem.current.SetSelectedGameObject(gameOverButton);
    }

    private void GoBackToMenu()
    {
        pause.GoMenu();
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
        EventSystem.current.SetSelectedGameObject(cardsMovements[0].gameObject);
        Time.timeScale = 1;
        hud.SetActive(!value);
        objective.SetActive(!value);

        if (value)
        {
            AkSoundEngine.PostEvent("Cards_Show_Deck", gameObject);
        }

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

        for (int i = 0; i < cardsAnimator.Count; i++)
        {
            cardsAnimator[i].SetBool("StartReverseMovement", false);
        }

        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].ResetSlotIndex();
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

public static class CanvasUtilities
{
    public static IEnumerator FadeCanvas(this CanvasGroup canvasGroup, bool value, float time)
    {
        float timer = 0.0f;
        if (value)
        {
            float alpha = 0;
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                alpha = timer / time;
                canvasGroup.SetCanvas(false, alpha);
                yield return null;
            }

            canvasGroup.SetCanvas(true, 1);
        }
        else
        {
            timer = time;
            float alpha = 0;
            while (timer > 0)
            {
                timer -= Time.unscaledDeltaTime;
                alpha = timer / time;
                canvasGroup.SetCanvas(false, alpha);
                yield return null;
            }

            canvasGroup.SetCanvas(false, 0);
        }
    }

    public static void SetCanvas(this CanvasGroup canvasGroup, bool value = true)
    {
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
        canvasGroup.alpha = value ? 1 : 0;
    }

    public static void SetCanvas(this CanvasGroup canvasGroup, bool value, float alpha)
    {
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
        canvasGroup.alpha = alpha;
    }
}