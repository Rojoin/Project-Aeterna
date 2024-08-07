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
    //[SerializeField] private List<CardsSlots> cardsCounter;
    [SerializeField] private VoidChannelSO OnInteraction;

    [SerializeField] private VoidChannelSO OnHudToggle;

    [Header("Card System")]
    [SerializeField] private bool activeCardSystem;

    [SerializeField] private int timeToSpawnShop;

    private void Start()
    {
        //cardsCounter = playerInventory.GetCardsCounterList();
    }

    void Update()
    {
        if (activeCardSystem) 
        {
            CardSystem();
        }
    }

    public IEnumerator ShowSelectableCardMenu() 
    {
        yield return new WaitForSeconds(timeToSpawnShop);


        //selectCardMenu.RefreshCardsSelectedList();
        //selectCardMenu.isCardActivated = !selectCardMenu.isCardActivated;
        //activeCardSystem = selectCardMenu.isCardActivated;
        //selectCardMenu.ShowSelectCardMenu(activeCardSystem);
        
    }

    public void CardSystem()
    {
        playerHud.ShowHud();

        if (selectCardMenu.GetIsCardSelected())
        {
            selectCardMenu.SetIsCardSelected(false);
            playerHud.ShowCardsHud(selectCardMenu.GetCardSelected());

            selectCardMenu.RefreshCardsSelectedList();
        }
    }
}
