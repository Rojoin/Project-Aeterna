using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHudInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerHud playerHud;
    [SerializeField] private SelectCardMenu selectCardMenu;
    [SerializeField] private List<CardsSlots> cardsCounter;
    [SerializeField] private List<LookAtMouse> selectableCard;
    [SerializeField] private VoidChannelSO OnInteraction;

    [SerializeField] private VoidChannelSO OnHudToggle;

    [Header("Card System")]
    [SerializeField] private bool activeCardSystem;

    private void Start()
    {
        cardsCounter = playerInventory.GetCardsCounterList();
        OnInteraction.Subscribe(ToggleStore);
        OnHudToggle.Subscribe(playerHud.ToggleHud);
    }

    void Update()
    {
        if (activeCardSystem) 
        {
            CardSystem();
        }
    }

    public void ToggleStore() 
    {
        if (playerInventory.GetMaxCards() > playerInventory.GetCurrentCards())
        {
            selectCardMenu.RefreshCardsSelectedList();
            selectCardMenu.isCardActivated = !selectCardMenu.isCardActivated;
            activeCardSystem = selectCardMenu.isCardActivated;
            selectCardMenu.ShowSelectCardMenu(activeCardSystem);
        }
    }

    public void CardSystem()
    {
        playerHud.ShowHud();

        for (int i = 0; i < cardsCounter.Count; i++)
        {
            cardsCounter[i].ShowCardsStacksUI();
        }

        if (selectCardMenu.GetIsCardSelected())
        {
            selectCardMenu.SetIsCardSelected(false);
            playerInventory.PickUpCard(selectCardMenu.GetCardSelected());
            playerHud.ShowCardsHud(selectCardMenu.GetCardSelected());

            selectCardMenu.RefreshCardsSelectedList();
        }
    }

    private void OnDestroy()
    {
        OnInteraction.Unsubscribe(ToggleStore);
        OnHudToggle.Unsubscribe(playerHud.ToggleHud);
    }
}
