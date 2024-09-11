using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private PlayerEntitySO player;

    public void CheckCardType(CardSO card)
    {
        switch (card.cardType)
        {
            case CardSO.CardType.Attack:

                if (card.isInverted)
                {

                }

                else
                {
                    UpgradeAttack(card);
                }

                break;

            case CardSO.CardType.Health:

                if (card.isInverted)
                {

                }

                else
                {
                    UpgradeHealth(card);
                }

                break;

            case CardSO.CardType.Speed:

                if (card.isInverted)
                {

                }

                else
                {
                    UpgradeSpeed(card);
                }

                break;
        }
    }

    private void UpgradeAttack(CardSO card)
    {
        float damage = card.damage;

        damage *= card.cardsOnSlot;

        player.damage += damage;
    }

    private void UpgradeHealth(CardSO card)
    {
        float maxHealth = card.health;
        float health = card.health;
        
        maxHealth *= card.cardsOnSlot;
        health *= card.cardsOnSlot;

        player.health += maxHealth;
        player.health += health;
    }

    private void UpgradeSpeed(CardSO card)
    {
        float speed = card.speed;

        speed *= card.cardsOnSlot;

        player.speed += card.speed;
    }
}
