using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public new TextMeshProUGUI name;
    public TextMeshProUGUI description;

    public Image artImage;

    public void ShowCard(CardSO card) 
    {
        name.text = card.name.ToString();
        description.text = card.description.ToString();

        artImage.sprite = card.cardSprite;
    }
}
