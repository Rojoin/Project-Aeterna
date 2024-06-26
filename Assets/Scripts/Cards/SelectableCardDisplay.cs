using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCardDisplay : MonoBehaviour
{
    public new TextMeshProUGUI name;
    public TextMeshProUGUI description;

    public Image artImage;

    public void ShowCardImage(CardSO card)
    {
      //  artImage.sprite = card.cardSprite;
    }

    public void ShowCardDescription(CardSO card)
    {
        name.text = card.name;
        description.text = card.description;
    }
}
