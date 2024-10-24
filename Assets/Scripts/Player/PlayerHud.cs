using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private List<GameObject> initalTemplate;

    [SerializeField] private List<GameObject> cardGO;

    [SerializeField] private List<CardDisplay> cardDisplay;

    private List<CardSO> invetory;

    void Start()
    {
        invetory = playerInventory.GetInventory();

        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowCardsHud() 
    {
        for (int i = 0; i < playerInventory.GetCurrentCards(); i++) 
        {
            initalTemplate[i].SetActive(false);
            cardDisplay[i].ShowCard(invetory[i]);
            cardGO[i].SetActive(true);
        }
    }

    public void DesactiveCardsGO() 
    {
        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    } 
}