using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image allCardSprite;

    public void ShowCard(CardSO card) 
    {
        allCardSprite.sprite = card.allCardSprite;
    }
}
