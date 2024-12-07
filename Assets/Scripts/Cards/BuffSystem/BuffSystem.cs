using UnityEngine;
using static CardSO;

public class BuffSystem : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private PlayerEntitySO player;
    [Header("Particle")]
    [SerializeField] private ParticleSystem vfxAura;

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

            case CardSO.CardType.AttackSpeed:

                if (card.isInverted)
                {
                    InvertedAttackSpeed(card);
                }

                else
                {
                    UpgradeAttackSpeed(card);
                }

                break;
        }

        vfxAura?.Play();
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
        float magicDamage = card.removeSpecialDamage;

        damage *= card.cardsOnSlot;
        magicDamage *= card.cardsOnSlot;

        player.damage += damage * newDamage;
        player.specialAttackDamage += magicDamage;
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
        player.healingValue += 10;
    }

    private void UpgradeSpeed(CardSO card)
    {
        float speed = card.speed;

        speed *= card.cardsOnSlot;

        player.speed += speed;
    }

    private void InvertedSpeedCard(CardSO card)
    {
        player.dashSpeed += 3f;
    }

    private void UpgradeAttackSpeed(CardSO card)
    {
        float newAttackSpeed = 0.1f;

        float attackSpeed = card.attackSpeed;

        attackSpeed += card.cardsOnSlot;

        player.attackSpeed += attackSpeed * newAttackSpeed;
    }

    private void InvertedAttackSpeed(CardSO card)
    {
        player.hasReverseTheStars = true;

        float damage = player.theStarDamage;

        damage *= card.cardsOnSlot;

        player.theStarDamage += damage;
    }

    public string GetShortDescription(CardSO card)
    {
        string greenHex = "00dc7a";
        string redHex = "e7423c";

        switch (card.cardType)
        {
            case CardType.Attack when card.isInverted:

                return $"<color=#{greenHex}>+{card.damage * card.cardsOnSlot}</color> Physical Damage\n <color=#{redHex}>{card.removeSpecialDamage * card.cardsOnSlot}</color> Magical Damage";

                break;

            case CardType.Attack:

                return $"<color=#{greenHex}>+{card.damage * card.cardsOnSlot}</color> Physical Damage";

                break;

            case CardType.Health when card.isInverted:

                float healingValue = player.healingValue;

                return $"<color=#{greenHex}>%{healingValue}</color> Healing Recieved";

                break;

            case CardType.Health:

                return $"<color=#{greenHex}>+{card.health * card.cardsOnSlot}</color> Max Health";

                break;

            case CardType.Speed when card.isInverted:

                return $"<color=#{greenHex}>+{player.dashSpeed}</color> Dash Speed";


                break;

            case CardType.Speed:

                return $"<color=#{greenHex}>+{card.speed * card.cardsOnSlot}</color> Movement Speed";

                break;

            case CardType.AttackSpeed when card.isInverted:

                return $"<color=#{greenHex}>+{player.theStarDamage}</color> Damage in the last attack of the combo";

                break;

            case CardType.AttackSpeed:

                return $"<color=#{greenHex}>+{card.attackSpeed * card.cardsOnSlot}</color> Attack Speed";

                break;
        }

        return "Error";
    }
}
