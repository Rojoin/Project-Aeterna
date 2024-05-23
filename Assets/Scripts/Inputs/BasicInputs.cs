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

    [SerializeField] private bool activeHud;

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

        if (activeHud) 
        {
            playerHud.ActiveHud();
        }

        else 
        {
            playerHud.DesactiveHud();
        }
        

        if (Input.GetKeyDown(KeyCode.V))
        {
            playerHud.DesactiveHud();
        }
    }

    public void ToggleStore() 
    {
        if (playerInventory.GetMaxCards() > playerInventory.GetCurrentCards())
        {
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
    }
}
