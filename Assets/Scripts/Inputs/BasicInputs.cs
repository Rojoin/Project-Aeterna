using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerHud playerHud;
    [SerializeField] private SelectCardMenu selectCardMenu;
    [SerializeField] private List<CardsCounter> cardsCounter;
    [SerializeField] private VoidChannelSO OnInteraction;


    [Header("Card System")]
    [SerializeField] private bool activeCardSystem;

    private void Start()
    {
        cardsCounter = playerInventory.GetCardsCounterList();
        OnInteraction.Subscribe(ToggleStore);
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
            activeCardSystem = !activeCardSystem;
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
    }
}
