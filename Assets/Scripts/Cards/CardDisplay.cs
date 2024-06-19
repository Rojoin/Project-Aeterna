using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image artImage;

    public void ShowCard(CardSO card) 
    {
        artImage.sprite = card.cardSprite;
    }
}
