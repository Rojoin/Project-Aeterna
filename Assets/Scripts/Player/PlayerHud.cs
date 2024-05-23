using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
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

    [SerializeField] private RectTransform hudTransform;

    [Header("Setup")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 endPosition;

    [SerializeField] private float hudMovementSpeed;

    private CardSO card;

    private int cardIndex;

    public List<GameObject> cardGO;



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

    public void ActiveHud() 
    {
        hudTransform.anchoredPosition = Vector2.MoveTowards(hudTransform.anchoredPosition, initialPosition, hudMovementSpeed * Time.deltaTime);
    }

    public void DesactiveHud() 
    {
        hudTransform.anchoredPosition = Vector2.MoveTowards(hudTransform.anchoredPosition, endPosition, hudMovementSpeed * Time.deltaTime);
    }
}