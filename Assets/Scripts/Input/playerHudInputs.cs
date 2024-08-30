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
    [SerializeField] private VoidChannelSO OnInteraction;

    [SerializeField] private VoidChannelSO OnHudToggle;

    [Header("Card System")]
    [SerializeField] private bool activeCardSystem;

    [SerializeField] private int timeToSpawnShop;

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

        selectCardMenu.ShowSelectCardMenu(activeCardSystem);
    }

    public void CardSystem()
    {
        playerHud.ShowCardsHud();
    }
}
