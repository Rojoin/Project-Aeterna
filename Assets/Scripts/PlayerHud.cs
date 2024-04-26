using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private CardDisplay[] cardDisplay; 

    public GameObject[] cardGO;

    private CardSO[] card;
    void Start()
    {
        card = new CardSO[5];

        for (int i = 0; i < cardGO.Length; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    void Update()
    {
        if (playerInventory.GetCurrentCard() >= 0)
        {
            for (int i = playerInventory.GetCurrentCard(); i < 5; i++)
            {
                card[i] = playerInventory.GetCardOnInventory(playerInventory.GetRandomCard());

                cardDisplay[i].ShowCard(card[i]);

                for (int j = playerInventory.GetCurrentCard(); j < 5; j++) 
                {
                    cardGO[i].SetActive(true);
                }

                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            for (int i = 0; i < cardGO.Length; i++)
            {
                cardGO[i].SetActive(false);
            }
        }
    }
}
