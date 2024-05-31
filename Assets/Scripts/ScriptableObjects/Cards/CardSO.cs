using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Health,
        Speed
    }

    public new string name;

    public string description;

    public Sprite cardSprite;

    public int ID;

    private int m_cardsOnSlot;
    public int cardsOnSlot;

    public bool isSelected;

    public CardType cardType;

    public float damage;

    public float health;

    public float speed;

    public void ResetStacks()
    {
        cardsOnSlot = m_cardsOnSlot;
    }

    private void OnEnable()
    {
        cardsOnSlot = m_cardsOnSlot;

        Debug.Log(nameof(CardSO) + "enable");

    }
}
