using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Scriptable Object: Card")]
    public Card card;

    [Header("Setup")]
    public new TextMeshProUGUI name;
    public TextMeshProUGUI description;

    public Image artImage;

    void Start()
    {
        name.text = card.name.ToString();
        description.text = card.description.ToString();

        artImage.sprite = card.cardSprite;
    }
}
