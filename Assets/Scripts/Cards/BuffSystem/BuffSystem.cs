using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private PlayerEntitySO player;

    private float resetDashSpeed;

    private void Start()
    {
        resetDashSpeed = player.dashSpeed;
    }

    public void CheckCardType(CardSO card)
    {
        switch (card.cardType)
        {
            case CardSO.CardType.Attack:

                if (card.isInverted)
                {
                    InvertedAttackCard(card);
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
                    InvertedSpeedCard(card);
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

    private void InvertedAttackCard(CardSO card)
    {
        float newDamage = 1.5f;

        float damage = card.damage;

        damage *= card.cardsOnSlot ;

        player.damage += damage * newDamage;
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

        int defaultChance = 10;
        int firstChance = 15;
        int secondChance = 20;
        int thirdChance = 25;
        int fourthChance = 30;

        if (randomNumber <= firstChance)
            player.healingValue = firstChance;

        if (randomNumber > firstChance && randomNumber <= secondChance)
            player.healingValue = secondChance;

        if (randomNumber > secondChance && randomNumber <= thirdChance)
            player.healingValue = thirdChance;

        if (randomNumber > thirdChance && randomNumber <= fourthChance)
            player.healingValue = fourthChance;

        else
            player.healingValue = defaultChance;
        
    }

    private void UpgradeSpeed(CardSO card)
    {
        float speed = card.speed;

        speed *= card.cardsOnSlot;

        player.speed += speed;
    }

    private void InvertedSpeedCard(CardSO card) 
    {
        int randomNumber = Random.Range(0, 100);

        int firstChance = 15;
        int secondChance = 20;
        int thirdChance = 25;
        int fourthChance = 30;

        player.dashSpeed = resetDashSpeed;

        if (randomNumber <= firstChance) 
            player.dashSpeed += 1f;

        if (randomNumber > firstChance && randomNumber <= secondChance)
            player.dashSpeed += 2f;

        if (randomNumber > secondChance && randomNumber <= thirdChance)
            player.dashSpeed += 3f;

        if (randomNumber > thirdChance && randomNumber <= fourthChance)
            player.dashSpeed += 4f;

        else
            player.dashSpeed += 1f;        
    }
}
