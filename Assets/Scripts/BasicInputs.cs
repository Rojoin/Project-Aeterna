using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerHud playerHud;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInventory.rand = Random.Range(0, playerInventory.GetMaxCards());
            playerInventory.PickUpCard(playerInventory.rand);
            playerHud.ShowHud();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            playerInventory.RemoveAllCards();
            playerHud.DesactiveCardsGO();
        }
    }
}
