using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInformationDisplay : MonoBehaviour
{
    public Image frame;
    public Image cardArt;
    public GameObject textBox;

    public Sprite invertedFrame;
    public Sprite normalFrame;

    public TextMeshProUGUI description;

    public void ShowCardImage(CardSO card) 
    {
        if (card != null) 
        {
            frame.enabled = true;
            textBox.SetActive(true);

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

        if (card == null) 
        {
            frame.enabled = false;
            textBox.SetActive(false);
        }
    }

    public void ShowCardDescription(string text) 
    {
        if(text != null) 
        {
            description.text = text;
        }
    }

    private void OnEnable()
    {
        ShowCardImage(null);
    }
}
