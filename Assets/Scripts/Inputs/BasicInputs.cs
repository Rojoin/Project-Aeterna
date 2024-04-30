using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerHud playerHud;
    [SerializeField] private SelectCardMenu selectCardMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInventory.GetMaxCards() > playerInventory.GetCurrentCards())
        {
            selectCardMenu.ShowSelectCardMenu(true);
        }

        if (selectCardMenu.GetIsCardSelected())
        {
            selectCardMenu.SetIsCardSelected(false);
            playerInventory.PickUpCard(selectCardMenu.GetCardSelected());
            playerHud.ShowHud(selectCardMenu.GetCardSelected());
            selectCardMenu.RefreshCardsSelectedList();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            playerInventory.RemoveAllCards();
            playerHud.DesactiveCardsGO();
        }
    }
}
