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

    public Image artMidground_1;
    public Image artMidground_2;
    public Image artMidground_3;
    public Image artFrame;

    public void ShowCardImage(CardSO card)
    {
        artMidground_1.sprite = card.artMidground_1;
        artMidground_2.sprite = card.artMidground_2;
        artMidground_3.sprite = card.artMidground_3;
        artFrame.sprite = card.artFrame;
    }

    public void ShowCardDescription(CardSO card)
    {
        name.text = card.name;
        description.text = card.description;
    }
}
