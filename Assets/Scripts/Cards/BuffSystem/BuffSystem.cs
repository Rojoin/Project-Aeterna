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
                    InvertedHealthCard(card);
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

        maxHealth *= card.cardsOnSlot;

        player.maxHealth += maxHealth;

        if (player.health == player.maxHealth)
        {
            player.health += maxHealth;
        }

        else
        {
            player.health += card.health;
        }
    }

    private void InvertedHealthCard(CardSO card)
    {
        int randomNumber = Random.Range(0, 100);

        int firstChanse = 15;
        int secondChanse = 20;
        int thirdChanse = 25;
        int fourthChanse = 30;

        if (card.cardsOnSlot == 1) 
        {
            if (randomNumber <= firstChanse)
                player.healingValue = firstChanse;

            if (randomNumber > firstChanse && randomNumber <= secondChanse)
                player.healingValue = secondChanse;

            if (randomNumber > secondChanse && randomNumber <= thirdChanse)
                player.healingValue = thirdChanse;

            if (randomNumber > thirdChanse && randomNumber <= fourthChanse)
                player.healingValue = fourthChanse;

            else
                player.healingValue = 0;
        }
    }

    private void UpgradeSpeed(CardSO card)
    {
        float speed = card.speed;

        speed *= card.cardsOnSlot;

        player.speed += card.speed;
    }
}
