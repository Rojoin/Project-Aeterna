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
        // if (card.isInverted) 
        // {
        //     allCardSprite.rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        // }

        allCardSprite.sprite = card.allCardSprite;
    }
}
