using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private Image backGround;
    [SerializeField] private Image bar;
    [SerializeField] private RectTransform barEndPos;
    private float fillAmount;

    public float FillAmount
    {
        get => fillAmount;
        set
        {
            fillAmount = value;
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        bar.fillAmount = FillAmount;
    }
}