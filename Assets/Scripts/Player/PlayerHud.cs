using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private CardSO card;

    private int slotIndex;

    private List<CardSO> invetory;

    public List<GameObject> cardGO;



    void Start()
    {
        card = ScriptableObject.CreateInstance<CardSO>();

        for (int i = 0; i < cardGO.Count; i++)
        {
            cardGO[i].SetActive(false);
        }
    }

    public void ShowCardsHud(CardSO cardSelected) 
    {
        invetory = playerInventory.GetPlayerCardsInventoryList();

        for (int i = slotIndex; i < playerInventory.GetCurrentCards();)
        {
            for (int j = 0; j < invetory.Count; j++) 
            {
                if (cardSelected.ID != invetory[j].ID)
                {
                    slotIndex += 1;
                    break;
                }
            }
            break;
        }

        for (int i = 0; i < invetory.Count; i++)
        {
            cardDisplay[i].ShowCard(invetory[i]);

            cardGO[i].SetActive(true);
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

        slotIndex = 0;
    } 
}