using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCardDisplay : MonoBehaviour
{
    public TextMeshProUGUI description;
    public TextMeshProUGUI tittle;

    public Image artMidground_0;
    public Image artMidground_1;
    public Image artMidground_2;
    public Image artMidground_3;

    public Image artFrame;

    public void ShowCardImage(CardSO card)
    {
        artMidground_0.sprite = card.artMidground_0 ? card.artMidground_0 : null;

        artMidground_1.sprite = card.artMidground_1 ? card.artMidground_1 : null;

        artMidground_2.sprite = card.artMidground_2 ? card.artMidground_2 : null;

        artMidground_3.sprite = card.artMidground_3 ? card.artMidground_3 : null;

        artFrame.sprite = card.artFrame;
    }

    public void ShowCardDescription(CardSO card)
    {
        description.text = card.description;
    }

    public void ShowCardTittle(CardSO card) 
    {
        tittle.text = card.tittle;
    }
}