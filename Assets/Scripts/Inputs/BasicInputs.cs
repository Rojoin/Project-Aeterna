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

    [Header("Card System")]
    [SerializeField] private bool ActiveCardSystem;

    private void Start()
    {
        cardsCounter = playerInventory.GetCardsCounterList();
    }

    void Update()
    {
        if (ActiveCardSystem) 
        {
            playerHud.ShowHud();

            for (int i = 0; i < cardsCounter.Count; i++)
            {
                cardsCounter[i].ShowCardsStacksUI();
            }

            if (Input.GetKeyDown(KeyCode.E) && playerInventory.GetMaxCards() > playerInventory.GetCurrentCards())
            {
                selectCardMenu.ShowSelectCardMenu(true);
            }

            if (selectCardMenu.GetIsCardSelected())
            {
                selectCardMenu.SetIsCardSelected(false);
                playerInventory.PickUpCard(selectCardMenu.GetCardSelected());
                playerHud.ShowCardsHud(selectCardMenu.GetCardSelected());

                selectCardMenu.RefreshCardsSelectedList();
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                playerInventory.RemoveAllCards();
                playerHud.DesactiveCardsGO();
            }
        }
    }
}
