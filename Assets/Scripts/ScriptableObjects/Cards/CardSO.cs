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

    public Sprite artMidground_1;
    public Vector3 newPosition1 = new Vector3(0, 0, 75);
    public Vector2 newSizeDelta1 = new Vector2(500, 500);
    public Sprite artMidground_2;
    public Vector3 newPosition2 = new Vector3(0, 0, 50);
    public Vector2 newSizeDelta2 = new Vector2(500, 500);
    public Sprite artMidground_3;
    public Vector3 newPosition3 = new Vector3(0, 0, 15);
    public Vector2 newSizeDelta3 = new Vector2(500, 500);
    public Sprite artFrame;

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
    }
}