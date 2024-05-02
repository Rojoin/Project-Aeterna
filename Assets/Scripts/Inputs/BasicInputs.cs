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

    private void Start()
    {
        cardsCounter = playerInventory.GetCardsCounterList();
    }

    void Update()
    {
        playerHud.ShowHud();

        if (Input.GetKeyDown(KeyCode.E) && playerInventory.GetMaxCards() > playerInventory.GetCurrentCards())
        {
            selectCardMenu.ShowSelectCardMenu(true);
        }

        if (selectCardMenu.GetIsCardSelected())
        {
            selectCardMenu.SetIsCardSelected(false);
            playerInventory.PickUpCard(selectCardMenu.GetCardSelected());
            playerHud.ShowCardsHud(selectCardMenu.GetCardSelected());

            for (int i = 0; i < cardsCounter.Count; i++)
            {
                cardsCounter[i].ShowCardsStacksUI();
            }

            selectCardMenu.RefreshCardsSelectedList();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            playerInventory.RemoveAllCards();
            playerHud.DesactiveCardsGO();
        }
    }
}
