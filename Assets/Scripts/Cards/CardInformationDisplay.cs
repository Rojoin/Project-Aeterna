using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInformationDisplay : MonoBehaviour
{
    public Image frame;
    public Image cardArt;

    public Sprite invertedFrame;
    public Sprite normalFrame;

    public TextMeshProUGUI description;

    public void ShowCardImage(CardSO card) 
    {
        if (card != null) 
        {
            if (card.isInverted)
            {
                frame.sprite = invertedFrame;
            }

            else
            {
                frame.sprite = normalFrame;
            }

            cardArt.sprite = card.allCardSprite;
        }
    }

    public void ShowCardDescription(string text) 
    {
        if(text != null) 
        {
            description.text = text;
        }
    }
}
