using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCardDisplay : MonoBehaviour
{
    public TextMeshProUGUI description;

    public Image artMidground_1;

    public Image artMidground_2;
    public Image artMidground_3;
    public Image artFrame;

    public void ShowCardImage(CardSO card)
    {
        artMidground_1.sprite = card.artMidground_1 ? card.artMidground_1 : null;
        artMidground_1.rectTransform.anchoredPosition3D = card.newPosition1;
        artMidground_1.rectTransform.sizeDelta = card.newSizeDelta1;
        artMidground_2.sprite = card.artMidground_2 ? card.artMidground_2 : null;
        artMidground_2.rectTransform.sizeDelta = card.newSizeDelta2;
        artMidground_2.rectTransform.anchoredPosition3D = card.newPosition2;
        artMidground_3.sprite = card.artMidground_3 ? card.artMidground_3 : null;
        artMidground_3.rectTransform.sizeDelta = card.newSizeDelta3;
        artMidground_3.rectTransform.anchoredPosition3D = card.newPosition3;
        artFrame.sprite = card.artFrame;
    }

    public void ShowCardDescription(CardSO card)
    {
        description.text = card.description;
    }
}