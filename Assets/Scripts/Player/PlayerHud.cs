using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private SelectCardMenu selectCardMenu;

    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI speed;

    [SerializeField] private List<CardDisplay> cardDisplay;

    public List<GameObject> cardGO;

    private CardSO card;

    private int cardIndex;

    void Start()
    {
        card = ScriptableObject.CreateInstance<CardSO>();

        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowCardsHud(int cardSelected) 
    {
        if (playerInventory.GetCurrentCards() <= playerInventory.GetMaxCards()) 
        {
            for (int i = cardIndex; cardIndex < playerInventory.GetCurrentCards();)
            {
                card = playerInventory.GetCardOnInventory(cardSelected);

                cardDisplay[i].ShowCard(card);

                cardGO[i].SetActive(true);

                cardIndex++;

                break;
            }
        }
    }

    public void ShowHud() 
    {
        health.text = "Health: " + playerHealth.GetHealth().ToString();
        damage.text = "Damage: " + playerHealth.GetDamage().ToString();
        speed.text = "Speed: " + playerHealth.GetSpeed().ToString();
    }

    public void DesactiveCardsGO() 
    {
        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }

        cardIndex = 0;
    } 
}
